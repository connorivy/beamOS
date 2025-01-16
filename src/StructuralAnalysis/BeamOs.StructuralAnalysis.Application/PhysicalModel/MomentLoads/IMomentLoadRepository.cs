using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;

public interface IMomentLoadRepository : IRepository<MomentLoadId, MomentLoad> { }
