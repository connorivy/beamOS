using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;

internal readonly record struct EnvelopeResultSetId : IIntBasedId
{
    public int Id { get; init; }

    public EnvelopeResultSetId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(EnvelopeResultSetId id) => id.Id;

    public static implicit operator EnvelopeResultSetId(int id) => new(id);
}
