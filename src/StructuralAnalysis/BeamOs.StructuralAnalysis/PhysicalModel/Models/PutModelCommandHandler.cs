using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

internal class PutModelCommandHandler(
    IModelRepository modelRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<ModelInfoData>, ModelResponse>
{
    public async Task<Result<ModelResponse>> ExecuteAsync(
        ModelResourceRequest<ModelInfoData> command,
        CancellationToken ct = default
    )
    {
        Model model = command.ToDomainObject();
        await modelRepository.Put(model);
        await unitOfWork.SaveChangesAsync(ct);

        return ModelToResponseMapper.Create(model.Settings.UnitSettings).Map(model);
    }
}
