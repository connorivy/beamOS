using BeamOs.Application.Common.Interfaces;
using BeamOs.Contracts.Common;

namespace BeamOs.Application.AnalyticalModel.ModelResults;

public class DeleteAnalyticalModelByModelIdCommandHandler(
    IModelResultRepository modelResultRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<ModelIdRequest, bool>
{
    public async Task<bool> ExecuteAsync(ModelIdRequest command, CancellationToken ct = default)
    {
        // todo: read before write can violate data integrity
        var analyticalModel = await modelResultRepository.GetByModelId(
            new(Guid.Parse(command.ModelId))
        );

        if (analyticalModel is null)
        {
            return true;
        }

        modelResultRepository.Remove(analyticalModel);
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }
}
