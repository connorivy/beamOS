using BeamOS.Common.Application.Interfaces;
using BeamOS.Common.Domain.Interfaces;
using BeamOS.Common.Domain.Models;
using BeamOS.PhysicalModel.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Contracts.Common;

namespace BeamOS.PhysicalModel.Api.Common.Endpoints;

public abstract class GetAggregateRootByGuidBasedIdEndpoint<TId, TAggregate, TResponse>(
    IRepository<TId, TAggregate> repository,
    IMapper<TAggregate, TResponse> responseMapper) : FastEndpoints.Endpoint<IdRequest, TResponse>
        where TId : notnull, IConstructable<TId, Guid>
        where TAggregate : AggregateRoot<TId>
{
    public abstract override void Configure();
    //{
    //    this.Get("model/{id}");
    //    this.AllowAnonymous();
    //}

    public override async Task HandleAsync(IdRequest req, CancellationToken ct)
    {
        TId id = TId.Construct(Guid.Parse(req.Id));

        TAggregate? model = await repository.GetById(id);

        if (model is not null)
        {
            TResponse response = responseMapper.Map(model);

            await this.SendAsync(response, cancellation: ct);
        }
        else
        {
            await this.SendNotFoundAsync(cancellation: ct);
        }
    }
}
