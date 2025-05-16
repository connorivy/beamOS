using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Riok.Mapperly.Abstractions;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public class CreateProposalCommandHandler(
    IModelRepository modelRepository,
    IModelProposalRepository modelProposalRepository,
    INodeRepository nodeRepository,
    IElement1dRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<ModelResourceRequest<ModelProposalData>, ModelProposalContract>
{
    public async Task<Result<ModelProposalContract>> ExecuteAsync(
        ModelResourceRequest<ModelProposalData> command,
        CancellationToken ct = default
    )
    {
        var model = await modelRepository.GetSingle(command.ModelId, ct);
        if (model is null)
        {
            return BeamOsError.NotFound(description: $"Model with id {command.ModelId} not found");
        }
        ModelProposal modelProposal = command.Body.ToProposalDomain(model);
        modelProposal.NodeProposals = [];
        modelProposal.Element1dProposals = [];
        modelProposalRepository.Add(modelProposal);

        var existingNodesToBeModified = await GetExistingEntities(
            nodeRepository,
            command.ModelId,
            command.Body.NodeProposals?.Select(n => new NodeId(n.Id)).ToList(),
            ct
        );
        var existingElementsToBeModified = await GetExistingEntities(
            element1dRepository,
            command.ModelId,
            command
                .Body.Element1dProposals?.OfType<ModifyElement1dProposal>()
                .Select(e => new Element1dId(e.ExistingElement1dId))
                .ToList(),
            ct
        );

        foreach (var node in command.Body.NodeProposals ?? [])
        {
            var nodeProposal = node.ToProposalDomain(command.ModelId, modelProposal.Id);
            modelProposal.NodeProposals.Add(nodeProposal);
        }
        foreach (var element1d in command.Body.Element1dProposals ?? [])
        {
            if (element1d is CreateElement1dProposal createElement1dProposal)
            {
                modelProposal.Element1dProposals.Add(
                    createElement1dProposal.ToProposalDomain(command.ModelId, modelProposal.Id)
                );
            }
            else if (element1d is ModifyElement1dProposal modifyElement1dProposal)
            {
                var existingElement = existingElementsToBeModified[
                    modifyElement1dProposal.ExistingElement1dId
                ];
                var element1dProposal = modifyElement1dProposal.ToProposalDomain(
                    existingElement,
                    modelProposal.Id
                );
                modelProposal.Element1dProposals.Add(element1dProposal);
            }
            else
            {
                throw new NotSupportedException(
                    $"Element1d proposal type {element1d.GetType()} is not supported"
                );
            }
        }

        await unitOfWork.SaveChangesAsync(ct);

        return modelProposal.ToContract();
    }

    private static async Task<Dictionary<int, TEntity>> GetExistingEntities<TId, TEntity>(
        IModelResourceRepository<TId, TEntity> repository,
        ModelId modelId,
        IList<TId>? ids,
        CancellationToken ct
    )
        where TId : struct, IIntBasedId
        where TEntity : BeamOsModelEntity<TId>
    {
        if (ids is null || ids.Count == 0)
        {
            return [];
        }

        var existingEntities = await repository.GetMany(modelId, ids, ct);
        return existingEntities.ToDictionary(e => e.Id.Id);
    }
}

[Mapper(ThrowOnPropertyMappingNullMismatch = true)]
[UseStaticMapper(typeof(UnitsNetMappers))]
[UseStaticMapper(typeof(BeamOsDomainContractMappers))]
public static partial class ProposalStaticMappers
{
    public static ModelProposal ToProposalDomain(this ModelProposalData command, Model model) =>
        new(model, command.Name, command.Description, command.Settings?.ToDomain());

    public static partial ModelProposalContract ToContract(this ModelProposal modelProposal);

    public static partial NodeProposal ToProposalDomain(
        this PutNodeRequest command,
        ModelId modelId,
        ModelProposalId modelProposalId
    );

    public static partial Element1dProposal ToProposalDomain(
        this CreateElement1dProposal command,
        ModelId modelId,
        ModelProposalId modelProposalId
    );

    public static partial Element1dProposal ToProposalDomain(
        this ModifyElement1dProposal command,
        Element1d element1d,
        ModelProposalId modelProposalId
    );

    public static Element1dProposalContract ToContract(this Element1dProposal element1dProposal)
    {
        if (element1dProposal.IsExisting)
        {
            return Element1dProposalContract.Modify(
                element1dProposal.ExistingId!.Value,
                element1dProposal.StartNodeId.ToProposedIdContract(),
                element1dProposal.EndNodeId.ToProposedIdContract(),
                element1dProposal.MaterialId.ToProposedIdContract(),
                element1dProposal.SectionProfileId.ToProposedIdContract()
            // element1dProposal.SectionProfileRotation.ToContract(),
            // element1dProposal.Metadata
            );
        }
        else
        {
            return Element1dProposalContract.Create(
                element1dProposal.StartNodeId.ToProposedIdContract(),
                element1dProposal.EndNodeId.ToProposedIdContract(),
                element1dProposal.MaterialId.ToProposedIdContract(),
                element1dProposal.SectionProfileId.ToProposedIdContract()
            // element1dProposal.SectionProfileRotation.ToContract(),
            // element1dProposal.Metadata
            );
        }
    }
}
