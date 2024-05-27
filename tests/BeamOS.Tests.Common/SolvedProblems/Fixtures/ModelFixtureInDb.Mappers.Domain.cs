using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures;

public partial class ModelFixtureInDb
{
    public TId ToId<TId>(Guid id)
        where TId : IConstructable<TId, Guid>
    {
        return TId.Construct(Guid.Parse(this.RuntimeIdToDbIdDict[id]));
    }

    public NodeId ToNodeId(Guid id) => this.ToId<NodeId>(id);

    public PointLoadId ToPointLoadId(Guid id) => this.ToId<PointLoadId>(id);

    public MomentLoadId ToMomentLoadId(Guid id) => this.ToId<MomentLoadId>(id);

    public MaterialId ToMaterialId(Guid id) => this.ToId<MaterialId>(id);

    public SectionProfileId ToSectionProfileId(Guid id) => this.ToId<SectionProfileId>(id);

    public Element1DId ToElement1dId(Guid id) => this.ToId<Element1DId>(id);

    public partial PointLoad ToDomainObjectWithDbIds(PointLoadFixture fixture);

    public partial MomentLoad ToDomainObjectWithDbIds(MomentLoadFixture fixture);

    public static Vector<double> ToVector(Vector3D vector3D) => vector3D.ToVector();

    public partial Node ToDomainObjectWithDbIds(NodeFixture fixture);

    public partial Material ToDomainObjectWithDbIds(MaterialFixture fixture);

    public partial SectionProfile ToDomainObjectWithDbIds(SectionProfileFixture fixture);
}
