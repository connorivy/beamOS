using BeamOS.Common.Api;
using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Api.Models.Mappers;
using BeamOS.PhysicalModel.Api.PointLoads.Mappers;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Contracts.Material;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using BeamOS.PhysicalModel.Contracts.SectionProfile;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeamOS.PhysicalModel.Api.Models.Endpoints;

public class GetModelHydrated(
    PhysicalModelDbContext dbContext,
    Element1DResponseMapper element1dResponseMapper,
    NodeResponseMapper nodeResponseMapper,
    MaterialResponseMapper materialResponseMapper,
    SectionProfileResponseMapper sectionProfileResponseMapper,
    PointLoadResponseMapper pointLoadResponseMapper,
    ModelSettingsResponseMapper settingsResponseMapper
) : BaseEndpoint, IGetEndpoint<string, ModelResponseHydrated>
{
    public override string Route => "models/{id}/" + CommonApiConstants.HYDRATED_ROUTE;

    public async Task<ModelResponseHydrated> GetAsync(string id, CancellationToken ct)
    {
        ModelId typedId = new(Guid.Parse(id));

        Model model1 = await dbContext
            .Models
            .FirstAsync(m => m.Id == typedId, cancellationToken: ct);
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

        ModelSettingsResponse settingsResponse = settingsResponseMapper.Map(model1.Settings);
        return new ModelResponseHydrated(
            id,
            model1.Name,
            model1.Description,
            settingsResponse,
            nodes,
            element1Ds,
            materials,
            sectionProfiles,
            pointLoads
        );
    }
}