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

public record BeamOsModelProposalBuilder : ModelProposalData
{
    public new List<CreateNodeRequest> CreateNodeProposals
    {
        get => base.CreateNodeProposals;
    }
    public new List<PutNodeRequest> ModifyNodeProposals
    {
        get => base.ModifyNodeProposals;
    }
    public new List<CreateElement1dProposal> CreateElement1dProposals
    {
        get => base.CreateElement1dProposals;
    }
    public new List<ModifyElement1dProposal> ModifyElement1dProposals
    {
        get => base.ModifyElement1dProposals;
    }
    public new List<CreateMaterialRequest> CreateMaterialProposals
    {
        get => base.CreateMaterialProposals;
    }
    public new List<PutMaterialRequest> ModifyMaterialProposals
    {
        get => base.ModifyMaterialProposals;
    }
    public new List<CreateSectionProfileRequest> CreateSectionProfileProposals
    {
        get => base.CreateSectionProfileProposals;
    }
    public new List<PutSectionProfileRequest> ModifySectionProfileProposals
    {
        get => base.ModifySectionProfileProposals;
    }
    public new List<CreateSectionProfileFromLibraryRequest> CreateSectionProfileFromLibraryProposals
    {
        get => base.CreateSectionProfileFromLibraryProposals;
    }
    public new List<PointLoad> PointLoadProposals
    {
        get => base.PointLoadProposals;
    }
    public new List<MomentLoad> MomentLoadProposals
    {
        get => base.MomentLoadProposals;
    }
    public new List<ResultSet> ResultSetProposals
    {
        get => base.ResultSetProposals;
    }
    public new List<LoadCase> LoadCaseProposals
    {
        get => base.LoadCaseProposals;
    }
    public new List<LoadCombination> LoadCombinationProposals
    {
        get => base.LoadCombinationProposals;
    }
    public new List<ProposalIssueData> ProposalIssues
    {
        get => base.ProposalIssues;
    }

    public BeamOsModelProposalBuilder()
    {
        base.CreateNodeProposals = [];
        base.ModifyNodeProposals = [];
        base.CreateElement1dProposals = [];
        base.ModifyElement1dProposals = [];
        base.CreateMaterialProposals = [];
        base.ModifyMaterialProposals = [];
        base.CreateSectionProfileProposals = [];
        base.ModifySectionProfileProposals = [];
        base.CreateSectionProfileFromLibraryProposals = [];
        base.PointLoadProposals = [];
        base.MomentLoadProposals = [];
        base.ResultSetProposals = [];
        base.LoadCaseProposals = [];
        base.LoadCombinationProposals = [];
        base.ProposalIssues = [];
    }

    public ModelProposalData Build() => this;
}
