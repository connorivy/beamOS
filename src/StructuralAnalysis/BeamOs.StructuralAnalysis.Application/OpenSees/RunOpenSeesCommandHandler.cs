using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.OpenSees;
using BeamOs.StructuralAnalysis.Domain.OpenSees.TcpServer;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeamOs.StructuralAnalysis.Application.OpenSees;

public sealed class RunOpenSeesCommandHandler(
    IModelRepository modelRepository,
    IResultSetRepository resultSetRepository,
    IStructuralAnalysisUnitOfWork unitOfWork,
    ILogger<RunOpenSeesCommandHandler> logger
) : ICommandHandler<RunDsmCommand, AnalyticalResultsResponse>, IDisposable
{
    private readonly TcpServer displacementServer = TcpServer.CreateStarted(logger);
    private readonly TcpServer reactionServer = TcpServer.CreateStarted(logger);
    private readonly TcpServer elementForceServer = TcpServer.CreateStarted(logger);

    public async Task<Result<AnalyticalResultsResponse>> ExecuteAsync(
        RunDsmCommand command,
        CancellationToken ct = default
    )
    {
        string outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        var model = await modelRepository.GetSingle(
            command.ModelId,
            static queryable =>
                queryable
                    .Include(m => m.PointLoads)
                    .Include(m => m.MomentLoads)
                    .Include(m => m.Element1ds)
                    .ThenInclude(el => el.StartNode)
                    .Include(m => m.Element1ds)
                    .ThenInclude(el => el.EndNode)
                    .Include(m => m.Element1ds)
                    .ThenInclude(el => el.SectionProfile)
                    .Include(m => m.Element1ds)
                    .ThenInclude(el => el.Material),
            ct
        );

        if (model is null)
        {
            return BeamOsError.NotFound(
                description: $"Could not find model with id {command.ModelId}"
            );
        }

        UnitSettings unitSettings = model.Settings.UnitSettings;
        if (command.UnitsOverride is not null)
        {
            unitSettings = RunDirectStiffnessMethodCommandHandler.GetUnitSettings(
                command.UnitsOverride
            );
        }

        TclWriter tclWriter = this.CreateWriterFromModel(
            model,
            this.displacementServer.Port,
            this.reactionServer.Port,
            this.elementForceServer.Port,
            unitSettings
        );

        if (tclWriter.OutputFileWithPath is null)
        {
            throw new Exception(
                "OutputFileWithPath should not be null after calling TclWriter.Write()"
            );
        }

        await this.RunTclWithOpenSees(tclWriter.OutputFileWithPath, outputDir);

        var resultSet = new ResultSet(command.ModelId)
        {
            NodeResults = this.GetResults(model, tclWriter)
        };

        var dsmElements =
            model.Settings.AnalysisSettings.Element1DAnalysisType == Element1dAnalysisType.Euler
                ? model.Element1ds.Select(el => new DsmElement1d(el)).ToArray()
                : model.Element1ds.Select(el => new TimoshenkoDsmElement1d(el)).ToArray();

        var otherResults = resultSet.ComputeDiagramsAndElement1dResults(dsmElements, unitSettings);

        resultSetRepository.Add(resultSet);

        await unitOfWork.SaveChangesAsync(ct);

        return otherResults.Map();
    }

    private Dictionary<Element1dId, Element1d> element1dCache;

    private TclWriter CreateWriterFromModel(
        Model model,
        int displacementPort,
        int reactionPort,
        int elementForcesPort,
        UnitSettings? unitSettingsOverride = null
    )
    {
        TclWriter tclWriter =
            new(
                model.Settings,
                displacementPort,
                reactionPort,
                elementForcesPort,
                unitSettingsOverride
            );

        this.element1dCache = new(model.Element1ds.Count);
        foreach (var element1d in model.Element1ds)
        {
            this.element1dCache.Add(element1d.Id, element1d);
            tclWriter.AddHydratedElement(element1d);
        }

        Debug.Assert(model.PointLoads is not null);
        Debug.Assert(model.MomentLoads is not null);
        tclWriter.AddLoads(model.PointLoads, model.MomentLoads);

        tclWriter.DefineAnalysis();
        tclWriter.Write();

        return tclWriter;
    }

    private double[] displacements;
    private double[] reactions;
    private double[] elemForces;

    private async Task RunTclWithOpenSees(string tclFileWithPath, string outputDir)
    {
        Task listenDisp = this.displacementServer.Listen(data => this.displacements = data);
        Task listenReact = this.reactionServer.Listen(data => this.reactions = data);
        Task listenElemForces = this.elementForceServer.Listen(data => this.elemForces = data);

        string exeName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            exeName = "OpenSees";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            exeName = "OpenSees.exe";
        }
        else
        {
            throw new Exception("Program running on unsupported platform");
        }

        using Process process =
            new()
            {
                StartInfo = new ProcessStartInfo(Path.Combine(outputDir, "bin", exeName))
                {
                    WorkingDirectory = outputDir,
                    Arguments = tclFileWithPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true,
            };
        process.OutputDataReceived += new DataReceivedEventHandler(this.process_OutputDataReceived);
        process.ErrorDataReceived += new DataReceivedEventHandler(this.process_ErrorDataReceived);

        try
        {
            logger.LogInformation("Starting process");
            process.Start();
            logger.LogInformation("Process started");
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            throw new Exception(
                "Unable to run opensees.exe. Did you follow the instructions in beamOS/opensees/readme?",
                ex
            );
        }
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        // todo : failure policy
        await Task.WhenAny(
            Task.WhenAll(listenDisp, listenReact, listenElemForces),
            Task.Delay(15000)
        );

        if (listenDisp.Status != TaskStatus.RanToCompletion)
        {
            logger.LogError(
                "Unable to receive the node displacements from OpenSees within the timeframe"
            );
        }
        if (listenReact.Status != TaskStatus.RanToCompletion)
        {
            logger.LogError(
                "Unable to receive the node reactions from OpenSees within the timeframe"
            );
        }
        if (listenElemForces.Status != TaskStatus.RanToCompletion)
        {
            logger.LogError(
                "Unable to receive the local element forces from OpenSees within the timeframe"
            );
        }

        //await listenDisp;
        //await listenReact;
        //await listenElemForces;
        process.Kill();
        //await TcpServerCallback.Result;
    }

    //private AnalyticalResults GetResults(Model model, TclWriter tclWriter)
    private NodeResult[] GetResults(Model model, TclWriter tclWriter)
    {
        ResultSetId analyticalResultsId = new();

        NodeResult[] nodeResults = new NodeResult[this.displacements.Length / 6];
        for (int i = 0; i < this.displacements.Length / 6; i++)
        {
            int indexOffset = i * 6;
            nodeResults[i] = new NodeResult(
                model.Id,
                analyticalResultsId,
                tclWriter.GetNodeIdFromOutputIndex(i),
                new Forces(
                    this.reactions[indexOffset],
                    this.reactions[indexOffset + 1],
                    this.reactions[indexOffset + 2],
                    this.reactions[indexOffset + 3],
                    this.reactions[indexOffset + 4],
                    this.reactions[indexOffset + 5],
                    model.Settings.UnitSettings.ForceUnit,
                    model.Settings.UnitSettings.TorqueUnit
                ),
                new Displacements(
                    this.displacements[indexOffset],
                    this.displacements[indexOffset + 1],
                    this.displacements[indexOffset + 2],
                    this.displacements[indexOffset + 3],
                    this.displacements[indexOffset + 4],
                    this.displacements[indexOffset + 5],
                    model.Settings.UnitSettings.LengthUnit,
                    model.Settings.UnitSettings.AngleUnit
                )
            );
        }

        return nodeResults;

        //ShearForceDiagram[] shearForceDiagrams = new ShearForceDiagram[this.element1dCache.Count];
        //MomentDiagram[] momentDiagrams = new MomentDiagram[this.element1dCache.Count];
        //double shearMin = double.MaxValue;
        //double shearMax = double.MinValue;
        //double momentMin = double.MaxValue;
        //double momentMax = double.MinValue;
        //for (int i = 0; i < this.elemForces.Length / 12; i++)
        //{
        //    int indexOffset = i * 12;
        //    var element1d = this.element1dCache[tclWriter.GetElementIdFromOutputIndex(i)];

        //    var localMemberEndForcesVector = Vector
        //        .Build
        //        .Dense(

        //            [
        //                this.elemForces[indexOffset],
        //                this.elemForces[indexOffset + 1],
        //                this.elemForces[indexOffset + 2],
        //                this.elemForces[indexOffset + 3],
        //                this.elemForces[indexOffset + 4],
        //                this.elemForces[indexOffset + 5],
        //                this.elemForces[indexOffset + 6],
        //                this.elemForces[indexOffset + 7],
        //                this.elemForces[indexOffset + 8],
        //                this.elemForces[indexOffset + 9],
        //                this.elemForces[indexOffset + 10],
        //                this.elemForces[indexOffset + 11],
        //            ]
        //        );

        //    var sfd = ShearForceDiagram.Create(
        //        analyticalResultsId,
        //        element1d.Id,
        //        element1d.StartNode.LocationPoint,
        //        element1d.EndNode.LocationPoint,
        //        element1d.SectionProfileRotation,
        //        element1d.Length,
        //        localMemberEndForcesVector,
        //        model.Settings.UnitSettings.LengthUnit,
        //        model.Settings.UnitSettings.ForceUnit,
        //        model.Settings.UnitSettings.TorqueUnit,
        //        LinearCoordinateDirection3D.AlongY
        //    );

        //    shearForceDiagrams[i] = sfd;
        //    sfd.MinMax(ref shearMin, ref shearMax);

        //    momentDiagrams[i] = MomentDiagram.Create(
        //        analyticalResultsId,
        //        element1d.Id,
        //        element1d.StartNode.LocationPoint,
        //        element1d.EndNode.LocationPoint,
        //        element1d.SectionProfileRotation,
        //        element1d.Length,
        //        localMemberEndForcesVector,
        //        model.Settings.UnitSettings.LengthUnit,
        //        model.Settings.UnitSettings.ForceUnit,
        //        model.Settings.UnitSettings.TorqueUnit,
        //        LinearCoordinateDirection3D.AlongY,
        //        sfd
        //    );
        //    momentDiagrams[i].MinMax(ref momentMin, ref momentMax);
        //}

        //return new AnalyticalResults(model.Id)
        //{
        //    MaxShear = new(shearMax, model.Settings.UnitSettings.ForceUnit),
        //    MinShear = new(shearMin, model.Settings.UnitSettings.ForceUnit),
        //    MaxMoment = new(momentMax, model.Settings.UnitSettings.TorqueUnit),
        //    MinMoment = new(momentMin, model.Settings.UnitSettings.TorqueUnit),
        //    NodeResults = nodeResults,
        //    ShearForceDiagrams = shearForceDiagrams,
        //    MomentDiagrams = momentDiagrams
        //};
    }

    //void process_Exited(object sender, EventArgs e)
    //{
    //    Console.WriteLine(string.Format("process exited with code {0}\n", process.ExitCode.ToString()));
    //}

    void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        logger.LogError("OpenSees process error {data}", e.Data);
    }

    void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        logger.LogInformation("OpenSees process info {data}", e.Data);
    }

    public void Dispose()
    {
        this.displacementServer.Dispose();
        this.reactionServer.Dispose();
        this.elementForceServer.Dispose();
    }
}
