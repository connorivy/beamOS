using System.Net.WebSockets;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public class ModelProposal : BeamOsModelEntity<ModelProposalId>
{
    public ModelProposal(
        ModelId modelId,
        string name,
        string description,
        ModelSettings settings,
        ModelProposalId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.Name = name;
        this.Description = description;
        this.Settings = settings;
    }

    public ModelProposal(
        Model model,
        string? name,
        string? description,
        ModelSettings? settings,
        ModelProposalId? id = null
    )
        : this(
            model.Id,
            name ?? model.Name,
            description ?? model.Description,
            settings ?? model.Settings,
            id
        ) { }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public ModelSettings Settings { get; private set; }
    public DateTimeOffset LastModified { get; set; } = DateTimeOffset.UtcNow;

    public List<NodeProposal>? NodeProposals { get; set; }
    public List<Element1dProposal>? Element1dProposals { get; set; }
    public List<MaterialProposal>? MaterialProposals { get; set; }
    public List<SectionProfileProposal>? SectionProfileProposals { get; set; }
    public List<SectionProfileProposalFromLibrary>? SectionProfileProposalsFromLibrary { get; set; }
    public List<ProposalIssue>? ProposalIssues { get; set; }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected ModelProposal() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public sealed class ProposalIssue : BeamOsModelEntity<ProposalIssueId>
{
    public ProposalIssue(
        ModelId modelId,
        ModelProposalId modelProposalId,
        ExisitingOrProposedGenericId proposedId,
        BeamOsObjectType objectType,
        string message,
        ProposalIssueSeverity severity,
        ProposalIssueCode code,
        ProposalIssueId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.ExistingId = proposedId.ExistingId;
        this.ProposedId = proposedId.ProposedId;
        this.ObjectType = objectType;
        this.ModelProposalId = modelProposalId;
        this.Message = message;
        this.Severity = severity;
        this.Code = code;
    }

    public int? ExistingId { get; private set; }
    public int? ProposedId { get; private set; }
    public BeamOsObjectType ObjectType { get; private set; }
    public ModelProposalId ModelProposalId { get; private set; }
    public ModelProposal? ModelProposal { get; private set; }
    public string Message { get; private set; }
    public ProposalIssueSeverity Severity { get; private set; }
    public ProposalIssueCode Code { get; private set; }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ProposalIssue() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
