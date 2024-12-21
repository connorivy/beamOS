using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public record struct ModelCreatedEvent(Guid Id) : IDomainEvent;
