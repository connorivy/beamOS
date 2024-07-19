using BeamOs.Domain.AnalyticalResults.AnalyticalNodeAggregate.ValueObjects;
using BeamOs.Domain.Diagrams.Common.ValueObjects;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate.ValueObjects;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeamOs.Infrastructure.Data.ValueConverters;

/// <summary>
/// These Id value converters are required for EF Core configuration. Without these converters, EF Core will not
/// be able to make proper db migrations, and will complain that the entities need to define their primary keys.
/// I'll be honest, I don't fully understand how these value converters solve that issue, but they do. And because
/// we have these, we don't have to implement a <see cref="IEntityTypeConfiguration"/> for each entity that defines
/// a primary key
/// </summary>
public class ModelIdConverter : ValueConverter<ModelId, Guid>
{
    public ModelIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class NodeIdConverter : ValueConverter<NodeId, Guid>
{
    public NodeIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class Element1DIdConverter : ValueConverter<Element1DId, Guid>
{
    public Element1DIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class SectionProfileIdConverter : ValueConverter<SectionProfileId, Guid>
{
    public SectionProfileIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class MaterialIdConverter : ValueConverter<MaterialId, Guid>
{
    public MaterialIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class PointLoadIdConverter : ValueConverter<PointLoadId, Guid>
{
    public PointLoadIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class MomentLoadIdConverter : ValueConverter<MomentLoadId, Guid>
{
    public MomentLoadIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class NodeResultIdConverter : ValueConverter<NodeResultId, Guid>
{
    public NodeResultIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class ShearForceDiagramIdConverter : ValueConverter<ShearForceDiagramId, Guid>
{
    public ShearForceDiagramIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class MomentDiagramIdConverter : ValueConverter<MomentDiagramId, Guid>
{
    public MomentDiagramIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}

public class DiagramConsistantIntervalIdConverter
    : ValueConverter<DiagramConsistantIntervalId, Guid>
{
    public DiagramConsistantIntervalIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}
