using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;

public class ResultSet : BeamOsModelEntity<ResultSetId>
{
    // public ResultSet(ModelId modelId, LoadCombinationId loadCombinationId, ResultSetId? id = null)
    //     : base(id ?? new(), modelId)
    // {
    //     this.LoadCombinationId = loadCombinationId;
    // }

    public ResultSet(ModelId modelId, LoadCombinationId id)
        : base(new(id), modelId) { }

    public LoadCombinationId LoadCombinationId => new LoadCombinationId(this.Id);
    public LoadCombination? LoadCombination { get; set; }
    public IList<NodeResult>? NodeResults { get; set; }
    public IList<Element1dResult>? Element1dResults { get; set; }

    //public ICollection<ShearForceDiagram>? ShearForceDiagrams { get; set; }
    //public ICollection<MomentDiagram>? MomentDiagrams { get; set; }
    public OtherAnalyticalResults ComputeDiagramsAndElement1dResults(
        IList<DsmElement1d> dsmElement1Ds,
        UnitSettings unitSettings
    ) => this.ComputeDiagrams(dsmElement1Ds, unitSettings, true);

    public OtherAnalyticalResults ComputeDiagramsFromExistingResults(
        IList<DsmElement1d> dsmElement1Ds,
        UnitSettings unitSettings
    ) => this.ComputeDiagrams(dsmElement1Ds, unitSettings, false);

