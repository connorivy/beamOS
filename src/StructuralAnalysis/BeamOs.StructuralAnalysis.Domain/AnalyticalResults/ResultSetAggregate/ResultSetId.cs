using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;

public readonly record struct ResultSetId : IIntBasedId
{
    public int Id { get; init; }

    public ResultSetId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(ResultSetId id) => id.Id;

    public static implicit operator ResultSetId(int id) => new(id);
}
