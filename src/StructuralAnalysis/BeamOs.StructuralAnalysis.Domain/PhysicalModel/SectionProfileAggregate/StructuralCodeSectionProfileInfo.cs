using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

public class StructuralCodeSectionProfileInfo : SectionProfileInfoBase
{
    public StructuralCodeSectionProfileInfo(
        ModelId modelId,
        string name,
        StructuralCode structuralCode,
        SectionProfileId? id = null
    )
        : base(modelId, name, id)
    {
        this.StructuralCode = structuralCode;
    }

    public StructuralCode StructuralCode { get; set; }

    public override SectionProfile GetSectionProfile()
    {
        return this.StructuralCode switch
        {
            // todo: add logic for different beam shapes
            StructuralCode.AISC_360_16 => SectionProfile.FromStructuralShapeData(
                this.ModelId,
                StructuralShapes.Lib.AISC.v16_0.WShapes.GetShapeByName(this.Name)
            ),
            _ => throw new NotImplementedException(
                $"Structural code {this.StructuralCode} not implemented."
            ),
        };
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private StructuralCodeSectionProfileInfo()
        : base() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public enum StructuralCode
{
    Undefined = 0,
    AISC_360_16,
}
