using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public class CreateModelCommandHandler(
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
        modelRepository.Add(model);
        await unitOfWork.SaveChangesAsync(ct);

        return model.ToResponse();
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class CreateModelCommandMapper
{
    public static partial Model ToDomainObject(this CreateModelRequest command);

    public static partial ModelResponse ToResponse(this Model entity);
}
