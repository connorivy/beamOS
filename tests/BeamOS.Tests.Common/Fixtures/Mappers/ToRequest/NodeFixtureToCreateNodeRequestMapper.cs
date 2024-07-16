using BeamOs.Api.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Domain.Common.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using Riok.Mapperly.Abstractions;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers;

[Mapper]
internal static partial class NodeFixtureToCreateNodeRequestMapper
{
    public static CreateNodeRequest ToRequest(this NodeFixture fixture, string modelId)
    {
        LengthUnit unit = fixture.LocationPoint.XCoordinate.Unit;
        return new(
            modelId,
            fixture.LocationPoint.XCoordinate.Value,
            fixture.LocationPoint.YCoordinate.As(unit),
            fixture.LocationPoint.ZCoordinate.As(unit),
            fixture.LocationPoint.XCoordinate.Unit.MapToString(),
            fixture.Restraint.ToRequest()
        );
    }

    private static partial RestraintRequest ToRequest(this Restraint restraint);
}

//[Mapper]
//internal static partial class NodeFixtureToNodeResultResponseMapper
//{
//    public static AnalyticalNodeResponse ToResponse(this NodeResultFixture fixture, string modelId)
//    {
//        LengthUnit unit = fixture.LocationPoint.XCoordinate.Unit;
//        return new(
//            fixture.NodeFixture.,
//            fixture.LocationPoint.XCoordinate.Value,
//            fixture.LocationPoint.YCoordinate.As(unit),
//            fixture.LocationPoint.ZCoordinate.As(unit),
//            fixture.LocationPoint.XCoordinate.Unit.MapToString(),
//            fixture.Restraint.ToRequest()
//        );
//    }

//    private static partial RestraintRequest ToRequest(this Restraint restraint);
//}
