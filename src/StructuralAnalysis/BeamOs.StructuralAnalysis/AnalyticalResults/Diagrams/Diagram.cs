using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.Extensions;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.ValueObjects.DiagramBuilder;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams;

public class Diagram
{
    private static readonly Length EqualityTolerance = new(1, LengthUnit.Inch);
    public Length ElementLength { get; private set; }
    public LengthUnit LengthUnit { get; private set; }
    public List<DiagramConsistentInterval> Intervals { get; private set; }

    public Diagram(
        Length elementLength,
        LengthUnit lengthUnit,
        List<DiagramConsistentInterval> intervals
    )
    {
        this.ElementLength = elementLength;
        this.Intervals = intervals;
        this.LengthUnit = lengthUnit;
    }

    public DiagramBuilder Integrate()
    {
        return new DiagramBuilder(
            this.ElementLength,
            EqualityTolerance,
            this.LengthUnit,
            this.Intervals
        );
    }

    public DiagramValueAtLocation GetValueAtLocation(Length location)
    {
        var (left, right) = this.Intervals.GetValueAtLocation(location, EqualityTolerance, out _);

        //bool isDiscontinuous = false;
        //if (isBetweenConsistantIntervals)
        //{
        //    isDiscontinuous = Math.Abs(left - right) < E
        //}
        return new DiagramValueAtLocation(left, right);
    }
}
