using BeamOS.Api;
using BeamOS.Api.Common;
using BeamOs.Api.PhysicalModel.MomentLoads.Mappers;
using BeamOs.Contracts.PhysicalModel.MomentLoad;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Infrastructure;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Api.PhysicalModel.MomentLoads.Endpoints;

public class GetMomentLoads(
    BeamOsFastEndpointOptions options,
    BeamOsStructuralDbContext dbContext,
    MomentLoadResponseMapper responseMapper
) : BeamOsFastEndpoint<GetMomentLoadRequest, List<MomentLoadResponse>>(options)
{
    public override string Route => "moment-loads";

    public override Http EndpointType => Http.GET;

    public override async Task<List<MomentLoadResponse>> ExecuteAsync(
        GetMomentLoadRequest req,
        CancellationToken ct = default
    )
    {
        ModelId modelIdTyped = new(Guid.Parse(req.ModelId));

        if (req.MomentLoadIds != null && req.MomentLoadIds.Length > 0)
        {
            var momentLoadIdsTypes = req.MomentLoadIds
                .Select(id => new MomentLoadId(Guid.Parse(id)))
                .ToHashSet();

            return await (
                from ml in dbContext.MomentLoads
                join node in dbContext.Nodes on ml.NodeId equals node.Id
                join model in dbContext.Models on node.ModelId equals model.Id
                where momentLoadIdsTypes.Contains(ml.Id)
                select responseMapper.Map(ml)
            ).ToListAsync(cancellationToken: ct);
        }
        else
        {
            return await (
                from ml in dbContext.MomentLoads
                join node in dbContext.Nodes on ml.NodeId equals node.Id
                join model in dbContext.Models on node.ModelId equals model.Id
                select responseMapper.Map(ml)
            ).ToListAsync(cancellationToken: ct);
        }
    }
}
