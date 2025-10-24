using System.Numerics;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;

internal class CreatePointLoadCommandHandler(
    IPointLoadRepository pointLoadRepository,
    ILoadCaseRepository loadCaseRepository,
    INodeRepository nodeRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<CreatePointLoadRequest>, PointLoadResponse>
{
    public async Task<Result<PointLoadResponse>> ExecuteAsync(
        ModelResourceRequest<CreatePointLoadRequest> command,
        CancellationToken ct = default
    )
    {
        // var x = await loadCaseRepository.GetMany(command.ModelId, null, ct);
        // foreach (var loadCase in x)
        // {
        //     Console.WriteLine(loadCase.Id);
        //     Console.WriteLine(loadCase.Name);
        // }
        var loadCase = await loadCaseRepository.GetSingle(
            command.ModelId,
            command.Body.LoadCaseId,
            ct
        );
        Console.WriteLine($"Load Case ID: {loadCase?.Id}, Name: {loadCase?.Name}");
        var node = await nodeRepository.GetSingle(command.ModelId, command.Body.NodeId, ct);
        Console.WriteLine($"Node ID: {node?.Id}, Position: {node?.LocationPoint}");
        PointLoad pointLoad = command.ToDomainObject();
        pointLoadRepository.Add(pointLoad);
        await unitOfWork.SaveChangesAsync(ct);

        return pointLoad.ToResponse();
    }
}

[Mapper(PreferParameterlessConstructors = false)]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
internal static partial class CreatePointLoadCommandMapper
{
    [MapNestedProperties(nameof(ModelResourceRequest<>.Body))]
    public static partial PointLoad ToDomainObject(
        this ModelResourceRequest<CreatePointLoadRequest> command
    );

    public static partial PointLoadResponse ToResponse(this PointLoad entity);
}
