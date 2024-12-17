using System.ComponentModel.DataAnnotations.Schema;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Domain.Common.Models;

[PrimaryKey(nameof(Id), nameof(ModelId))]
public class BeamOsModelEntity<TId> : BeamOSEntity<TId>
    where TId : struct
{
    public BeamOsModelEntity(TId? id, ModelId modelId)
    {
        this.Id = id;
        this.ModelId = modelId;
    }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public override TId? Id
    {
        get => base.Id;
        protected set => base.Id = value;
    }
    public ModelId ModelId { get; protected set; }

    [Obsolete("EF Ctor")]
    public BeamOsModelEntity() { }
}
