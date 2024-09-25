using BeamOs.Application.AnalyticalModel.Diagrams.ShearDiagrams.Interfaces;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;

namespace BeamOs.Infrastructure.Repositories.AnalyticalModel.Diagrams.ShearDiagrams;

internal class ShearForceDiagramDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<ShearForceDiagramId, ShearForceDiagram>(dbContext),
        IShearDiagramRepository { }
