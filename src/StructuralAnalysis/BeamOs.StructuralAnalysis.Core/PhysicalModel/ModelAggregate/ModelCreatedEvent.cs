using BeamOs.Common.Domain.Models;
using MediatR;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

internal record struct ModelCreatedEvent(Guid ModelId, int? UserId = null)
    : IDomainEvent,
        INotification;
