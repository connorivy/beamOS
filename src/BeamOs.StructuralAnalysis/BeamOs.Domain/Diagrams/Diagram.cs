using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.Common.Extensions;
using BeamOs.Domain.Diagrams.Common.ValueObjects;
using BeamOs.Domain.Diagrams.Common.ValueObjects.DiagramBuilder;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.Diagrams;

public class Diagram
{
    private static readonly Length EqualityTolerance = new(1, LengthUnit.Inch);
    public Length ElementLength { get; private set; }
    public LengthUnit LengthUnit { get; private set; }
    public List<DiagramConsistantInterval> Intervals { get; private set; }

    public Diagram(
        Length elementLength,
        LengthUnit lengthUnit,
        List<DiagramConsistantInterval> intervals
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
