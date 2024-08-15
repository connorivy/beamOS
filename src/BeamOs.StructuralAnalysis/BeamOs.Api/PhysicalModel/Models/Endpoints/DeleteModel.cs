using BeamOs.Api.Common;
using BeamOS.Api.Common;
using BeamOs.Application.PhysicalModel.Models;
using BeamOs.Common.Identity.Policies;
using BeamOs.Contracts.Common;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using FastEndpoints;

namespace BeamOs.Api.PhysicalModel.Models.Endpoints;

public class DeleteModel(BeamOsFastEndpointOptions options, IModelRepository modelRepository)
    : BeamOsFastEndpoint<ModelIdRequest, bool>(options)
{
    public override Http EndpointType => Http.DELETE;
    public override string Route => "/models/{modelId}";

    public override void ConfigureAuthentication() =>
        this.Policy(p => p.AddRequirements(new RequireModelOwnerAccess()));

    public override async Task<bool> ExecuteRequestAsync(ModelIdRequest req, CancellationToken ct)
    {
        ModelId modelId = new(Guid.Parse(req.ModelId));
        await modelRepository.RemoveById(modelId, ct);
        //await unitOfWork.SaveChangesAsync(); // I don't think this is necessary right here

        return true;
    }
}
