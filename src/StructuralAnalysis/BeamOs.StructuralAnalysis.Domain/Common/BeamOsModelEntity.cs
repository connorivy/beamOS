using System.ComponentModel.DataAnnotations.Schema;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Domain.Common;

[PrimaryKey(nameof(Id), nameof(ModelId))]
public class BeamOsModelEntity<TId> : BeamOsEntity<TId>, IBeamOsModelEntity
    where TId : struct, IIntBasedId
{
    public BeamOsModelEntity(TId id, ModelId modelId)
        : base(id)
    {
        this.ModelId = modelId;
    }

    public virtual ModelId ModelId { get; protected set; }
    public Model? Model { get; private set; }

    [Obsolete("EF Ctor")]
    public BeamOsModelEntity() { }

    public int GetIntId() => this.Id.Id;

    public void SetIntId(int value) => this.Id = new TId() { Id = value };
}

public interface IBeamOsModelEntity
{
    public ModelId ModelId { get; }
    public Model? Model { get; }
    public int GetIntId();
    public void SetIntId(int value);
}

[PrimaryKey(nameof(Id), nameof(ResultSetId), nameof(ModelId))]
public class BeamOsAnalyticalResultEntity<TId> : BeamOsEntity<TId>
    where TId : struct
{
    public BeamOsAnalyticalResultEntity(TId id, ResultSetId resultSetId, ModelId modelId)
        : base(id)
    {
        this.ModelId = modelId;
        this.ResultSetId = resultSetId;
    }

    public ModelId ModelId { get; protected set; }
    public Model? Model { get; private set; }

    public ResultSetId ResultSetId { get; protected set; }
    public ResultSet? ResultSet { get; private set; }

    [Obsolete("EF Ctor")]
    public BeamOsAnalyticalResultEntity() { }
}

[PrimaryKey(nameof(Id), nameof(ModelProposalId), nameof(ModelId))]
public abstract class BeamOsModelProposalEntity<TId, TModelEntityId> : BeamOsEntity<TId>
    where TId : struct
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

    [Obsolete("EF Ctor", true)]
    protected BeamOsModelProposalEntity() { }
}
