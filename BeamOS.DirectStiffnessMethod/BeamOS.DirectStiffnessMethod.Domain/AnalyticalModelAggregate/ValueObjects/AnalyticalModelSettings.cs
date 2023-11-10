using BeamOS.Common.Domain.Models;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.Enums;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;
public class AnalyticalModelSettings : BeamOSValueObject
{
    public UnitSettings UnitSettings { get; }
    public ModelOrientation ModelOrientation { get; }
    public bool IsLocked { get; }
    public Length Tolerance { get; }
    public Length MinTreeNodeLength { get; }
    public int ElementsPerTreeNode { get; }
    public AnalyticalModelSettings(
        UnitSettings unitSettings,
        ModelOrientation modelOrientation,
        Length? tolerance = null,
        Length? minTreeNodeLength = null,
        int? elementsPerTreeNode = null
    )
    {
        // required params
        this.UnitSettings = unitSettings;
        this.ModelOrientation = modelOrientation;

        // optional params
        this.Tolerance = tolerance ?? new Length(2, LengthUnit.Inch);
        this.MinTreeNodeLength = minTreeNodeLength ?? new Length(24, LengthUnit.Inch);
        this.ElementsPerTreeNode = elementsPerTreeNode ?? 10;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.UnitSettings;
        yield return this.ModelOrientation;
        yield return this.IsLocked;
        yield return this.Tolerance;
        yield return this.MinTreeNodeLength;
        yield return this.ElementsPerTreeNode;
    }
}
