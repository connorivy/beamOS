using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.Element1ds.Mappers;
using BeamOs.Api.PhysicalModel.Materials.Mappers;
using BeamOs.Api.PhysicalModel.Models.Extensions;
using BeamOs.Api.PhysicalModel.Models.Mappers;
using BeamOs.Api.PhysicalModel.MomentLoads.Mappers;
using BeamOs.Api.PhysicalModel.Nodes.Mappers;
using BeamOs.Api.PhysicalModel.PointLoads.Mappers;
using BeamOs.Api.PhysicalModel.SectionProfiles.Mappers;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Material;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;
using BeamOs.Contracts.PhysicalModel.SectionProfile;
using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.Common.Extensions;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
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
) : BeamOsFastEndpoint<GetModelHydratedRequest, ModelResponseHydrated>(options)
{
    public override string Route => "models/{modelId}/" + CommonApiConstants.HYDRATED_ROUTE;

    public override Http EndpointType => Http.GET;

    public override async Task<ModelResponseHydrated> ExecuteAsync(
        GetModelHydratedRequest req,
        CancellationToken ct
    )
    {
        ModelId typedId = new(Guid.Parse(req.ModelId));

        Model model1 = await dbContext
            .Models
            .FirstAsync(m => m.Id == typedId, cancellationToken: ct);

        UnitSettings unitSettings = req.Units?.ToDomainObject() ?? model1.Settings.UnitSettings;

        List<Node> nodes = await (
            from n in dbContext.Nodes
            join m in dbContext.Models on n.ModelId equals m.Id
            where m.Id == typedId
            select n
        ).ToListAsync(cancellationToken: ct);
        List<NodeResponse> nodeResponses = MapUnitsAndGetResponse(
            nodes,
            unitSettings,
            nodeResponseMapper
        );

        List<Element1D> element1ds = await (
            from e in dbContext.Element1Ds
            join m in dbContext.Models on e.ModelId equals m.Id
            where m.Id == typedId
            select e
        ).ToListAsync(cancellationToken: ct);
        List<Element1DResponse> element1dResponses = MapUnitsAndGetResponse(
            element1ds,
            unitSettings,
            element1dResponseMapper
        );

        var materials = await (
            from entity in dbContext.Materials
            join m in dbContext.Models on entity.ModelId equals m.Id
            where m.Id == typedId
            select entity
        ).ToListAsync(cancellationToken: ct);
        List<MaterialResponse> materialResponses = MapUnitsAndGetResponse(
            materials,
            unitSettings,
            materialResponseMapper
        );

        var sectionProfiles = await (
            from entity in dbContext.SectionProfiles
            join m in dbContext.Models on entity.ModelId equals m.Id
            where m.Id == typedId
            select entity
        ).ToListAsync(cancellationToken: ct);
        List<SectionProfileResponse> sectionProfileResponses = MapUnitsAndGetResponse(
            sectionProfiles,
            unitSettings,
            sectionProfileResponseMapper
        );

        var pointLoads = await (
            from entity in dbContext.PointLoads
            join n in dbContext.Nodes on entity.NodeId equals n.Id
            where n.ModelId == typedId
            select entity
        ).ToListAsync(cancellationToken: ct);
        List<PointLoadResponse> pointLoadResponses = MapUnitsAndGetResponse(
            pointLoads,
            unitSettings,
            pointLoadResponseMapper
        );

        List<MomentLoad> momentLoads = await (
            from ml in dbContext.MomentLoads
            join n in dbContext.Nodes on ml.NodeId equals n.Id
            where n.ModelId == typedId
            select ml
        ).ToListAsync(cancellationToken: ct);
        List<MomentLoadResponse> momentLoadResponses = MapUnitsAndGetResponse(
            momentLoads,
            unitSettings,
            momentLoadResponseMapper
        );

        var settingsResponse = settingsResponseMapper.Map(model1.Settings);
        return new ModelResponseHydrated(
            req.ModelId,
            model1.Name,
            model1.Description,
            settingsResponse,
            nodeResponses,
            element1dResponses,
            materialResponses,
            sectionProfileResponses,
            pointLoadResponses,
            momentLoadResponses
        );
    }

    private static List<TResponse> MapUnitsAndGetResponse<TEntity, TResponse>(
        IEnumerable<TEntity> entities,
        UnitSettings unitSettings,
        IMapper<TEntity, TResponse> responseMapper
    )
        where TEntity : IBeamOsDomainObject
    {
        if (unitSettings == UnitSettings.SI)
        {
            return entities.Select(responseMapper.Map).ToList();
        }

        return entities
            .Select(e =>
            {
                e.UseUnitSettings(unitSettings);
                return responseMapper.Map(e);
            })
            .ToList();
    }
}
