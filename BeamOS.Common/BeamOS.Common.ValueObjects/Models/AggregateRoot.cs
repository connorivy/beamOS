namespace BeamOS.Common.Domain.Models;
public abstract class AggregateRoot<TId> : BeamOSEntity<TId>
    where TId : notnull
{
    protected AggregateRoot(TId id) : base(id)
    {
    }
}
