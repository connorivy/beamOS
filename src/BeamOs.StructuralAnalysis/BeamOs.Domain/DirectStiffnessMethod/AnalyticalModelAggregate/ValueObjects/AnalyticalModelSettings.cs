using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate.ValueObjects;

public class AnalyticalModelSettings : BeamOSValueObject
{
    public UnitSettings UnitSettings { get; private set; }

    public AnalyticalModelSettings(UnitSettings unitSettings)
    {
        this.UnitSettings = unitSettings;
    }

    //public ModelOrientation ModelOrientation { get; private set; }
    //public bool IsLocked { get; private set; }
    //public Length Tolerance { get; private set; }
    //public Length MinTreeNodeLength { get; private set; }
    //public int ElementsPerTreeNode { get; private set; }
    //public AnalyticalModelSettings(
    //    UnitSettings unitSettings,
    //    ModelOrientation modelOrientation,
    //    Length? tolerance = null,
    //    Length? minTreeNodeLength = null,
    //    int? elementsPerTreeNode = null
    //)
    //{
    //    // required params
    //    this.UnitSettings = unitSettings;
    //    this.ModelOrientation = modelOrientation;

    //    // optional params
    //    this.Tolerance = tolerance ?? new Length(2, LengthUnit.Inch);
    //    this.MinTreeNodeLength = minTreeNodeLength ?? new Length(24, LengthUnit.Inch);
    //    this.ElementsPerTreeNode = elementsPerTreeNode ?? 10;
    //}
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.UnitSettings;
        //yield return this.ModelOrientation;
        //yield return this.IsLocked;
        //yield return this.Tolerance;
        //yield return this.MinTreeNodeLength;
        //yield return this.ElementsPerTreeNode;
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private AnalyticalModelSettings() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
