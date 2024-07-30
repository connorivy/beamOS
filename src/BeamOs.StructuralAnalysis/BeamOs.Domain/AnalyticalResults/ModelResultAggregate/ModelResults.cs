using BeamOs.Domain.AnalyticalResults.AnalyticalModelAggregate.ValueObjects;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using UnitsNet;

namespace BeamOs.Domain.AnalyticalResults.ModelResultAggregate;

public class ModelResult : AggregateRoot<ModelResultId>
{
    public ModelResult(
        ModelId modelId,
        Force maxShear,
        Force minShear,
        Torque maxMoment,
        Torque minMoment,
        ModelResultId? id = null
    )
        : base(id ?? new())
    {
        this.ModelId = modelId;
        this.MaxShear = maxShear;
        this.MinShear = minShear;
        this.MaxMoment = maxMoment;
        this.MinMoment = minMoment;
    }

    public ModelId ModelId { get; private set; }
    public Force MaxShear { get; private set; }
    public Force MinShear { get; private set; }
    public Torque MaxMoment { get; private set; }
    public Torque MinMoment { get; private set; }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ModelResult() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
