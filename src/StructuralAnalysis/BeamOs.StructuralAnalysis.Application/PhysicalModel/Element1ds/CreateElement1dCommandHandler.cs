using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

public class CreateElement1dCommandHandler(
    IElement1dRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<CreateElement1dCommand, Element1dResponse>
{
    public async Task<Result<Element1dResponse>> ExecuteAsync(
        CreateElement1dCommand command,
        CancellationToken ct = default
    )
    {
        Element1d element1d = command.ToDomainObject();
        element1dRepository.Add(element1d);
        await unitOfWork.SaveChangesAsync(ct);

        return element1d.ToResponse();
    }
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class CreateElement1dCommandMapper
{
    public static partial Element1d ToDomainObject(this CreateElement1dCommand command);

    public static partial Element1dResponse ToResponse(this Element1d entity);
}

public readonly struct CreateElement1dCommand : IModelResourceRequest<CreateElement1dRequest>
{
    public Guid ModelId { get; init; }
    public CreateElement1dRequest Body { get; init; }
    public int StartNodeId => this.Body.StartNodeId;
    public int EndNodeId => this.Body.EndNodeId;
    public int MaterialId => this.Body.MaterialId;
    public int SectionProfileId => this.Body.SectionProfileId;
    public AngleContract? SectionProfileRotation => this.Body.SectionProfileRotation;
    public int? Id => this.Body.Id;
    public Dictionary<string, string>? Metadata => this.Body.Metadata;
}
