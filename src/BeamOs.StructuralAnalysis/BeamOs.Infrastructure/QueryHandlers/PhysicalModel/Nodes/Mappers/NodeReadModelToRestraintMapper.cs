using BeamOs.Application.Common.Interfaces;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Infrastructure.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Nodes.Mappers;


//[Mapper]
//internal partial class NodeReadModelToRestraintMapper : IMapper<NodeReadModel, RestraintResponse>
//{
//    public RestraintResponse Map(NodeReadModel source) => this.ToResponse(source);

//    //[MapProperty(
//    //    nameof(@NodeReadModel.Restraint_CanRotateAboutX),
//    //    nameof(@RestraintResponse.CanRotateAboutX)
//    //)]
//    //[MapProperty(
//    //    nameof(@NodeReadModel.Restraint_CanRotateAboutY),
//    //    nameof(@RestraintResponse.CanRotateAboutY)
//    //)]
//    //[MapProperty(
//    //    nameof(@NodeReadModel.Restraint_CanRotateAboutZ),
//    //    nameof(@RestraintResponse.CanRotateAboutZ)
//    //)]
//    //[MapProperty(
//    //    nameof(@NodeReadModel.Restraint_CanTranslateAlongX),
//    //    nameof(@RestraintResponse.CanTranslateAlongX)
//    //)]
//    //[MapProperty(
//    //    nameof(@NodeReadModel.Restraint_CanTranslateAlongY),
//    //    nameof(@RestraintResponse.CanTranslateAlongY)
//    //)]
//    //[MapProperty(
//    //    nameof(@NodeReadModel.Restraint_CanTranslateAlongZ),
//    //    nameof(@RestraintResponse.CanTranslateAlongZ)
//    //)]
//    public partial RestraintResponse ToResponse(NodeReadModel source);
//}
