using BeamOs.Domain.Common.Interfaces;

namespace BeamOs.Infrastructure.Data.Models;

internal abstract class ReadModelBase : IBeamOsDomainObject
{
    public Guid Id { get; protected set; }

    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }
}
