using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOS.Tests.Common.Fixtures;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToDomain;

[Mapper]
public static partial class PointLoadFixtureToDomainMapper
{
    public static partial PointLoad ToDomain(PointLoadFixture2 fixture);
}

[Mapper]
public static partial class SectionProfileFixtureToDomainMapper
{
    public static partial SectionProfile ToDomain(SectionProfileFixture2 fixture);
}

[Mapper]
public static partial class NodeFixtureToDomainMapper
{
    public static partial Node ToDomain(NodeFixture2 fixture);
}

[Mapper]
public static partial class MomentLoadFixtureToDomainMapper
{
    public static partial MomentLoad ToDomain(MomentLoadFixture2 fixture);

    public static Vector<double> ToVector(Vector3D vector3D) => vector3D.ToVector();
}

[Mapper]
public static partial class MaterialFixtureToDomainMapper
{
    public static partial Material ToDomain(MaterialFixture2 fixture);
}

// [Mapper]
public static partial class Element1dFixtureToDomainMapper
{
    public static partial Element1D ToDomain(Element1dFixture2 fixture);
}
