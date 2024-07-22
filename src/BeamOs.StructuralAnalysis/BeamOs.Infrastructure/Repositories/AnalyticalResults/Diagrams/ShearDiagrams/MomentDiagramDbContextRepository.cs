using BeamOs.Application.AnalyticalResults.Diagrams.ShearDiagrams.Interfaces;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate.ValueObjects;

namespace BeamOs.Infrastructure.Repositories.AnalyticalResults.Diagrams.ShearDiagrams;

internal class MomentDiagramDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<MomentDiagramId, MomentDiagram>(dbContext),
        IMomentDiagramRepository { }
