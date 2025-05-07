using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate.ValueObjects;
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

public class ModelIdConverter : ValueConverter<ModelId, Guid>
{
    public ModelIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class NodeIdConverter : ValueConverter<NodeId, int>
{
    public NodeIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class PointLoadIdConverter : ValueConverter<PointLoadId, int>
{
    public PointLoadIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class MomentLoadIdConverter : ValueConverter<MomentLoadId, int>
{
    public MomentLoadIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class Element1dIdConverter : ValueConverter<Element1dId, int>
{
    public Element1dIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class MaterialIdConverter : ValueConverter<MaterialId, int>
{
    public MaterialIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class SectionProfileIdConverter : ValueConverter<SectionProfileId, int>
{
    public SectionProfileIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class ResultSetIdConverter : ValueConverter<ResultSetId, int>
{
    public ResultSetIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class DiagramConsistantIntervalIdConverter : ValueConverter<DiagramConsistantIntervalId, int>
{
    public DiagramConsistantIntervalIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class ShearForceDiagramIdConverter : ValueConverter<ShearForceDiagramId, int>
{
    public ShearForceDiagramIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class MomentDiagramIdConverter : ValueConverter<MomentDiagramId, int>
{
    public MomentDiagramIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class LoadCaseIdConverter : ValueConverter<LoadCaseId, int>
{
    public LoadCaseIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class LoadCombinationIdConverter : ValueConverter<LoadCombinationId, int>
{
    public LoadCombinationIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}
