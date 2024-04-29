using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.Models;

public interface IModelRepository : IRepository<ModelId, Model> { }
