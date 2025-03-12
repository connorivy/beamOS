using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.OpenSees.Tcp;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.Extensions.Logging;

namespace BeamOs.StructuralAnalysis.Domain.OpenSees;

public sealed class OpenSeesAnalysisModel(Model model, UnitSettings unitSettings, ILogger logger)
    : IDisposable
{
    private readonly TcpServer displacementServer = TcpServer.CreateStarted(logger);
    private readonly TcpServer reactionServer = TcpServer.CreateStarted(logger);
    private readonly TcpServer elementForceServer = TcpServer.CreateStarted(logger);

    public async Task<AnalysisResults> RunAnalysis()
    {
        TclWriter tclWriter = CreateWriterFromModel(
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

        string outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        await this.RunTclWithOpenSees(tclWriter.OutputFileWithPath, outputDir);

        var resultSet = new ResultSet(model.Id) { NodeResults = this.GetResults(model, tclWriter) };

        var dsmElements =
            model.Settings.AnalysisSettings.Element1DAnalysisType == Element1dAnalysisType.Euler
                ? model.Element1ds.Select(el => new DsmElement1d(el)).ToArray()
                : model.Element1ds.Select(el => new TimoshenkoDsmElement1d(el)).ToArray();

        var otherResults = resultSet.ComputeDiagramsAndElement1dResults(dsmElements, unitSettings);

        return new AnalysisResults()
        {
            ResultSet = resultSet,
            OtherAnalyticalResults = otherResults
        };
    }

    private static TclWriter CreateWriterFromModel(
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

        //this.element1dCache = new(model.Element1ds.Count);
        foreach (var element1d in model.Element1ds)
        {
            //this.element1dCache.Add(element1d.Id, element1d);
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

    //private double[] elemForces;

    private async Task RunTclWithOpenSees(string tclFileWithPath, string outputDir)
    {
        Task listenDisp = this.displacementServer.Listen(data => this.displacements = data);
        Task listenReact = this.reactionServer.Listen(data => this.reactions = data);
        //Task listenElemForces = this.elementForceServer.Listen(data => this.elemForces = data);

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
        await Task.WhenAny(Task.WhenAll(listenDisp, listenReact), Task.Delay(15000));

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
        //if (listenElemForces.Status != TaskStatus.RanToCompletion)
        //{
        //    logger.LogError(
        //        "Unable to receive the local element forces from OpenSees within the timeframe"
        //    );
        //}

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
    }

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
