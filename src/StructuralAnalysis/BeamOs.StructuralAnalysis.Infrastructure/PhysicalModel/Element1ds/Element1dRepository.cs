using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Infrastructure.Common;

namespace BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Element1ds;

internal class Element1dRepository(StructuralAnalysisDbContext dbContext)
    : RepositoryBase<Element1dId, Element1d>(dbContext),
        IElement1dRepository { }
