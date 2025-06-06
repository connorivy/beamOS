using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

public class AcceptModelProposalCommandHandler(
    IModelProposalRepository modelProposalRepository,
    INodeDefinitionRepository nodeRepository,
    IMaterialRepository materialRepository,
    ISectionProfileRepository sectionProfileRepository,
    ISectionProfileFromLibraryRepository sectionProfileFromLibraryRepository,
    IElement1dRepository element1dRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<IModelEntity, ModelResponse>
{
    public async Task<Result<ModelResponse>> ExecuteAsync(
        IModelEntity command,
        CancellationToken ct = default
    )
    {
        ModelProposal? modelProposal = await modelProposalRepository.GetSingle(
            command.ModelId,
            command.Id,
            ct
        );
        if (modelProposal is null)
        {
            return BeamOsError.NotFound(
                description: $"Model proposal with id {command.Id} for model {command.ModelId} was not found"
            );
        }

        if (modelProposal.ProposalIssues.Any(p => p.Severity == ProposalIssueSeverity.Critical))
        {
            return BeamOsError.InvalidOperation(
                description: "Model proposal cannot be accepted because it has critical issues"
            );
        }

        Dictionary<NodeProposalId, Node> nodeProposalIdToNodeDict = [];
        List<Node> nodes = [];
        foreach (var nodeProposal in modelProposal.NodeProposals)
        {
            var node = nodeProposal.ToDomain();
            nodes.Add(node);
            if (nodeProposal.IsExisting)
            {
                nodeRepository.Put(node);
            }
            else
            {
                nodeRepository.Add(node);
                nodeProposalIdToNodeDict.Add(nodeProposal.Id, node);
            }
        }

        Dictionary<MaterialProposalId, Material> materialProposalIdToMaterialDict = [];
        List<Material> materials = [];
        foreach (var materialProposal in modelProposal.MaterialProposals)
        {
            var material = materialProposal.ToDomain();
            materials.Add(material);
            if (materialProposal.IsExisting)
            {
                materialRepository.Put(material);
            }
            else
            {
                materialRepository.Add(material);
                materialProposalIdToMaterialDict.Add(materialProposal.Id, material);
            }
        }

        Dictionary<
            SectionProfileProposalId,
            SectionProfileInfoBase
        > sectionProfileProposalIdToDomainDict = [];
        List<SectionProfile> sectionProfiles = [];
        List<SectionProfileFromLibrary> sectionProfilesFromLibrary = [];
        foreach (var sectionProfileProposal in modelProposal.SectionProfileProposals)
        {
            var sectionProfile = sectionProfileProposal.ToDomain();
            sectionProfiles.Add(sectionProfile);
            if (sectionProfileProposal.IsExisting)
            {
                sectionProfileRepository.Put(sectionProfile);
            }
            else
            {
                sectionProfileRepository.Add(sectionProfile);
                sectionProfileProposalIdToDomainDict.Add(sectionProfileProposal.Id, sectionProfile);
            }
        }
        foreach (var sectionProfileProposal in modelProposal.SectionProfileProposalsFromLibrary)
        {
            var sectionProfile = sectionProfileProposal.ToDomain();
            sectionProfilesFromLibrary.Add(sectionProfile);
            if (sectionProfileProposal.IsExisting)
            {
                sectionProfileFromLibraryRepository.Put(sectionProfile);
            }
            else
            {
                sectionProfileFromLibraryRepository.Add(sectionProfile);
                sectionProfileProposalIdToDomainDict.Add(sectionProfileProposal.Id, sectionProfile);
            }
        }

        List<Element1d> element1ds = [];
        foreach (var element1dProposal in modelProposal.Element1dProposals)
        {
            var element1d = element1dProposal.ToDomain(
                nodeProposalIdToNodeDict,
                materialProposalIdToMaterialDict,
                sectionProfileProposalIdToDomainDict
            );
            element1ds.Add(element1d);
            if (element1dProposal.IsExisting)
            {
                element1dRepository.Put(element1d);
            }
            else
            {
                element1dRepository.Add(element1d);
            }
        }
        modelProposalRepository.Remove(modelProposal);

        await unitOfWork.SaveChangesAsync(ct);

        ModelToResponseMapper modelToResponseMapper = ModelToResponseMapper.Create(
            modelProposal.Settings.UnitSettings
        );
        ModelResponse response = new(
            modelProposal.ModelId,
            modelProposal.Name,
            modelProposal.Description,
            modelProposal.Settings.ToContract(),
            DateTimeOffset.UtcNow,
            [.. nodes.Select(n => n.ToResponse())],
            [.. element1ds.Select(e => e.ToResponse())],
            [
                .. materials.Select(m =>
                    m.ToResponse(modelProposal.Settings.UnitSettings.PressureUnit)
                ),
            ],
            [
                .. sectionProfiles.Select(s =>
                    s.ToResponse(modelProposal.Settings.UnitSettings.LengthUnit)
                ),
            ],
            [.. sectionProfilesFromLibrary.Select(s => s.ToResponse())]
        // modelProposal.Materials?.Select(m => m.ToContract()).ToList(),
        // modelProposal.SectionProfiles?.Select(s => s.ToContract()).ToList(),
        // modelProposal.PointLoads?.Select(p => p.ToContract()).ToList(),
        // modelProposal.MomentLoads?.Select(m => m.ToContract()).ToList(),
        // modelProposal.ResultSets?.Select(r => r.ToContract()).ToList()
        );

        return response;
    }
}

public class RejectModelProposalCommandHandler(
    IModelProposalRepository modelProposalRepository,
    IStructuralAnalysisUnitOfWork unitOfWork
) : ICommandHandler<IModelEntity, bool>
{
    public async Task<Result<bool>> ExecuteAsync(
        IModelEntity command,
        CancellationToken ct = default
    )
    {
        await modelProposalRepository.RemoveById(command.ModelId, command.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
