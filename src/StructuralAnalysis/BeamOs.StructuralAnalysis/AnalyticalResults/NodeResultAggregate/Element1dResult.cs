using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using CSparse.Double;
using CSparse.Storage;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;

public class Element1dResult : BeamOsAnalyticalResultEntity<Element1dId>
{
    public Element1dResult(ModelId modelId, ResultSetId resultSetId, Element1dId element1dId)
        : base(element1dId, resultSetId, modelId) { }

    //public required Forces LocalStartForces { get; init; }
    //public required Forces LocalEndForces { get; init; }

    //public required Displacements LocalStartDisplacements { get; init; }
    //public required Displacements LocalEndDisplacements { get; init; }

    public required Force MaxShear { get; init; }
    public required Force MinShear { get; init; }
    public required Torque MaxMoment { get; init; }
    public required Torque MinMoment { get; init; }

    public required Length MaxDisplacement { get; init; }
    public required Length MinDisplacement { get; init; }

    public Element1dId Element1dId => this.Id;

    //public Element1d? Element1d { get; set; }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Element1dResult() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public readonly record struct DeflectionDiagrams(
    Element1dId Element1dId,
    int NumSteps,
    double[] Offsets
)
{
    public static DeflectionDiagrams Create(
        Element1dId element1DId,
        Point startPoint,
        Point endPoint,
        Angle sectionProfileRotation,
        LengthUnit lengthUnit,
        Span<double> localElementDisplacements
    ) =>
        Create(
            element1DId,
            startPoint,
            endPoint,
            sectionProfileRotation,
            lengthUnit,
            localElementDisplacements,
            out _,
            out _
        );

    public static DeflectionDiagrams Create(
        Element1dId element1DId,
        Point startPoint,
        Point endPoint,
        Angle sectionProfileRotation,
        LengthUnit lengthUnit,
        Span<double> localElementDisplacements,
        out double displacementMin,
        out double displacementMax
    )
    {
        int numIntervals = 10;
        double[] offsets = new double[numIntervals * 3];

        double beamLength = Line.GetLength(startPoint, endPoint).As(lengthUnit);
        DenseColumnMajorStorage<double> rotationMatrixTranspose = DenseMatrix
            .OfArray(Element1d.GetRotationMatrix(endPoint, startPoint, sectionProfileRotation))
            .Transpose();

        displacementMin = double.MaxValue;
        displacementMax = double.MinValue;
        for (int j = 0; j < numIntervals; j++)
        {
            double step = (double)j / (numIntervals - 1);

            //var displacements = DeflectedShapeShapeFunctionCalculator.Solve(
            //    step * beamLength,
            //    beamLength,
            //    localElementDisplacements,
            //    rotationMatrixTranspose
            //);
            //displacements.AsSpan().CopyTo(offsets.AsSpan(j * 3));

            var displacements = offsets.AsSpan(j * 3, 3);

            DeflectedShapeShapeFunctionCalculator.Solve(
                step * beamLength,
                beamLength,
                localElementDisplacements,
                rotationMatrixTranspose,
                displacements
            );

            var displacement = Math.Sqrt(
                Math.Pow(displacements[0], 2)
                    + Math.Pow(displacements[1], 2)
                    + Math.Pow(displacements[2], 2)
            );

            displacementMin = Math.Min(displacementMin, displacement);
            displacementMax = Math.Max(displacementMax, displacement);
        }

        return new DeflectionDiagrams()
        {
            Element1dId = element1DId,
            NumSteps = numIntervals,
            Offsets = offsets,
        };
    }
}
