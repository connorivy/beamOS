using BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Interfaces;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;

namespace BeamOs.Infrastructure.Repositories.AnalyticalResults.Diagrams.ShearDiagrams;

internal class ShearForceDiagramDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<ShearForceDiagramId, ShearForceDiagram>(dbContext),
        IShearDiagramRepository { }