    private OtherAnalyticalResults ComputeDiagrams(
        IList<DsmElement1d> dsmElement1Ds,
        UnitSettings unitSettings,
        bool createElementResults = false
    )
    {
        ShearForceDiagram[] shearForceDiagrams = new ShearForceDiagram[dsmElement1Ds.Count];
        MomentDiagram[] momentDiagrams = new MomentDiagram[dsmElement1Ds.Count];
        Element1dResult[] element1dResults = new Element1dResult[dsmElement1Ds.Count];
        DeflectionDiagrams[] displacementResults = new DeflectionDiagrams[dsmElement1Ds.Count];

        Dictionary<int, Element1dResult> elementResultsDict;
        if (createElementResults)
        {
            this.Element1dResults ??= new List<Element1dResult>();
            elementResultsDict = null;
        }
        else
        {
            elementResultsDict = this.Element1dResults.ToDictionary(n => n.Id.Id, n => n);
        }
        Dictionary<int, NodeResult> nodeResultDict = this.NodeResults.ToDictionary(
            n => n.Id.Id,
            n => n
        );

        double globalShearMin = double.MaxValue;
        double globalShearMax = double.MinValue;
        double globalMomentMin = double.MaxValue;
        double globalMomentMax = double.MinValue;

        for (int i = 0; i < dsmElement1Ds.Count; i++)
        {
            var element = dsmElement1Ds[i];

            var (localElementDisplacements, localMemberEndForcesVector) =
                GetLocalElementDisplacementsAndForces(element, nodeResultDict, unitSettings);

            var sfd = ShearForceDiagram.Create(
                this.ModelId,
                this.Id,
                element.Element1dId,
                element.StartPoint,
                element.EndPoint,
                element.SectionProfileRotation,
                element.Length,
                localMemberEndForcesVector,
                unitSettings.LengthUnit,
                unitSettings.ForceUnit,
                unitSettings.TorqueUnit,
                LinearCoordinateDirection3D.AlongY
            );
            shearForceDiagrams[i] = sfd;

            momentDiagrams[i] = MomentDiagram.Create(
                this.ModelId,
                this.Id,
                element.Element1dId,
                element.StartPoint,
                element.EndPoint,
                element.SectionProfileRotation,
                element.Length,
                localMemberEndForcesVector,
                unitSettings.LengthUnit,
                unitSettings.ForceUnit,
                unitSettings.TorqueUnit,
                LinearCoordinateDirection3D.AlongY,
                sfd
            );

            displacementResults[i] = DeflectionDiagrams.Create(
                element.Element1dId,
                element.StartPoint,
                element.EndPoint,
                element.SectionProfileRotation,
                unitSettings.LengthUnit,
                localElementDisplacements,
                out var displacementMin,
                out var displacementMax
            );

            double shearMin = double.MaxValue;
            double shearMax = double.MinValue;
            double momentMin = double.MaxValue;
            double momentMax = double.MinValue;

            if (createElementResults)
            {
                sfd.MinMax(ref shearMin, ref shearMax);
                momentDiagrams[i].MinMax(ref momentMin, ref momentMax);
                this.Element1dResults.Add(
                    new(this.ModelId, this.Id, element.Element1dId)
                    {
                        MaxMoment = new(momentMax, unitSettings.TorqueUnit),
                        MinMoment = new(momentMin, unitSettings.TorqueUnit),
                        MaxShear = new(shearMax, unitSettings.ForceUnit),
                        MinShear = new(shearMin, unitSettings.ForceUnit),
                        MaxDisplacement = new(displacementMax, unitSettings.LengthUnit),
                        MinDisplacement = new(displacementMin, unitSettings.LengthUnit),
                    }
                );
            }
            else
            {
                var elementResult = elementResultsDict[element.Element1dId];
                shearMin = elementResult.MinShear.As(unitSettings.ForceUnit);
                shearMax = elementResult.MaxShear.As(unitSettings.ForceUnit);
                momentMin = elementResult.MinMoment.As(unitSettings.TorqueUnit);
                momentMax = elementResult.MaxMoment.As(unitSettings.TorqueUnit);
            }
            globalShearMin = Math.Min(globalShearMin, shearMin);
            globalShearMax = Math.Max(globalShearMax, shearMax);
            globalMomentMin = Math.Min(globalMomentMin, momentMin);
            globalMomentMax = Math.Max(globalMomentMax, momentMax);
        }

        Domain.DirectStiffnessMethod.GlobalStresses stresses = new()
        {
            MaxMoment = new(globalMomentMax, unitSettings.TorqueUnit),
            MaxShear = new(globalShearMax, unitSettings.ForceUnit),
            MinMoment = new(globalMomentMin, unitSettings.TorqueUnit),
            MinShear = new(globalShearMin, unitSettings.ForceUnit),
        };

        return new()
        {
            ModelId = this.ModelId,
            Id = this.Id,
            ShearDiagrams = shearForceDiagrams,
            MomentDiagrams = momentDiagrams,
            //Element1dResults = element1dResults,
            DeflectionDiagrams = displacementResults,
            GlobalStresses = stresses,
        };
    }

    private static (double[], double[]) GetLocalElementDisplacementsAndForces(
        DsmElement1d dsmElement1d,
        Dictionary<int, NodeResult> nodeResultsDict,
        UnitSettings unitSettings
    )
    {
        var elementDisplacements = new double[12];
        nodeResultsDict[dsmElement1d.StartNodeId]
            .Displacements.CopyTo(
                elementDisplacements.AsSpan(0, 6),
                unitSettings.LengthUnit,
                unitSettings.AngleUnit
            );
        nodeResultsDict[dsmElement1d.EndNodeId]
            .Displacements.CopyTo(
                elementDisplacements.AsSpan(6, 6),
                unitSettings.LengthUnit,
                unitSettings.AngleUnit
            );

        var globlEndDisplacementVector = Vector<double>.Build.Dense(elementDisplacements);
        var localEndDisplacementVector =
            dsmElement1d.GetTransformationMatrix() * globlEndDisplacementVector;

        var elementForces =
            dsmElement1d.GetLocalStiffnessMatrix(
                unitSettings.ForceUnit,
                unitSettings.ForcePerLengthUnit,
                unitSettings.TorqueUnit
            ) * localEndDisplacementVector;

        return (localEndDisplacementVector.AsArray(), elementForces.AsArray());
    }

    [Obsolete("EF Core Constructor", true)]
    private ResultSet() { }
}
