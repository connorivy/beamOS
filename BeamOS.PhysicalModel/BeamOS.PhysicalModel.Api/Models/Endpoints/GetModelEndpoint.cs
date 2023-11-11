using BeamOS.PhysicalModel.Api.Common.Endpoints;
using BeamOS.PhysicalModel.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Application.Common.Interfaces;
using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;

namespace BeamOS.PhysicalModel.Api.Models.Endpoints;
public class GetModelEndpoint(
    IRepository<ModelId, Model> repository,
    IMapper<Model, ModelResponse> responseMapper) : GetAggregateRootByGuidBasedIdEndpoint<ModelId, Model, ModelResponse>(repository, responseMapper)
{
    public override void Configure()
    {
        this.Get("model/{id}");
        this.AllowAnonymous();
    }
}
