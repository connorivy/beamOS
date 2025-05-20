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
) : ICommandHandler<ModelResourceRequest<ModelProposalData>, ModelProposalResponse>
{
    public async Task<Result<ModelProposalResponse>> ExecuteAsync(
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
        modelProposal.ProposalIssues = [];
        modelProposalRepository.Add(modelProposal);

        var existingNodesToBeModified = await GetExistingEntities(
            nodeRepository,
            command.ModelId,
            command.Body.ModifyNodeProposals?.Select(n => new NodeId(n.Id)).ToList(),
            ct
        );
        var existingElementsToBeModified = await GetExistingEntities(
            element1dRepository,
            command.ModelId,
            command
                .Body.ModifyElement1dProposals?.OfType<ModifyElement1dProposal>()
                .Select(e => new Element1dId(e.ExistingElement1dId))
                .ToList(),
            ct
        );

        foreach (var node in command.Body.CreateNodeProposals ?? [])
        {
            var nodeProposal = node.ToProposalDomain(command.ModelId, modelProposal.Id);
            modelProposal.NodeProposals.Add(nodeProposal);
        }
        foreach (var node in command.Body.ModifyNodeProposals ?? [])
        {
            var existingNode = existingNodesToBeModified[node.Id];
            var nodeProposal = node.ToProposalDomain(existingNode, modelProposal.Id);
            modelProposal.NodeProposals.Add(nodeProposal);
        }
        foreach (var element1d in command.Body.CreateElement1dProposals ?? [])
        {
            var element1dProposal = element1d.ToProposalDomain(command.ModelId, modelProposal.Id);
            modelProposal.Element1dProposals.Add(element1dProposal);
        }
        foreach (var element1d in command.Body.ModifyElement1dProposals ?? [])
        {
            var existingElement = existingElementsToBeModified[element1d.ExistingElement1dId];
            var element1dProposal = element1d.ToProposalDomain(existingElement, modelProposal.Id);
            modelProposal.Element1dProposals.Add(element1dProposal);
        }
        foreach (var proposalIssueContract in command.Body.ProposalIssues ?? [])
        {
            var proposalIssue = proposalIssueContract.ToDomain(command.ModelId, modelProposal.Id);
            modelProposal.ProposalIssues.Add(proposalIssue);
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

    public static partial ModelProposalInfo ToInfoContract(this ModelProposal modelProposal);

    public static partial NodeProposal ToProposalDomain(
        this CreateNodeRequest command,
        ModelId modelId,
        ModelProposalId modelProposalId
    );

    public static partial NodeProposal ToProposalDomain(
        this PutNodeRequest command,
        Node existingNode,
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

    public static partial CreateNodeProposalResponse ToCreateProposalContract(
        this NodeProposal command
    );

    [MapProperty(
        nameof(NodeProposal.ExistingId),
        nameof(ModifyNodeProposalResponse.ExistingNodeId)
    )]
    public static partial ModifyNodeProposalResponse ToModifyProposalContract(
        this NodeProposal command
    );

    public static partial CreateElement1dProposalResponse ToCreateProposalContract(
        this Element1dProposal command
    );

    [MapProperty(
        nameof(Element1dProposal.ExistingId),
        nameof(ModifyElement1dProposal.ExistingElement1dId)
    )]
    public static partial ModifyElement1dProposalResponse ToModifyProposalContract(
        this Element1dProposal command
    );

    public static ProposalIssueContract ToProposalContract(this ProposalIssue command)
    {
        ProposedID proposedId;
        if (command.ProposedId is not null)
        {
            proposedId = ProposedID.Proposed(command.ProposedId.Value);
        }
        else if (command.ExistingId is not null)
        {
            proposedId = ProposedID.Existing(command.ExistingId.Value);
        }
        else
        {
            throw new ArgumentNullException(
                nameof(command),
                "Either ExistingId or ProposedId must be provided."
            );
        }
        return new ProposalIssueContract
        {
            Id = command.Id,
            ProposedId = proposedId,
            ObjectType = command.ObjectType,
            Message = command.Message,
            Severity = command.Severity,
        };
    }

    public static partial ProposalIssue ToDomain(
        this ProposalIssueData command,
        ModelId modelId,
        ModelProposalId modelProposalId
    );

    public static ModelProposalResponse ToContract(this ModelProposal modelProposal)
    {
        var response = new ModelProposalResponse
        {
            Id = modelProposal.Id,
            LastModified = modelProposal.LastModified,
            ModelProposal = modelProposal.ToInfoContract(),
            CreateNodeProposals = [],
            ModifyNodeProposals = [],
            CreateElement1dProposals = [],
            ModifyElement1dProposals = [],
            ProposalIssues = [],
        };
        // var response = modelProposal.ToContract();
        // foreach (var nodeProposal in modelProposal.NodeProposals)
        // {
        //     if (nodeProposal.IsExisting)
        //     {
        //         (response.ModifyNodeProposals ?? []).Add(nodeProposal());
        //     }
        //     else
        //     {
        //         (response.CreateNodeProposals ?? []).Add(nodeProposal.ToInfoContract());
        //     }
        // }
        foreach (var nodeProposal in modelProposal.NodeProposals ?? [])
        {
            if (nodeProposal.IsExisting)
            {
                response.ModifyNodeProposals.Add(nodeProposal.ToModifyProposalContract());
            }
            else
            {
                response.CreateNodeProposals.Add(nodeProposal.ToCreateProposalContract());
            }
        }
        foreach (var element1dProposal in modelProposal.Element1dProposals ?? [])
        {
            if (element1dProposal.IsExisting)
            {
                response.ModifyElement1dProposals.Add(element1dProposal.ToModifyProposalContract());
            }
            else
            {
                response.CreateElement1dProposals.Add(element1dProposal.ToCreateProposalContract());
            }
        }
        foreach (var proposalIssue in modelProposal.ProposalIssues ?? [])
        {
            response.ProposalIssues.Add(proposalIssue.ToProposalContract());
        }
        return response;
    }
}
