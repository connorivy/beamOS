using System.Diagnostics;
using System.Reflection;
using BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Interfaces;
using BeamOs.Application.AnalyticalResults.ModelResults;
using BeamOs.Application.AnalyticalResults.NodeResults;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Contracts.Common;
using BeamOs.Domain.AnalyticalResults.Common.ValueObjects;
using BeamOs.Domain.AnalyticalResults.ModelResultAggregate;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.OpenSees;
using BeamOs.Domain.OpenSees.TcpServer;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra.Double;

namespace BeamOs.Application.OpenSees;

public class GetTclFromModelCommandHandler(
    IModelRepository modelRepository,
    IModelResultRepository modelResultRepository,
    INodeResultRepository nodeResultRepository,
    IShearDiagramRepository shearDiagramRepository,
    IMomentDiagramRepository momentDiagramRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<ModelIdRequest, string>
{
    public async Task<string> ExecuteAsync(ModelIdRequest command, CancellationToken ct = default)
    {
        ModelId modelId = new(Guid.Parse(command.ModelId));
        string outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        var model = await modelRepository.GetById(
            modelId,
            ct,
            nameof(Model.Element1ds),
            $"{nameof(Model.Element1ds)}.{nameof(Element1D.SectionProfile)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1D.Material)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1D.StartNode)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1D.EndNode)}",
            nameof(Model.PointLoads)
        );

        int displacementPort = 1024;
        int reactionPort = displacementPort + 1;
        int elementForcesPort = displacementPort + 2;
        TclWriter tclWriter = this.CreateWriterFromModel(
            outputDir,
            model,
            displacementPort,
            reactionPort,
            elementForcesPort
        );

        await this.RunTclWithOpenSees(outputDir, displacementPort, reactionPort, elementForcesPort);

        var results = this.GetResults(modelId, model, tclWriter);

        foreach (var nodeResult in results.NodeResults ?? Enumerable.Empty<NodeResult>())
        {
            nodeResultRepository.Add(nodeResult);
        }
        foreach (
            var shearForceDiagram in results.ShearForceDiagrams
                ?? Enumerable.Empty<ShearForceDiagram>()
        )
        {
            shearDiagramRepository.Add(shearForceDiagram);
        }

        foreach (var momentDiagram in results.MomentDiagrams ?? Enumerable.Empty<MomentDiagram>())
        {
            momentDiagramRepository.Add(momentDiagram);
        }

        modelResultRepository.Add(
            new ModelResult(
                model.Id,
                results.MaxShearValue,
                results.MinShearValue,
                results.MaxMomentValue,
                results.MinMomentValue
            )
        );

        await unitOfWork.SaveChangesAsync(ct);

        //await unitOfWork.SaveChangesAsync();

        return tclWriter.ToString();
    }

    private Dictionary<Element1DId, Element1D> element1dCache;

    private TclWriter CreateWriterFromModel(
        string outputDir,
        Model? model,
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
        tclWriter.Write(Path.Combine(outputDir, "model.tcl"));

        return tclWriter;
    }

    private double[] displacements;
    private double[] reactions;
    private double[] elemForces;

    private async Task RunTclWithOpenSees(
        string outputDir,
        int displacementPort,
        int reactionPort,
        int elementForcesPort
    )
    {
        using TcpServer displacementServer = new(displacementPort);
        using TcpServer reactionServer = new(reactionPort);
        using TcpServer elementForces = new(elementForcesPort);

        //using SocketListener displacementServer = new(displacementPort);
        //using SocketListener reactionServer = new(reactionPort);
        //using SocketListener elementForces = new(elementForcesPort);

        Task listenDisp = displacementServer.Listen(data => this.displacements = data);
        Task listenReact = reactionServer.Listen(data => this.reactions = data);
        Task listenElemForces = elementForces.Listen(data => this.elemForces = data);
        //elementForces.ListenCallback(data => this.elemForces = data);

        Process process =
            new()
            {
                StartInfo = new ProcessStartInfo(Path.Combine(outputDir, "OpenSees.exe"))
                {
                    WorkingDirectory = outputDir,
                    Arguments = Path.Combine(outputDir, "model.tcl"),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true,
            };
        process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
        process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        // todo : failure policy
        await listenDisp;
        await listenReact;
        await listenElemForces;

        await process.WaitForExitAsync();
        //await TcpServerCallback.Result;
    }

    private ModelResults GetResults(ModelId modelId, Model? model, TclWriter tclWriter)
    {
        NodeResult[] nodeResults = new NodeResult[this.displacements.Length / 6];
        for (int i = 0; i < this.displacements.Length / 6; i++)
        {
            int indexOffset = i * 6;
            nodeResults[i] = new NodeResult(
                modelId,
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

        ShearForceDiagram[] shearForceDiagrams = new ShearForceDiagram[this.element1dCache.Count];
        MomentDiagram[] momentDiagrams = new MomentDiagram[this.element1dCache.Count];
        double shearMin = double.MaxValue;
        double shearMax = double.MinValue;
        double momentMin = double.MaxValue;
        double momentMax = double.MinValue;
        for (int i = 0; i < this.elemForces.Length / 12; i++)
        {
            int indexOffset = i * 12;
            var element1d = this.element1dCache[tclWriter.GetElementIdFromOutputIndex(i)];
            //var globalMemberEndForcesVector = Vector
            //    .Build
            //    .Dense(

            //        [
            //            this.elemForces[indexOffset],
            //            this.elemForces[indexOffset + 1],
            //            this.elemForces[indexOffset + 2],
            //            this.elemForces[indexOffset + 3],
            //            this.elemForces[indexOffset + 4],
            //            this.elemForces[indexOffset + 5],
            //            this.elemForces[indexOffset + 6],
            //            this.elemForces[indexOffset + 7],
            //            this.elemForces[indexOffset + 8],
            //            this.elemForces[indexOffset + 9],
            //            this.elemForces[indexOffset + 10],
            //            this.elemForces[indexOffset + 11],
            //        ]
            //    );
            //var localMemberEndForcesVector =
            //    DenseMatrix.OfArray(element1d.GetTransformationMatrix()).Transpose()
            //    * globalMemberEndForcesVector;


            var localMemberEndForcesVector = Vector
                .Build
                .Dense(

                    [
                        this.elemForces[indexOffset],
                        this.elemForces[indexOffset + 1],
                        this.elemForces[indexOffset + 2],
                        this.elemForces[indexOffset + 3],
                        this.elemForces[indexOffset + 4],
                        this.elemForces[indexOffset + 5],
                        this.elemForces[indexOffset + 6],
                        this.elemForces[indexOffset + 7],
                        this.elemForces[indexOffset + 8],
                        this.elemForces[indexOffset + 9],
                        this.elemForces[indexOffset + 10],
                        this.elemForces[indexOffset + 11],
                    ]
                );

            var sfd = ShearForceDiagram.Create(
                element1d.Id,
                element1d.StartNode.LocationPoint,
                element1d.EndNode.LocationPoint,
                element1d.SectionProfileRotation,
                element1d.Length,
                localMemberEndForcesVector,
                model.Settings.UnitSettings.LengthUnit,
                model.Settings.UnitSettings.ForceUnit,
                model.Settings.UnitSettings.TorqueUnit,
                LinearCoordinateDirection3D.AlongY
            );

            shearForceDiagrams[i] = sfd;
            sfd.MinMax(ref shearMin, ref shearMax);

            momentDiagrams[i] = MomentDiagram.Create(
                model.Id,
                element1d.Id,
                element1d.StartNode.LocationPoint,
                element1d.EndNode.LocationPoint,
                element1d.SectionProfileRotation,
                element1d.Length,
                localMemberEndForcesVector,
                model.Settings.UnitSettings.LengthUnit,
                model.Settings.UnitSettings.ForceUnit,
                model.Settings.UnitSettings.TorqueUnit,
                LinearCoordinateDirection3D.AlongY,
                sfd
            );
            momentDiagrams[i].MinMax(ref momentMin, ref momentMax);
        }

        return new ModelResults
        {
            NodeResults = nodeResults,
            ShearForceDiagrams = shearForceDiagrams,
            MomentDiagrams = momentDiagrams,
            MaxShearValue = new(shearMax, model.Settings.UnitSettings.ForceUnit),
            MinShearValue = new(shearMin, model.Settings.UnitSettings.ForceUnit),
            MaxMomentValue = new(momentMax, model.Settings.UnitSettings.TorqueUnit),
            MinMomentValue = new(momentMin, model.Settings.UnitSettings.TorqueUnit),
        };
    }

    //void process_Exited(object sender, EventArgs e)
    //{
    //    Console.WriteLine(string.Format("process exited with code {0}\n", process.ExitCode.ToString()));
    //}

    static void process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        Console.WriteLine(e.Data + "\n");
    }

    static void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        Console.WriteLine(e.Data + "\n");
    }
}
