using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;

public class Material : BeamOsModelEntity<MaterialId>
{
    public Material(
        ModelId modelId,
        Pressure modulusOfElasticity,
        Pressure modulusOfRigidity,
        MaterialId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.ModelId = modelId;
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
    }

    public Pressure ModulusOfElasticity { get; private set; }
    public Pressure ModulusOfRigidity { get; private set; }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Material() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public sealed class MaterialProposal : BeamOsModelProposalEntity<MaterialProposalId, MaterialId>
{
    public MaterialProposal(
        ModelId modelId,
        ModelProposalId modelProposalId,
        Pressure modulusOfElasticity,
        Pressure modulusOfRigidity,
        MaterialProposalId? id = null
    )
        : base(id ?? new(), modelProposalId, modelId)
    {
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
    }

    public MaterialProposal(
        Material material,
        ModelProposalId modelProposalId,
        Pressure? modulusOfElasticity = null,
        Pressure? modulusOfRigidity = null,
        MaterialProposalId? id = null
    )
        : this(
            modelId: material.ModelId,
            modelProposalId: modelProposalId,
            modulusOfElasticity: modulusOfElasticity ?? material.ModulusOfElasticity,
            modulusOfRigidity: modulusOfRigidity ?? material.ModulusOfRigidity,
            id: id
        ) { }

    public Pressure ModulusOfElasticity { get; private set; }
    public Pressure ModulusOfRigidity { get; private set; }

    public Material ToDomain()
    {
        return new Material(this.ModelId, this.ModulusOfElasticity, this.ModulusOfRigidity);
    }

    [Obsolete("EF Core Constructor", true)]
    private MaterialProposal() { }
}
