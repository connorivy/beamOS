using BeamOS.PhysicalModel.Api.Common.Endpoints;
using BeamOS.PhysicalModel.Api.Common.Interfaces;
using BeamOS.PhysicalModel.Application.Common.Interfaces;
using BeamOS.PhysicalModel.Contracts.PointLoad;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;

namespace BeamOS.PhysicalModel.Api.PointLoads.Endpoints;

public class GetPointLoadEndpoint(
    IRepository<PointLoadId, PointLoad> repository,
    IMapper<PointLoad, PointLoadResponse> responseMapper) : GetAggregateRootByGuidBasedIdEndpoint<PointLoadId, PointLoad, PointLoadResponse>(repository, responseMapper)
{
    public override void Configure()
    {
        this.Get("point-load/{id}");
        this.AllowAnonymous();
    }
}
