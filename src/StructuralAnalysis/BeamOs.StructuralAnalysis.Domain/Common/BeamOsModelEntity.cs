using System.ComponentModel.DataAnnotations.Schema;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Domain.Common;

[PrimaryKey(nameof(Id), nameof(ModelId))]
public class BeamOsModelEntity<TId> : BeamOsEntity<TId>
    where TId : struct
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
    public ModelId ModelId { get; protected set; }

    [Obsolete("EF Ctor")]
    public BeamOsModelEntity() { }
}
