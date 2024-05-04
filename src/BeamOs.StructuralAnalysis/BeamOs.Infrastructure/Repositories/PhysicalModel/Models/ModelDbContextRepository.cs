using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

namespace BeamOs.Infrastructure.Repositories.PhysicalModel.Models;

internal class ModelDbContextRepository(BeamOsStructuralDbContext dbContext)
    : RepositoryBase<ModelId, Model>(dbContext),
        IModelRepository { }
