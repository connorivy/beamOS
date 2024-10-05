using BeamOs.Common.Events;

namespace BeamOs.Domain.PhysicalModel.ModelAggregate.Events;

public record struct ModelDeletedEvent(Guid Id) : IDomainEvent;
