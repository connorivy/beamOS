using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Components.Features.Common;
using Fluxor;
using Microsoft.Extensions.Caching.Hybrid;

namespace BeamOs.WebApp.Components.Features.StructuralApi;


//public class UpdateCacheForModified(HybridCache cache)
//{
//    [EffectMethod]
//    public async Task HandleModelEntityCreatedAction(ModelEntityCreated action)
//    {
//        var cached = await cache.GetOrDefaultAsync(action.ModelEntity.ModelId.ToString(), default(CachedModelResponse));

//        if (cached is null)
//        {
//            return;
//        }

//        if (action.ModelEntity is NodeResponse nodeResponse)
//        {
//            cached.Nodes[nodeResponse.Id] = nodeResponse;
//        }
//    }
//}

//public static class HybridCacheExtensions
//{
//    public static async ValueTask<TValue?> GetOrDefaultAsync<TValue>(
//        this HybridCache cache,
//        string key,
//        TValue defaultValue,
//        CancellationToken ct = default
//    )
//    {
//        return await cache.GetOrCreateAsync<TValue>(
//            key,
//            _ => new ValueTask<TValue>(defaultValue),
//            new HybridCacheEntryOptions
//            {
//                Flags =
//                    HybridCacheEntryFlags.DisableLocalCacheWrite
//                    | HybridCacheEntryFlags.DisableDistributedCacheWrite
//            },
//            cancellationToken: ct
//        );
//    }
//}
