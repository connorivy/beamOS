using BeamOs.Tests.Common;
using BeamOs.Tests.WebApp.Integration.Extensions;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration.ModelEditorTests;

public partial class ModelEditorPageTests : ReactPageTest
{
    private static readonly SemaphoreSlim ModelCreationLock = new(1, 1);
    private static Guid? modelId;

    [Before(TUnit.Core.HookType.Test)]
    public async Task ModelEditorPage_ShouldLoadSuccessfully()
    {
        // Create a new model and navigate to its editor page
        if (modelId == null)
        {
            await ModelCreationLock.WaitAsync();

            try
            {
                modelId ??= await this.PageContext.NavigateToNewModelPage(
                    modelName: "Test Model",
                    description: "This is a test model for integration testing."
                );
            }
            finally
            {
                ModelCreationLock.Release();
            }
        }

        await this.Page.GotoAsync($"/models/{modelId}");
    }
}
