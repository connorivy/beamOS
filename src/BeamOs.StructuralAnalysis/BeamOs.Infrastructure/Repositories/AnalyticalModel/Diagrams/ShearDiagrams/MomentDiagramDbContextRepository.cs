using BeamOs.Application.AnalyticalModel.Diagrams.ShearDiagrams.Interfaces;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate.ValueObjects;

namespace BeamOs.Infrastructure.Repositories.AnalyticalModel.Diagrams.ShearDiagrams;

internal class MomentDiagramDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<MomentDiagramId, MomentDiagram>(dbContext),
        IMomentDiagramRepository { }
