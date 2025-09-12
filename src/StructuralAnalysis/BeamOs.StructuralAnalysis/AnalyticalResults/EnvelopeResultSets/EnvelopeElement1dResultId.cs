using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;

internal readonly record struct EnvelopeElement1dResultId : IIntBasedId
{
    public int Id { get; init; }

    public EnvelopeElement1dResultId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(EnvelopeElement1dResultId id) => id.Id;

    public static implicit operator EnvelopeElement1dResultId(int id) => new(id);
}
