using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.OpenSees;
using BeamOs.StructuralAnalysis.Domain.OpenSees.TcpServer;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeamOs.StructuralAnalysis.Application.OpenSees;

public class RunOpenSeesCommandHandler(
    //{
    //}
    //public class RunOpenSeesAnalysisCommandHandler(
    IModelRepository modelRepository,
    IResultSetRepository resultSetRepository,
    //IModelResultRepository modelResultRepository,
    //INodeResultRepository nodeResultRepository,
    //IShearDiagramRepository shearDiagramRepository,
    //IMomentDiagramRepository momentDiagramRepository,
    IStructuralAnalysisUnitOfWork unitOfWork,
    ILogger<RunOpenSeesCommandHandler> logger
) : ICommandHandler<ModelId, int>
{
    public async Task<Result<int>> ExecuteAsync(ModelId modelId, CancellationToken ct = default)
    {
        string outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        var model = await modelRepository.GetSingle(
            modelId,
            static queryable =>
                queryable
                    .Include(m => m.PointLoads)
                    .Include(m => m.Element1ds)
                    .ThenInclude(el => el.StartNode)
                    .Include(m => m.Element1ds)
                    .ThenInclude(el => el.EndNode)
                    .Include(m => m.Element1ds)
                    .ThenInclude(el => el.SectionProfile)
                    .Include(m => m.Element1ds)
                    .ThenInclude(el => el.Material),
            ct
        //ct,
        //nameof(Model.Element1ds),
        //nameof(Model.Nodes),
        //$"{nameof(Model.Element1ds)}.{nameof(Element1d.SectionProfile)}",
        //$"{nameof(Model.Element1ds)}.{nameof(Element1d.Material)}",
        //$"{nameof(Model.Element1ds)}.{nameof(Element1d.StartNode)}",
        //$"{nameof(Model.Element1ds)}.{nameof(Element1d.EndNode)}",
        //nameof(Model.PointLoads)
        );

        if (model is null)
        {
            return BeamOsError.NotFound(description: $"Could not find model with id {modelId}");
        }

        int displacementPort = 1024;
        int reactionPort = displacementPort + 1;
        int elementForcesPort = displacementPort + 2;
        TclWriter tclWriter = this.CreateWriterFromModel(
            model,
            displacementPort,
            reactionPort,
            elementForcesPort
        );

        if (tclWriter.OutputFileWithPath is null)
        {
            throw new Exception(
                "OutputFileWithPath should not be null after calling TclWriter.Write()"
            );
        }

        await this.RunTclWithOpenSees(
            tclWriter.OutputFileWithPath,
            outputDir,
            displacementPort,
            reactionPort,
            elementForcesPort
        );

        var resultSet = new ResultSet(modelId);

        resultSet.NodeResults = this.GetResults(model, tclWriter);

        resultSetRepository.Add(resultSet);

        //foreach (var nodeResult in results.NodeResults ?? Enumerable.Empty<NodeResult>())
        //{
        //    nodeResultRepository.Add(nodeResult);
        //}
        //foreach (
        //    var shearForceDiagram in results.ShearForceDiagrams
        //        ?? Enumerable.Empty<ShearForceDiagram>()
        //)
        //{
        //    shearDiagramRepository.Add(shearForceDiagram);
        //}

        //foreach (var momentDiagram in results.MomentDiagrams ?? Enumerable.Empty<MomentDiagram>())
        //{
        //    momentDiagramRepository.Add(momentDiagram);
        //}

        //modelResultRepository.Add(analyticalResults);

        await unitOfWork.SaveChangesAsync(ct);

        //await unitOfWork.SaveChangesAsync();

        return resultSet.Id.Id;
    }

    private Dictionary<Element1dId, Element1d> element1dCache;

    private TclWriter CreateWriterFromModel(
        Model model,
        int displacementPort,
        int reactionPort,
        int elementForcesPort
    )
    {
        TclWriter tclWriter =
            new(model.Settings.UnitSettings, displacementPort, reactionPort, elementForcesPort);

        this.element1dCache = new(model.Element1ds.Count);
        foreach (var element1d in model.Element1ds)
        {
            this.element1dCache.Add(element1d.Id, element1d);
            tclWriter.AddHydratedElement(element1d);
        }
        tclWriter.AddPointLoads(model.PointLoads);

        tclWriter.DefineAnalysis();
        tclWriter.Write();

        return tclWriter;
    }

    private double[] displacements;
    private double[] reactions;
    private double[] elemForces;

    private async Task RunTclWithOpenSees(
        string tclFileWithPath,
        string outputDir,
        int displacementPort,
        int reactionPort,
        int elementForcesPort
    )
    {
        using TcpServer displacementServer = new(displacementPort, logger);
        using TcpServer reactionServer = new(reactionPort, logger);
        using TcpServer elementForces = new(elementForcesPort, logger);

        //using SocketListener displacementServer = new(displacementPort);
        //using SocketListener reactionServer = new(reactionPort);
        //using SocketListener elementForces = new(elementForcesPort);

        Task listenDisp = displacementServer.Listen(data => this.displacements = data);
        Task listenReact = reactionServer.Listen(data => this.reactions = data);
        Task listenElemForces = elementForces.Listen(data => this.elemForces = data);
        //elementForces.ListenCallback(data => this.elemForces = data);

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

        Process process =
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

        await process.WaitForExitAsync();
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
}
