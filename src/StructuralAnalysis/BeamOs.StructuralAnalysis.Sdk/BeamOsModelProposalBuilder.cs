using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Sdk;

public class BeamOsModelProposalBuilder
{
    public string? Description { get; set; }
    public ModelSettings? Settings { get; set; }

    public List<CreateNodeRequest> CreateNodeProposals { get; } = [];
    public List<PutNodeRequest> ModifyNodeProposals { get; } = [];
    public List<CreateElement1dProposal> CreateElement1dProposals { get; } = [];
    public List<ModifyElement1dProposal> ModifyElement1dProposals { get; } = [];
    public List<PutMaterialRequest> MaterialProposals { get; } = [];
    public List<PutSectionProfileRequest> SectionProfileProposals { get; } = [];
    public List<SectionProfileFromLibrary> SectionProfileFromLibraryProposals { get; } = [];
    public List<PointLoad> PointLoadProposals { get; } = [];
    public List<MomentLoad> MomentLoadProposals { get; } = [];
    public List<ResultSet> ResultSetProposals { get; } = [];
    public List<LoadCase> LoadCaseProposals { get; } = [];
    public List<LoadCombination> LoadCombinationProposals { get; } = [];
    public List<ProposalIssueData> ProposalIssues { get; } = [];

    public ModelProposalData Build()
    {
        return new ModelProposalData
        {
            Description = this.Description,
            Settings = this.Settings,
            CreateNodeProposals = this.CreateNodeProposals,
            ModifyNodeProposals = this.ModifyNodeProposals,
            CreateElement1dProposals = this.CreateElement1dProposals,
            ModifyElement1dProposals = this.ModifyElement1dProposals,
            MaterialProposals = this.MaterialProposals,
            SectionProfileProposals = this.SectionProfileProposals,
            SectionProfileFromLibraryProposals = this.SectionProfileFromLibraryProposals,
            PointLoadProposals = this.PointLoadProposals,
            MomentLoadProposals = this.MomentLoadProposals,
            ResultSetProposals = this.ResultSetProposals,
            LoadCaseProposals = this.LoadCaseProposals,
            LoadCombinationProposals = this.LoadCombinationProposals,
            ProposalIssues = this.ProposalIssues,
        };
    }
}
