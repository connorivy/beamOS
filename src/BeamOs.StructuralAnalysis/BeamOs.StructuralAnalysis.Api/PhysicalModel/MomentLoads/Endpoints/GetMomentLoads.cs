using BeamOs.Api.PhysicalModel.MomentLoads.Mappers;
using BeamOS.Common.Api;
using BeamOS.PhysicalModel.Api.Mappers;
using BeamOS.PhysicalModel.Contracts.MomentLoad;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.MomentLoadAggregate.ValueObjects;
using BeamOS.PhysicalModel.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Api.PhysicalModel.MomentLoads.Endpoints;

public class GetMomentLoads(
    PhysicalModelDbContext dbContext,
    MomentLoadResponseMapper responseMapper
) : BeamOsEndpoint<string, string[]?, List<MomentLoadResponse>>
{
    public override string Route => "moment-loads";

    public override EndpointType EndpointType => EndpointType.Get;

    public override async Task<List<MomentLoadResponse>> ExecuteAsync(
        [FromQuery] string modelId,
        [FromQuery] string[]? momentLoadIds = null,
        CancellationToken ct = default
    )
    {
        ModelId modelIdTyped = new(Guid.Parse(modelId));

        if (momentLoadIds != null && momentLoadIds.Length > 0)
        {
            var momentLoadIdsTypes = momentLoadIds
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
