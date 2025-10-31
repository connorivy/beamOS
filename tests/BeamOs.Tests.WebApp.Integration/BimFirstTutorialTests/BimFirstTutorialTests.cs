using BeamOs.Tests.WebApp.Integration.Extensions;

namespace BeamOs.Tests.WebApp.Integration.BimFirstTutorialTests;

public partial class BimFirstTutorialTests : ReactPageTest
{
    private static readonly SemaphoreSlim TutorialPageCreationLock = new(1, 1);
    private static Guid? modelId;
    private Guid ModelId =>
        modelId ?? throw new InvalidOperationException("Model ID has not been initialized.");

    [Before(TUnit.Core.HookType.Test)]
    public async Task TutorialPage_ShouldLoadSuccessfully()
    {
        // Create a new model and navigate to its editor page
        if (modelId == null)
        {
            await TutorialPageCreationLock.WaitAsync();

            try
            {
                modelId ??= await this.PageContext.CreateTutorial();
            }
            finally
            {
                TutorialPageCreationLock.Release();
            }
        }

        await this.Page.GotoAsync($"/tutorial?modelId={modelId}");
    }
}
