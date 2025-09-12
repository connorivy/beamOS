using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

internal abstract class SectionProfileInfoBase : BeamOsModelEntity<SectionProfileId>
{
    public const string TypeDiscriminator = "SectionProfileInfoType";

    public SectionProfileInfoBase(ModelId modelId, string name, SectionProfileId? id = null)
        : base(id ?? new(), modelId)
    {
        this.Name = name;
    }

    public string Name { get; set; }
    public abstract BeamOsObjectType SectionProfileType { get; protected set; }

    public abstract SectionProfile GetSectionProfile();

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected SectionProfileInfoBase()
        : base() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
