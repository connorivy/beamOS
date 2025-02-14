using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;

public interface IMomentLoadRepository : IModelResourceRepository<MomentLoadId, MomentLoad> { }
