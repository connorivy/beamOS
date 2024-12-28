using BeamOs.Common.Domain.Models;
using MediatR;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public record struct ModelCreatedEvent(Guid ModelId) : IDomainEvent, INotification;
