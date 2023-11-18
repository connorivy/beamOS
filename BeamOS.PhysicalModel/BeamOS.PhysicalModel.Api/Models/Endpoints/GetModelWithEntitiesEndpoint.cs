using BeamOS.PhysicalModel.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Contracts.Common;
using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Contracts.Node;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Infrastructure;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BeamOS.PhysicalModel.Api.Models.Endpoints;

//public class GetModelWithEntitiesEndpoint(
//    PhysicalModelDbContext dbContext,
//    IMapper<Element1D, Element1DResponse> element1dResponseMapper,
//    IMapper<Node, NodeResponse> nodeResponseMapper,
//    IMapper<ModelSettings, ModelSettingsResponse> settingsResponseMapper) : Endpoint<IdRequest, ModelWithEntitiesResponse>
//{
//    public override void Configure()
//    {
//        this.Get("models/{id:alpha}/h");
//        this.AllowAnonymous();
//        this.Summary(s =>
//        {
//            s.ExampleRequest = new CreateModelRequest(
//            "Big Ol' Building",
//            "Description",
//            new ModelSettingsRequest(
//                new UnitSettingsRequest(
//                    "Inch",
//                    "SquareInch",
//                    "CubicInch",
//                    "KilopoundForce",
//                    "KilopoundForcePerInch",
//                    "KilopoundForceInch")
//                )
//            );
//            s.Params["WithEntities"] = "false";
//        });
//    }

//    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
//    {
//        ModelId id = new(Guid.Parse(req.Id));

//        Model model = await dbContext.Models
//            .FirstAsync(m => m.Id == id, cancellationToken: ct);
//        List<Element1DResponse> element1Ds = await dbContext.Element1Ds
//            .Where(el => el.ModelId == id)
//            .Select(el => element1dResponseMapper.Map(el))
//            .ToListAsync(cancellationToken: ct);
//        List<NodeResponse> nodes = await dbContext.Nodes
//            .Where(el => el.ModelId == id)
//            .Select(el => nodeResponseMapper.Map(el))
//            .ToListAsync(cancellationToken: ct);

//        ModelSettingsResponse settingsResponse = settingsResponseMapper.Map(model.Settings);
//        ModelWithEntitiesResponse response = new(
//            req.Id,
//            model.Name,
//            model.Description,
//            settingsResponse,
//            nodes,
//            element1Ds);

//        await this.SendAsync(response, cancellation: ct);
//    }
//}
