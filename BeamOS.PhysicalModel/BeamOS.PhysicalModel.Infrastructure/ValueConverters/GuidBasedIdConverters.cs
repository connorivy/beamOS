using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeamOS.PhysicalModel.Infrastructure.ValueConverters;

public class ModelIdConverter : ValueConverter<ModelId, Guid>
{
    public ModelIdConverter()
        : base(
            x => x.Value,
            x => new(x),
            null)
    { }
}

public class NodeIdConverter : ValueConverter<NodeId, Guid>
{
    public NodeIdConverter()
        : base(
            x => x.Value,
            x => new(x),
            null)
    { }
}
public class Element1DIdConverter : ValueConverter<Element1DId, Guid>
{
    public Element1DIdConverter()
        : base(
            x => x.Value,
            x => new(x),
            null)
    { }
}

public class SectionProfileIdConverter : ValueConverter<SectionProfileId, Guid>
{
    public SectionProfileIdConverter()
        : base(
            x => x.Value,
            x => new(x),
            null)
    { }
}
public class MaterialIdConverter : ValueConverter<MaterialId, Guid>
{
    public MaterialIdConverter()
        : base(
            x => x.Value,
            x => new(x),
            null)
    { }
}
public class PointLoadIdConverter : ValueConverter<PointLoadId, Guid>
{
    public PointLoadIdConverter()
        : base(
            x => x.Value,
            x => new(x),
            null)
    { }
}


