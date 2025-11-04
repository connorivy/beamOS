using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

internal class ModelIdConverter : ValueConverter<ModelId, Guid>
{
    public ModelIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class ModelIdProposalConverter : ValueConverter<ModelProposalId, int>
{
    public ModelIdProposalConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class NodeIdConverter : ValueConverter<NodeId, int>
{
    public NodeIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class NodeProposalIdConverter : ValueConverter<NodeProposalId, int>
{
    public NodeProposalIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class PointLoadIdConverter : ValueConverter<PointLoadId, int>
{
    public PointLoadIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class MomentLoadIdConverter : ValueConverter<MomentLoadId, int>
{
    public MomentLoadIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class Element1dIdConverter : ValueConverter<Element1dId, int>
{
    public Element1dIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class Element1dProposalIdConverter : ValueConverter<Element1dProposalId, int>
{
    public Element1dProposalIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class MaterialIdConverter : ValueConverter<MaterialId, int>
{
    public MaterialIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class MaterialProposalIdConverter : ValueConverter<MaterialProposalId, int>
{
    public MaterialProposalIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class SectionProfileIdConverter : ValueConverter<SectionProfileId, int>
{
    public SectionProfileIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class SectionProfileProposalIdConverter : ValueConverter<SectionProfileProposalId, int>
{
    public SectionProfileProposalIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class ResultSetIdConverter : ValueConverter<ResultSetId, int>
{
    public ResultSetIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class DiagramConsistantIntervalIdConverter
    : ValueConverter<DiagramConsistantIntervalId, int>
{
    public DiagramConsistantIntervalIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class ShearForceDiagramIdConverter : ValueConverter<ShearForceDiagramId, int>
{
    public ShearForceDiagramIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class MomentDiagramIdConverter : ValueConverter<MomentDiagramId, int>
{
    public MomentDiagramIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class LoadCaseIdConverter : ValueConverter<LoadCaseId, int>
{
    public LoadCaseIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class LoadCombinationIdConverter : ValueConverter<LoadCombinationId, int>
{
    public LoadCombinationIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class EnvelopeElement1dResultIdConverter : ValueConverter<EnvelopeElement1dResultId, int>
{
    public EnvelopeElement1dResultIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class EnvelopeResultSetIdConverter : ValueConverter<EnvelopeResultSetId, int>
{
    public EnvelopeResultSetIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class ProposalIssueIdConverter : ValueConverter<ProposalIssueId, int>
{
    public ProposalIssueIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class ModelEntityDeleteProposalIdConverter
    : ValueConverter<DeleteModelEntityProposalId, int>
{
    public ModelEntityDeleteProposalIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class OctreeIdConverter : ValueConverter<OctreeId, int>
{
    public OctreeIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

internal class OctreeNodeIdConverter : ValueConverter<OctreeNodeId, int>
{
    public OctreeNodeIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}
