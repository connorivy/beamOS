using BeamOS.Common.Api;
using BeamOS.Common.Application.Interfaces;
using BeamOS.PhysicalModel.Api.Common.Endpoints;
using BeamOS.PhysicalModel.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Application.Models.Commands;
using BeamOS.PhysicalModel.Contracts.Common;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;

namespace BeamOS.PhysicalModel.Api.Models.Endpoints;
public class GetHydratedModelEndpoint(
    IRepository<ModelId, Model> modelRepository,
    IRepository<NodeId, Node> nodeRepository,
    IRepository<Element1DId, Element1D> element1DRepository) : BaseEndpoint<IdRequest, HydratedModelResponse>
{
    public override void Configure()
    {
        this.Get("model/{id}");
        this.AllowAnonymous();
    }

    public override async Task<HydratedModelResponse> ExecuteAsync(IdRequest req, CancellationToken ct)
    {
        //Model? model = await modelRepository.GetById(new ModelId(Guid.Parse(req.Id)));
        //if (model is null)
        //{
            return null;
        //}

        //HydratedModelResponse response = new(
        //    req.Id,
        //    ""
    }

    //public override async Task HandleAsync(CreateModelRequest req, CancellationToken ct)
    //{
    //    CreateModelCommand command = commandMapper.Map(req);

    //    Model model = await createModelCommandHandler.ExecuteAsync(command, ct);

    //    ModelResponse response = modelResponseMapper.Map(model);

    //    await this.SendAsync(response, cancellation: ct);
    //}
}
