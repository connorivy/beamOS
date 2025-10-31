using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration.Extensions;

public static class PageContextExtensions
{
    extension(PageContext page)
    {
        public async Task<Guid> NavigateToNewModelPage(
            string? modelName = null,
            string? description = null,
            string? lengthUnit = null,
            string? forceUnit = null
        )
        {
            await AssemblySetup.CreateAuthenticatedUser(page);

            await page.Page.GotoAsync(
                "/models",
                new PageGotoOptions() { WaitUntil = WaitUntilState.NetworkIdle }
            );

            // find the create model button and click it
            var createModelButton = page.Page.GetByRole(
                AriaRole.Button,
                new PageGetByRoleOptions { Name = "create model" }
            );
            await createModelButton.ClickAsync();

            // the 'create model' dialog should be visible
            var dialog = page.Page.GetByRole(AriaRole.Dialog);

            // fill the model name input
            var modelNameInput = dialog.GetByLabel("name");
            await modelNameInput.FillAsync(modelName ?? "My Test Model");

            // fill the description input
            var descriptionInput = dialog.GetByLabel("description");
            await descriptionInput.FillAsync(description ?? "This is a test model created during integration testing.");
            // select the length unit from the dropdown
            var lengthUnitDropdown = dialog
                .GetByRole(AriaRole.Combobox)
                .Filter(new() { HasText = "Length Unit" });
            await lengthUnitDropdown.ClickAsync();
            var lengthUnitOption = page.Page.GetByRole(AriaRole.Option, new() { Name = lengthUnit ?? "foot" });
            await lengthUnitOption.ClickAsync();

            // select the force unit from the forceunit dropdown
            var forceUnitDropdown = dialog
                .GetByRole(AriaRole.Combobox)
                .Filter(new() { HasText = "Force Unit" });
            await forceUnitDropdown.ClickAsync();
            var forceUnitOption = page.Page.GetByRole(AriaRole.Option, new() { Name = forceUnit ?? "kilopoundforce" });
            await forceUnitOption.ClickAsync();

            // click the submit button
            var submitButton = dialog.GetByRole(AriaRole.Button, new() { Name = "create" });
            await submitButton.ClickAsync();

            // wait for the page url to look something like {http}://localhost:{port}/models/{guid}
            await page.Page.WaitForURLAsync(
                new Regex("^(http|https)://localhost:\\d+/models/[0-9a-fA-F-]{36}$"),
                new() { Timeout = 3000 }
            );

            // get the model id from the url
            var url = page.Page.Url;
            var modelId = url.Split('/').Last();
            var modelIdGuid = Guid.Parse(modelId);
            return modelIdGuid;
        }

        public async Task<Guid> CreateTutorial()
        {
            await AssemblySetup.CreateAuthenticatedUser(page);

            await page.Page.GotoAsync(
                "/models",
                new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle }
            );

            // Find the card with the heading "Tutorial" and click it
            var tutorialCard = page.Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Tutorial" })
                .First;

            var modelResponseTask = page.Page.WaitForResponseAsync(r =>
                r.Url.Contains("/models") && r.Request.Method == "POST"
            );
            await tutorialCard.ClickAsync();

            // Assert - Verify that the URL has changed to the tutorial page
            await Assertions.Expect(page.Page).ToHaveURLAsync("/tutorial");

            // Wait for the POST request to /models and extract the modelId from the response
            var response = await modelResponseTask;

            var responseBody = await response.JsonAsync();
            Console.WriteLine($"responseBody: {responseBody}");
            var modelId = responseBody?.GetProperty("id").GetString() ?? throw new InvalidOperationException("Model ID not found in response.");
            return Guid.Parse(modelId!);
        }
    }
}