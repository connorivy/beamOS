using BeamOs.Common.Domain.Models;
using MediatR;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public record struct ModelCreatedEvent(Guid ModelId, int? UserId = null)
    : IDomainEvent,
        INotification;

public record struct TempModelCreatedEvent(Guid ModelId) : IDomainEvent, INotification;

// Note: This event is raised when a BIM source model is updated.
// The SourceModel reference may not have navigation properties loaded when accessed
// in a different DbContext scope, so the handler queries the model directly.
internal record struct BimFirstSourceModelUpdatedEvent(
    ModelId SourceModelId,
    IList<ModelId> SubscribedModelIds,
    DateTimeOffset UpdateTime,
    DateTimeOffset PreviousUpdateTime
) : IDomainEvent, INotification;
