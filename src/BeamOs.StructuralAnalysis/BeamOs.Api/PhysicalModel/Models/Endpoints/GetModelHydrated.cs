using BeamOS.Api;
using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.Element1ds.Mappers;
using BeamOs.Api.PhysicalModel.Materials.Mappers;
using BeamOs.Api.PhysicalModel.Models.Mappers;
using BeamOs.Api.PhysicalModel.MomentLoads.Mappers;
using BeamOs.Api.PhysicalModel.Nodes.Mappers;
using BeamOs.Api.PhysicalModel.PointLoads.Mappers;
using BeamOs.Api.PhysicalModel.SectionProfiles.Mappers;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using BeamOs.Domain.PhysicalModel.Common.Extensions;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Infrastructure;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Api.PhysicalModel.Models.Endpoints;

public class GetModelHydrated(
    BeamOsFastEndpointOptions options,
    BeamOsStructuralDbContext dbContext,
    Element1DResponseMapper element1dResponseMapper,
    NodeResponseMapper nodeResponseMapper,
    MaterialResponseMapper materialResponseMapper,
    SectionProfileResponseMapper sectionProfileResponseMapper,
    PointLoadResponseMapper pointLoadResponseMapper,
    MomentLoadResponseMapper momentLoadResponseMapper,
    ModelSettingsResponseMapper settingsResponseMapper
) : BeamOsFastEndpoint<IdRequestFromPath, ModelResponseHydrated>(options)
{
    public override string Route => "models/{id}/" + CommonApiConstants.HYDRATED_ROUTE;

    public override Http EndpointType => Http.GET;

    public override async Task<ModelResponseHydrated> ExecuteAsync(
        IdRequestFromPath req,
        CancellationToken ct
    )
    {
        ModelId typedId = new(Guid.Parse(req.Id));

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
        //List<MomentLoadResponse> momentLoadResponses = await (
        //    from ml in dbContext.MomentLoads
        //    join n in dbContext.Nodes on ml.NodeId equals n.Id
        //    where n.ModelId == typedId
        //    select momentLoadResponseMapper.Map(ml)
        //).ToListAsync(cancellationToken: ct);

        List<MomentLoad> momentLoads = await (
            from ml in dbContext.MomentLoads
            join n in dbContext.Nodes on ml.NodeId equals n.Id
            where n.ModelId == typedId
            select ml
        ).ToListAsync(cancellationToken: ct);
        List<MomentLoadResponse> momentLoadResponses = momentLoads
            .Select(ml =>
            {
                ml.UseUnitSettings(unitSettings);
                return momentLoadResponseMapper.Map(ml);
            })
            .ToList();

        var settingsResponse = settingsResponseMapper.Map(model1.Settings);
        return new ModelResponseHydrated(
            req.Id,
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
