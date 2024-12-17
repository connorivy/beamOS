using BeamOs.StructuralAnalysis.Application.Common;
using Microsoft.AspNetCore.Http;

namespace BeamOs.StructuralAnalysis.Api.Shared.Common;

internal static class RequestBinders
{
    public static Func<HttpRequest, Task<TCommand>> ModelResourceCommandBinder<TCommand, TBody>()
        where TCommand : IModelResourceRequest<TBody>, new() =>
        static async (ctx) =>
        {
            Guid modelId = Guid.Parse((string)ctx.RouteValues["modelId"]);
            TBody body = await ctx.ReadFromJsonAsync<TBody>();

            return new TCommand() { ModelId = modelId, Body = body };
        };
}
