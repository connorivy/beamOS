using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeamOs.Infrastructure.PhysicalModel.Common.ValueConverters;

public class ModelIdConverter : ValueConverter<ModelId, Guid>
{
    public ModelIdConverter()
        : base(x => x.Value, x => new(x), null) { }
}

public class NodeIdConverter : ValueConverter<NodeId, Guid>
{
    public NodeIdConverter()
        : base(x => x.Value, x => new(x), null) { }
}

public class Element1DIdConverter : ValueConverter<Element1DId, Guid>
{
    public Element1DIdConverter()
        : base(x => x.Value, x => new(x), null) { }
}

public class SectionProfileIdConverter : ValueConverter<SectionProfileId, Guid>
{
    public SectionProfileIdConverter()
        : base(x => x.Value, x => new(x), null) { }
}

public class MaterialIdConverter : ValueConverter<MaterialId, Guid>
{
    public MaterialIdConverter()
        : base(x => x.Value, x => new(x), null) { }
}

public class PointLoadIdConverter : ValueConverter<PointLoadId, Guid>
{
    public PointLoadIdConverter()
        : base(x => x.Value, x => new(x), null) { }
}

public class MomentLoadIdConverter : ValueConverter<MomentLoadId, Guid>
{
    public MomentLoadIdConverter()
        : base(x => x.Value, x => new(x), null) { }
}
