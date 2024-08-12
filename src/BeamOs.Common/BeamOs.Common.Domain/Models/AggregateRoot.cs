namespace BeamOs.Common.Domain.Models;

public abstract class AggregateRoot<TId> : BeamOSEntity<TId>
    where TId : notnull
{
    protected AggregateRoot(TId id)
        : base(id) { }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Obsolete("EF Core Constructor", true)]
    protected AggregateRoot() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
