using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOs.StructuralAnalysis.Application.AnalyticalResults;

public sealed class GetDiagramsCommandHandler(
    IModelRepository modelRepository,
    IResultSetRepository resultSetRepository
) : ICommandHandler<GetDiagramsCommand, AnalyticalResultsResponse>
{
    public async Task<Result<AnalyticalResultsResponse>> ExecuteAsync(
        GetDiagramsCommand command,
        CancellationToken ct = default
    )
    {
        ResultSetId resultSetId = new(command.Id);
        var elementResults = await resultSetRepository.GetSingle(
            command.ModelId,
            resultSetId,
            ct,
            $"{nameof(ResultSet.Element1dResults)}",
            $"{nameof(ResultSet.NodeResults)}"
        );

        if (elementResults is null)
        {
            return BeamOsError.NotFound(
                description: $"Could not find result set with id = {command.Id} for model {command.ModelId}"
            );
        }

        var model = await modelRepository.GetSingle(
            command.ModelId,
            ct,
            nameof(Model.Nodes),
            nameof(Model.Element1ds),
            //$"{nameof(Model.Element1ds)}.{nameof(Element1d.SectionProfile)}",
            //$"{nameof(Model.Element1ds)}.{nameof(Element1d.Material)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1d.StartNode)}",
            $"{nameof(Model.Element1ds)}.{nameof(Element1d.EndNode)}"
        //$"{nameof(Model.Nodes)}.{nameof(Node.PointLoads)}",
        //$"{nameof(Model.Nodes)}.{nameof(Node.MomentLoads)}"
        );

        var nodeResultsDict = elementResults.NodeResults.ToDictionary(n => n.Id.Id, n => n);
        var elementResultsDict = elementResults.Element1dResults.ToDictionary(n => n.Id.Id, n => n);

        Dictionary<int, Node> nodesDict = model.Nodes.ToDictionary(n => n.Id.Id, n => n);

        ShearForceDiagram[] shearForceDiagrams = new ShearForceDiagram[model.Element1ds.Count];
        MomentDiagram[] momentDiagrams = new MomentDiagram[model.Element1ds.Count];
        Element1dResult[] element1DResults = new Element1dResult[model.Element1ds.Count];
        DeflectionDiagrams[] displacementResults = new DeflectionDiagrams[model.Element1ds.Count];

        UnitSettings unitSettings;
        if (command.UnitsOverride is not null)
        {
            unitSettings = RunDirectStiffnessMethodCommandHandler.GetUnitSettings(
                command.UnitsOverride
            );
        }
        else
        {
            unitSettings = model.Settings.UnitSettings;
        }

        double globalShearMin = double.MaxValue;
        double globalShearMax = double.MinValue;
        double globalMomentMin = double.MaxValue;
        double globalMomentMax = double.MinValue;

        for (int i = 0; i < model.Element1ds.Count; i++)
        {
            var element = model.Element1ds[i];

            var elementDisplacements = new double[12];
            nodeResultsDict[element.StartNodeId]
                .Displacements
                .CopyTo(
                    elementDisplacements.AsSpan(0, 6),
                    unitSettings.LengthUnit,
                    unitSettings.AngleUnit
                );
            nodeResultsDict[element.EndNodeId]
                .Displacements
                .CopyTo(
                    elementDisplacements.AsSpan(6, 6),
                    unitSettings.LengthUnit,
                    unitSettings.AngleUnit
                );

            // todo : I could compute this possibly cheaply, I might not need to store it
            var elementForces = new double[12];
            var elementResult = elementResultsDict[element.Id];
            elementResult
                .LocalStartForces
                .CopyTo(
                    elementForces.AsSpan(0, 6),
                    unitSettings.ForceUnit,
                    unitSettings.TorqueUnit
                );
            elementResult
                .LocalEndForces
                .CopyTo(
                    elementForces.AsSpan(6, 6),
                    unitSettings.ForceUnit,
                    unitSettings.TorqueUnit
                );

            var sfd = ShearForceDiagram.Create(
                model.Id,
                resultSetId,
                element.Id,
                element.StartNode.LocationPoint,
                element.EndNode.LocationPoint,
                element.SectionProfileRotation,
                element.Length,
                elementForces,
                unitSettings.LengthUnit,
                unitSettings.ForceUnit,
                unitSettings.TorqueUnit,
                LinearCoordinateDirection3D.AlongY
            );
            shearForceDiagrams[i] = sfd;
            momentDiagrams[i] = MomentDiagram.Create(
                model.Id,
                resultSetId,
                element.Id,
                element.StartNode.LocationPoint,
                element.EndNode.LocationPoint,
                element.SectionProfileRotation,
                element.Length,
                elementForces,
                unitSettings.LengthUnit,
                unitSettings.ForceUnit,
                unitSettings.TorqueUnit,
                LinearCoordinateDirection3D.AlongY,
                sfd
            );
            var transformationMatrix = Element1d.GetTransformationMatrix(
                element.EndNode.LocationPoint,
                element.StartNode.LocationPoint,
                element.SectionProfileRotation
            );
            var x =
                Matrix<double>.Build.DenseOfArray(transformationMatrix)
                * Vector<double>.Build.Dense(elementDisplacements);

            // transform el displacemnts to local coords
            displacementResults[i] = DeflectionDiagrams.Create(
                element.Id,
                element.StartNode.LocationPoint,
                element.EndNode.LocationPoint,
                element.SectionProfileRotation,
                unitSettings.LengthUnit,
                x,
                out var displacementMin,
                out var displacementMax
            );

            globalShearMin = Math.Min(
                globalShearMin,
                elementResult.MinShear.As(unitSettings.ForceUnit)
            );
            globalShearMax = Math.Max(
                globalShearMax,
                elementResult.MaxShear.As(unitSettings.ForceUnit)
            );
            globalMomentMin = Math.Min(
                globalMomentMin,
                elementResult.MinMoment.As(unitSettings.TorqueUnit)
            );
            globalMomentMax = Math.Max(
                globalMomentMax,
                elementResult.MaxMoment.As(unitSettings.TorqueUnit)
            );
        }

        Domain.DirectStiffnessMethod.GlobalStresses globalStresses =
            new()
            {
                MaxMoment = new(globalMomentMax, unitSettings.TorqueUnit),
                MaxShear = new(globalShearMax, unitSettings.ForceUnit),
                MinMoment = new(globalMomentMin, unitSettings.TorqueUnit),
                MinShear = new(globalShearMin, unitSettings.ForceUnit)
            };

        OtherAnalyticalResults domainResults =
            new()
            {
                Id = resultSetId,
                ModelId = model.Id,
                DeflectionDiagrams = displacementResults,
                ShearDiagrams = shearForceDiagrams,
                MomentDiagrams = momentDiagrams,
                GlobalStresses = globalStresses
            };

        return domainResults.Map();
    }
}

public readonly struct GetDiagramsCommand : IHasModelId
{
    public Guid ModelId { get; init; }

    public int Id { get; init; }

    public string? UnitsOverride { get; init; }
}
