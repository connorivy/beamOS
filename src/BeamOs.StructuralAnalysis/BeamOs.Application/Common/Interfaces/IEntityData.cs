using BeamOs.Domain.Common.Interfaces;

namespace BeamOs.Application.Common.Interfaces;

public interface IEntityData : IBeamOsDomainObject
{
    public Guid Id { get; }
}
