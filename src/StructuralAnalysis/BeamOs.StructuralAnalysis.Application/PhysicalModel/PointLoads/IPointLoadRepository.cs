using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;

public interface IPointLoadRepository : IModelResourceRepository<PointLoadId, PointLoad> { }
