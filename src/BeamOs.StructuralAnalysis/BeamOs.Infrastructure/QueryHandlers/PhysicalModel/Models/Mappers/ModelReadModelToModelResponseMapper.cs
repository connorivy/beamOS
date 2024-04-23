using BeamOs.Application.Common.Interfaces;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Infrastructure.Data.Models;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Element1d.Mappers;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Materials.Mappers;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.MomentLoads.Mapper;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.PointLoads.Mappers;
using BeamOs.Infrastructure.QueryHandlers.PhysicalModel.SectionProfiles.Mappers;

namespace BeamOs.Infrastructure.QueryHandlers.PhysicalModel.Models.Mappers;


//internal partial class ModelReadModelToModelResponseMapper(
//    IFlattenedModelSettingsToModelSettingsResponseMapper modelSettingsCommandMapper,
//    NodeReadModelToResponseMapper nodeReadModelToResponseMapper,
//    Element1dReadModelToResponseMapper element1DReadModelToResponseMapper,
//    MaterialReadModelToResponseMapper materialReadModelToResponseMapper,
//    SectionProfileReadModelToResponseMapper sectionProfileReadModelToResponseMapper,
//    PointLoadReadModelsToResponseMapper pointLoadReadModelsToResponseMapper,
//    MomentLoadReadModelToResponseMapper momentLoadReadModelToResponseMapper
//) : IMapper<ModelReadModel, ModelResponse>
//{
//    public ModelResponse Map(ModelReadModel source)
//    {
//        return new(
//            source.Id.ToString(),
//            source.Name,
//            source.Description,
//            modelSettingsCommandMapper.Map(source),
//            source.Nodes != null ? nodeReadModelToResponseMapper.Map(source.Nodes).ToList() : null,
//            source.Element1ds != null
//                ? element1DReadModelToResponseMapper.Map(source.Element1ds).ToList()
//                : null,
//            source.Materials != null
//                ? materialReadModelToResponseMapper.Map(source.Materials).ToList()
//                : null,
//            source.SectionProfiles != null
//                ? sectionProfileReadModelToResponseMapper.Map(source.SectionProfiles).ToList()
//                : null,
//            source.Nodes?.FirstOrDefault()?.PointLoads != null
//                ? pointLoadReadModelsToResponseMapper
//                    .Map(source.Nodes.SelectMany(n => n.PointLoads))
//                    .ToList()
//                : null,
//            source.Nodes?.FirstOrDefault()?.MomentLoads != null
//                ? momentLoadReadModelToResponseMapper
//                    .Map(source.Nodes.SelectMany(n => n.MomentLoads))
//                    .ToList()
//                : null
//        );
//    }
//}
