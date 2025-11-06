using BeamOs.Common.Contracts;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Domain.Common;

[PrimaryKey(nameof(Id), nameof(ModelId))]
internal abstract class BeamOsModelEntity<TId> : BeamOsEntity<TId>, IBeamOsModelEntity
    where TId : struct, IIntBasedId
{
    public BeamOsModelEntity(TId id, ModelId modelId)
        : base(id)
    {
        this.ModelId = modelId;
    }

    public virtual ModelId ModelId { get; protected set; }
    public Model? Model { get; set; }

    [Obsolete("EF Ctor")]
    public BeamOsModelEntity() { }

    public int GetIntId() => this.Id.Id;

    public void SetIntId(int value) => this.Id = new TId() { Id = value };
}

internal interface IHasModelIdDomain
{
    public ModelId ModelId { get; }
    public Model? Model { get; }
}

internal interface IHasIntIdDomain
{
    public int GetIntId();
    public void SetIntId(int value);
}

internal interface IBeamOsModelEntity : IHasModelIdDomain, IHasIntIdDomain { }

internal interface IBeamOsModelEntity<TId, in T> : IBeamOsModelEntity
    where TId : struct, IIntBasedId
    where T : IBeamOsModelEntity<TId, T>
{
    public bool MemberwiseEquals(T other);
}

internal interface IBeamOsModelProposalEntity : IHasModelIdDomain, IHasIntIdDomain
{
    public ModelProposalId ModelProposalId { get; }
}

[PrimaryKey(nameof(Id), nameof(ResultSetId), nameof(ModelId))]
internal class BeamOsAnalyticalResultEntity<TId> : BeamOsModelEntity<TId>
    where TId : struct, IIntBasedId
{
    public BeamOsAnalyticalResultEntity(TId id, ResultSetId resultSetId, ModelId modelId)
        : base(id, modelId)
    {
        this.ModelId = modelId;
        this.ResultSetId = resultSetId;
    }

    public ResultSetId ResultSetId { get; protected set; }
    public ResultSet? ResultSet { get; private set; }

    [Obsolete("EF Ctor")]
    public BeamOsAnalyticalResultEntity() { }
}

[PrimaryKey(nameof(Id), nameof(ModelProposalId), nameof(ModelId))]
internal abstract class BeamOsModelProposalEntity<TId, TModelEntityId>
    : BeamOsEntity<TId>,
        IBeamOsModelProposalEntity
    where TId : struct, IIntBasedId
    where TModelEntityId : struct
{
    public BeamOsModelProposalEntity(
        TId id,
        ModelProposalId modelProposalId,
        ModelId modelId,
        TModelEntityId? existingId = null
    )
        : base(id)
    {
        this.ModelId = modelId;
        this.ModelProposalId = modelProposalId;
        this.ExistingId = existingId;
    }

    public ModelId ModelId { get; protected set; }
    public Model? Model { get; private set; }

    public ModelProposalId ModelProposalId { get; protected set; }
    public ModelProposal? ModelProposal { get; set; }

    public TModelEntityId? ExistingId { get; set; }
    public bool IsExisting => this.ExistingId != null;

    public int GetIntId() => this.Id.Id;

    public void SetIntId(int value) => this.Id = new TId() { Id = value };

    [Obsolete("EF Ctor", true)]
    protected BeamOsModelProposalEntity() { }
}
