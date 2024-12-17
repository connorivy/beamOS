using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeamOs.StructuralAnalysis.Infrastructure.Common;

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

public class ModelIdConverter : ValueConverter<ModelId, Guid>
{
    public ModelIdConverter()
        : base(x => x.Id, x => new(x), null) { }
}
