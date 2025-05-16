using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;

public class PutElement1dCommandHandler(
    IElement1dRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<PutElement1dCommand, Element1dResponse>
{
    public async Task<Result<Element1dResponse>> ExecuteAsync(
        PutElement1dCommand command,
        CancellationToken ct = default
    )
    {
        Element1d element1d = command.ToDomainObject();
        element1dRepository.Put(element1d);
        await unitOfWork.SaveChangesAsync(ct);

        return element1d.ToResponse();
    }
}

public class BatchPutElement1dCommandHandler(
    IElement1dRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<Element1dId, Element1d, BatchPutElement1dCommand, PutElement1dRequest>(
        element1dRepository,
        unitOfWork
    )
{
    protected override Element1d ToDomainObject(ModelId modelId, PutElement1dRequest putRequest) =>
        new PutElement1dCommand(modelId, putRequest).ToDomainObject();
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class PutElement1dCommandMapper
{
    public static partial Element1dResponse ToResponse(this PutElement1dCommand command);

    public static partial Element1d ToDomainObject(this PutElement1dCommand command);
}

public readonly struct PutElement1dCommand : IModelResourceWithIntIdRequest<Element1dData>
{
    public int Id { get; init; }
    public Guid ModelId { get; init; }
    public Element1dData Body { get; init; }
    public int StartNodeId => this.Body.StartNodeId;
    public int EndNodeId => this.Body.EndNodeId;
    public int MaterialId => this.Body.MaterialId;
    public int SectionProfileId => this.Body.SectionProfileId;
    public AngleContract? SectionProfileRotation => this.Body.SectionProfileRotation;
    public Dictionary<string, string>? Metadata => this.Body.Metadata;

    public PutElement1dCommand() { }

    public PutElement1dCommand(ModelId modelId, PutElement1dRequest putElement1DRequest)
    {
        this.Id = putElement1DRequest.Id;
        this.ModelId = modelId;
        this.Body = putElement1DRequest;
    }
}

public readonly struct BatchPutElement1dCommand : IModelResourceRequest<PutElement1dRequest[]>
{
    public Guid ModelId { get; init; }
    public PutElement1dRequest[] Body { get; init; }
}
