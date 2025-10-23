using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using EntityFramework.Exceptions.Common;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

internal class CreateTempModelCommandHandler(
    IModelRepository modelRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<CreateModelRequest, ModelResponse>
{
    public async Task<Result<ModelResponse>> ExecuteAsync(
        CreateModelRequest command,
        CancellationToken ct = default
    )
    {
        Model model = command.ToDomainObject();
        modelRepository.AddTempModel(model);
        try
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (UniqueConstraintException)
        {
            return BeamOsError.Conflict(description: $"Model with ID {model.Id} already exists.");
        }

        return ModelToResponseMapper.Create(model.Settings.UnitSettings).Map(model);
    }
}
