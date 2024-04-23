using BeamOs.Application.Common.Interfaces;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Infrastructure.Data.Interfaces;
using BeamOs.Infrastructure.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Nodes.Mappers;


//[Mapper]
//internal partial class IFlattenedLocationPointToLocationPointResponseMapper
//    : IMapper<IFlattenedLocationPoint, PointResponse>
//{
//    public PointResponse Map(IFlattenedLocationPoint source) => this.ToResponse(source);

//    [MapProperty(
//        nameof(IFlattenedLocationPoint.LocationPoint_XCoordinate),
//        nameof(@PointResponse.XCoordinate)
//    )]
//    [MapProperty(
//        nameof(IFlattenedLocationPoint.LocationPoint_YCoordinate),
//        nameof(@PointResponse.YCoordinate)
//    )]
//    [MapProperty(
//        nameof(IFlattenedLocationPoint.LocationPoint_ZCoordinate),
//        nameof(@PointResponse.ZCoordinate)
//    )]
//    public partial PointResponse ToResponse(IFlattenedLocationPoint source);
//}

//[Mapper]
//internal partial class NodeReadModelToLocationPointResponseMapper
//    : IMapper<NodeReadModel, PointResponse>
//{
//    private readonly IFlattenedLocationPointToLocationPointResponseMapper pointResponseMapper =
//        new();

//    public PointResponse Map(NodeReadModel source) => this.pointResponseMapper.Map(source);
//}
