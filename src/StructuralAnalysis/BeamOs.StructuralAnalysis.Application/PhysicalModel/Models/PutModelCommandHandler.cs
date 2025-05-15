using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public class PutModelCommandHandler(
    IModelRepository modelRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<ModelData>, ModelResponse>
{
    public async Task<Result<ModelResponse>> ExecuteAsync(
        ModelResourceRequest<ModelData> command,
        CancellationToken ct = default
    )
    {
        Model model = command.ToDomainObject();
        modelRepository.Put(model);
        await unitOfWork.SaveChangesAsync(ct);

        return ModelToResponseMapper.Create(model.Settings.UnitSettings).Map(model);
    }
}
