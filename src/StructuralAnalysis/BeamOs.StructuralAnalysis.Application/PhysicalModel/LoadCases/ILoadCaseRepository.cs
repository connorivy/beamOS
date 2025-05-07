using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;

public interface ILoadCaseRepository : IModelResourceRepository<LoadCaseId, LoadCase>
{
}
