using PuppeteerSharp;

namespace BeamOs.DevOps.PipelineHelper;

public static class IElementExtensions
{
    public static async Task<IElementHandle> QuerySelectorAsyncFluent(
        this Task<IElementHandle> task,
        string selector
    )
    {
        return await (await task).QuerySelectorAsync(selector);
    }

    public static async Task<string> GetInnerTextAsync(this IElementHandle elementHandle)
    {
        return await elementHandle.EvaluateFunctionAsync<string>("e => e.innerText");
    }

    public static async Task<string> GetInnerTextAsync(this Task<IElementHandle> elementTask)
    {
        return await (await elementTask).EvaluateFunctionAsync<string>("e => e.innerText");
    }
}
