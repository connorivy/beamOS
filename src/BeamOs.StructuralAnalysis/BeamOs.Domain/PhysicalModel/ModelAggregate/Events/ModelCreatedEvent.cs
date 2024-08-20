using BeamOs.Common.Events;

namespace BeamOs.Domain.PhysicalModel.ModelAggregate.Events;

public record struct ModelCreatedEvent(Guid Id) : IDomainEvent;
