using BeamOS.Common.Api;
using BeamOS.Common.Api.Interfaces;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Contracts.Material;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Contracts.SectionProfile;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.MaterialAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate;
using BeamOS.PhysicalModel.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BeamOS.PhysicalModel.Api.Models.Endpoints;

public class GetModelEndpoint(
    PhysicalModelDbContext dbContext,
    IMapper<Element1D, Element1DResponse> element1dResponseMapper,
    IMapper<Node, NodeResponse> nodeResponseMapper,
    IMapper<Model, ModelResponse> modelResponseMapper,
    IMapper<Material, MaterialResponse> materialResponseMapper,
    IMapper<SectionProfile, SectionProfileResponse> sectionProfileResponseMapper,
    IMapper<ModelSettings, ModelSettingsResponse> settingsResponseMapper)
        : BaseEndpoint, IGetEndpoint<string, ModelResponse, bool?>
{
    public override string Route => "models/{id}";

    public async Task<ModelResponse?> GetSimpleResponse(string request, CancellationToken ct)
    {
        ModelId expectedId = new(Guid.Parse(request));
        Model? element = await dbContext.Models.FirstAsync(m => m.Id == expectedId, cancellationToken: ct);

        if (element is null)
        {
            return null;
        }

        ModelResponse? response = modelResponseMapper.Map(element);
        return response;
    }

    public async Task<ModelResponse> GetAsync(
        string id,
        bool? sendEntities,
        CancellationToken ct)
    {
        if (sendEntities is null or false)
        {
            return await this.GetSimpleResponse(id, ct);
        }
        ModelId typedId = new(Guid.Parse(id));

        Model model1 = await dbContext.Models
            .FirstAsync(m => m.Id == typedId, cancellationToken: ct);
        List<Element1DResponse> element1Ds = await dbContext.Element1Ds
            .Where(el => el.ModelId == typedId)
            .Select(el => element1dResponseMapper.Map(el))
            .ToListAsync(cancellationToken: ct);
        List<NodeResponse> nodes = await dbContext.Nodes
            .Where(el => el.ModelId == typedId)
            .Select(el => nodeResponseMapper.Map(el))
            .ToListAsync(cancellationToken: ct);
        List<MaterialResponse> materials = await dbContext.Materials
            .Where(el => el.ModelId == typedId)
            .Select(el => materialResponseMapper.Map(el))
            .ToListAsync(cancellationToken: ct);
        List<SectionProfileResponse> sectionProfiles = await dbContext.SectionProfiles
            .Where(el => el.ModelId == typedId)
            .Select(el => sectionProfileResponseMapper.Map(el))
            .ToListAsync(cancellationToken: ct);
        List<SectionProfileResponse> pointLoads = await dbContext.PointLoads
            .Where(el => el.ModelId == typedId)
            .Select(el => sectionProfileResponseMapper.Map(el))
            .ToListAsync(cancellationToken: ct);

        ModelSettingsResponse settingsResponse = settingsResponseMapper.Map(model1.Settings);
        return new ModelResponse(
            id,
            model1.Name,
            model1.Description,
            settingsResponse,
            Nodes: nodes,
            Element1Ds: element1Ds,
            Materials: materials,
            SectionProfiles: sectionProfiles);
    }
}
