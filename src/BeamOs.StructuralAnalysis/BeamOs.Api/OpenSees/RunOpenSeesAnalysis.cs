using BeamOS.Api.Common;
using BeamOs.Application.AnalyticalModel.ModelResults;
using BeamOs.Application.OpenSees;
using BeamOs.Contracts.Common;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using FastEndpoints;

namespace BeamOs.Api.OpenSees;

public class RunOpenSeesAnalysis(
    RunOpenSeesAnalysisCommandHandler runOpenSeesAnalysisCommandHandler,
    IModelResultRepository modelResultRepository
) : BeamOsFastEndpoint<ModelIdRequest, bool>
{
    public override string Route => "models/{modelId}/analyze/opensees";

    public override Http EndpointType => Http.POST;

    protected override void ConfigureEndpoint()
    {
        this.Description(x => x.Accepts<ModelIdRequest>());
    }

    public override async Task<bool> ExecuteRequestAsync(ModelIdRequest req, CancellationToken ct)
    {
        ModelId modelId = new(Guid.Parse(req.ModelId));
        if (await modelResultRepository.GetByModelId(modelId, ct) is not null)
        {
            return true;
        }
        return await runOpenSeesAnalysisCommandHandler.ExecuteAsync(req, ct);
    }
}
