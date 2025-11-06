using BeamOs.Common.Domain.Models;
using MediatR;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public record struct ModelCreatedEvent(Guid ModelId, int? UserId = null)
    : IDomainEvent,
        INotification;

public record struct TempModelCreatedEvent(Guid ModelId) : IDomainEvent, INotification;

internal record struct BimFirstSourceModelUpdatedEvent(
    ModelId SourceModelId,
    IList<ModelId> SubscribedModelIds,
    DateTimeOffset UpdateTime,
    DateTimeOffset PreviousUpdateTime
) : IDomainEvent, INotification;
