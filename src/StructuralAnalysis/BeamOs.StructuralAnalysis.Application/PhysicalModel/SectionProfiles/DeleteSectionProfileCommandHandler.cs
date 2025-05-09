using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;

public class DeleteSectionProfileCommandHandler(
    ISectionProfileRepository repo,
    IStructuralAnalysisUnitOfWork unitOfWork
) : DeleteModelEntityCommandHandler<SectionProfileId, SectionProfile>(repo, unitOfWork) { }
