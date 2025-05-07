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

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override TId Id
    {
        get => base.Id;
        protected set => base.Id = value;
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

    public override TId Id
    {
        get => base.Id;
        protected set => base.Id = value;
    }

    public ModelId ModelId { get; protected set; }
    public Model? Model { get; private set; }

    public ResultSetId ResultSetId { get; protected set; }
    public ResultSet? ResultSet { get; private set; }

    [Obsolete("EF Ctor")]
    public BeamOsAnalyticalResultEntity() { }
}
