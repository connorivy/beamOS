using BeamOs.ApiClient.Builders;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Common.Domain.Interfaces;
using BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using MathNet.Spatial.Euclidean;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.Fixtures.Mappers.ToDomain;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public partial class CreateModelRequestBuilderToDomainMapper
{
    private readonly Dictionary<string, Guid> runtimeIdToGuidDict = [];

    //public static ModelId ToTypedId(FixtureId fixtureId) =>
    //    new(Guid.Parse(fixtureId.ToString()));

    public static ModelId ToStrongModelId(FixtureId id) => new(Guid.Parse(id.ToString()));

    public NodeId ToStrongNodeId(FixtureId id) => this.ToStrongId<NodeId>(id);

    public PointLoadId ToStrongPointLoadId(FixtureId id) => this.ToStrongId<PointLoadId>(id);

    public MomentLoadId ToStrongMomentLoadId(FixtureId id) => this.ToStrongId<MomentLoadId>(id);

    public SectionProfileId ToStrongSectionProfileId(FixtureId id) =>
        this.ToStrongId<SectionProfileId>(id);

    public MaterialId ToStrongMaterialId(FixtureId id) => this.ToStrongId<MaterialId>(id);

    public Element1DId ToStrongElement1DId(FixtureId id) => this.ToStrongId<Element1DId>(id);

    public static Vector3D ToNonUnit(UnitVector3D unitVector) => unitVector.ToVector3D();

    public TId ToStrongId<TId>(FixtureId id)
        where TId : IConstructable<TId, Guid>
    {
        if (!this.runtimeIdToGuidDict.TryGetValue(id.ToString(), out Guid dbId))
        {
            dbId = Guid.NewGuid();
            this.runtimeIdToGuidDict[id.ToString()] = dbId;
        }
        return TId.Construct(dbId);
    }

    //public partial SectionProfile ToDomain(CreateSectionProfileRequestBuilder builder);

    //public partial PointLoad ToDomain(CreatePointLoadRequestBuilder builder);

    public partial Model ToDomain(CreateModelRequestBuilder fixture);
}
