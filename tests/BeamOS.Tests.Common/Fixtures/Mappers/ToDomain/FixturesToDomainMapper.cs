using BeamOs.Application.Common.Mappers;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
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
    public static partial PointLoad ToDomain(this PointLoadFixture2 fixture);
}

[Mapper]
public static partial class SectionProfileFixtureToDomainMapper
{
    public static partial SectionProfile ToDomain(this SectionProfileFixture2 fixture);
}

[Mapper]
[UseStaticMapper(typeof(PointLoadFixtureToDomainMapper))]
[UseStaticMapper(typeof(MomentLoadFixtureToDomainMapper))]
public static partial class NodeFixtureToDomainMapper
{
    public static partial Node ToDomain(this NodeFixture2 fixture);
}

[Mapper]
public static partial class MomentLoadFixtureToDomainMapper
{
    public static partial MomentLoad ToDomain(this MomentLoadFixture2 fixture);

    public static Vector<double> ToVector(Vector3D vector3D) => vector3D.ToVector();
}

[Mapper]
public static partial class MaterialFixtureToDomainMapper
{
    public static partial Material ToDomain(this MaterialFixture2 fixture);
}

[Mapper]
[UseStaticMapper(typeof(NodeFixtureToDomainMapper))]
[UseStaticMapper(typeof(MaterialFixtureToDomainMapper))]
[UseStaticMapper(typeof(SectionProfileFixtureToDomainMapper))]
public static partial class Element1dFixtureToDomainMapper
{
    public static partial Element1D ToDomain(this Element1dFixture2 fixture);
}

[Mapper]
[UseStaticMapper(typeof(NodeFixtureToDomainMapper))]
[UseStaticMapper(typeof(Element1dFixtureToDomainMapper))]
[UseStaticMapper(typeof(Vector3ToFromMathnetVector))]
public static partial class ModelFixtureToDomainMapper
{
    public static partial Model ToDomain(this ModelFixture2 fixture);

    public static partial Model ToDomain(this IModelFixture2 fixture);
}
