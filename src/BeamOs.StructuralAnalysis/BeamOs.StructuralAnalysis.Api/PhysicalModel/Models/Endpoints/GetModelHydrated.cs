using BeamOs.Api.Common;
using BeamOs.Api.PhysicalModel.Element1ds.Mappers;
using BeamOs.Api.PhysicalModel.Materials.Mappers;
using BeamOs.Api.PhysicalModel.Models.Mappers;
using BeamOs.Api.PhysicalModel.MomentLoads.Mappers;
using BeamOs.Api.PhysicalModel.Nodes.Mappers;
using BeamOs.Api.PhysicalModel.PointLoads.Mappers;
using BeamOs.Api.PhysicalModel.SectionProfiles.Mappers;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Contracts.Material;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Contracts.MomentLoad;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using BeamOS.PhysicalModel.Contracts.SectionProfile;
using BeamOS.PhysicalModel.Domain.Common.Extensions;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Api.PhysicalModel.Models.Endpoints;

public class GetModelHydrated(
    PhysicalModelDbContext dbContext,
    Element1DResponseMapper element1dResponseMapper,
    NodeResponseMapper nodeResponseMapper,
    MaterialResponseMapper materialResponseMapper,
    SectionProfileResponseMapper sectionProfileResponseMapper,
    PointLoadResponseMapper pointLoadResponseMapper,
    MomentLoadResponseMapper momentLoadResponseMapper,
    ModelSettingsResponseMapper settingsResponseMapper
) : BeamOsEndpoint<string, ModelResponseHydrated>
{
    public override string Route => "models/{id}/" + CommonApiConstants.HYDRATED_ROUTE;

    public override EndpointType EndpointType => EndpointType.Get;

    public override async Task<ModelResponseHydrated> ExecuteAsync(string id, CancellationToken ct)
    {
        ModelId typedId = new(Guid.Parse(id));

        Model model1 = await dbContext
            .Models
            .FirstAsync(m => m.Id == typedId, cancellationToken: ct);
        UnitSettings unitSettings = model1.Settings.UnitSettings;

        List<Element1DResponse> element1Ds = await dbContext
            .Element1Ds
            .Where(el => el.ModelId == typedId)
            .Select(el => element1dResponseMapper.Map(el))
            .ToListAsync(cancellationToken: ct);
        List<NodeResponse> nodes = await dbContext
            .Nodes
            .Where(el => el.ModelId == typedId)
            .Select(el => nodeResponseMapper.Map(el))
            .ToListAsync(cancellationToken: ct);
        List<MaterialResponse> materials = await dbContext
            .Materials
            .Where(el => el.ModelId == typedId)
            .Select(el => materialResponseMapper.Map(el))
            .ToListAsync(cancellationToken: ct);
        List<SectionProfileResponse> sectionProfiles = await dbContext
            .SectionProfiles
            .Where(el => el.ModelId == typedId)
            .Select(el => sectionProfileResponseMapper.Map(el))
            .ToListAsync(cancellationToken: ct);
        List<PointLoadResponse> pointLoads = await (
            from pl in dbContext.PointLoads
            join n in dbContext.Nodes on pl.NodeId equals n.Id
            where n.ModelId == typedId
            select pointLoadResponseMapper.Map(pl)
        ).ToListAsync(cancellationToken: ct);
        List<MomentLoadResponse> momentLoadResponses = await (
            from ml in dbContext.MomentLoads
            join n in dbContext.Nodes on ml.NodeId equals n.Id
            where n.ModelId == typedId
            select momentLoadResponseMapper.Map(ml)
        ).ToListAsync(cancellationToken: ct);

        //List<MomentLoad> momentLoads = await (
        //    from ml in dbContext.MomentLoads
        //    join n in dbContext.Nodes on ml.NodeId equals n.Id
        //    where n.ModelId == typedId
        //    select ml
        //).ToListAsync(cancellationToken: ct);
        //List<MomentLoadResponse> momentLoadResponses = momentLoads
        //    .Select(ml =>
        //    {
        //        ml.UseUnitSettings(unitSettings);
        //        return momentLoadResponseMapper.Map(ml);
        //    })
        //    .ToList();

        var settingsResponse = settingsResponseMapper.Map(model1.Settings);
        return new ModelResponseHydrated(
            id,
            model1.Name,
            model1.Description,
            settingsResponse,
            nodes,
            element1Ds,
            materials,
            sectionProfiles,
            pointLoads,
            momentLoadResponses
        );
    }
}
