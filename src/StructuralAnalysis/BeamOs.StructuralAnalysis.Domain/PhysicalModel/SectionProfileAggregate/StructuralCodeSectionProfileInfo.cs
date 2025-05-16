using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

public class SectionProfileFromLibrary : SectionProfileInfoBase
{
    public SectionProfileFromLibrary(
        ModelId modelId,
        string name,
        StructuralCode library,
        SectionProfileId? id = null
    )
        : base(modelId, name, id)
    {
        this.Name = name;
        this.Library = library;
    }

    public StructuralCode Library { get; set; }

    public override SectionProfile GetSectionProfile()
    {
        return this.Library switch
        {
            // todo: add logic for different beam shapes
            StructuralCode.AISC_360_16 => SectionProfile.FromStructuralShapeData(
                this.ModelId,
                StructuralShapes.Lib.AISC.v16_0.WShapes.GetShapeByName(this.Name),
                this.Id
            ),
            _ => throw new NotImplementedException(
                $"Structural code {this.Library} not implemented."
            ),
        };
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private SectionProfileFromLibrary()
        : base() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
