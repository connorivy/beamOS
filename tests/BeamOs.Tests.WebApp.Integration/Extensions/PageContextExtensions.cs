using System.Text.Json;
using System.Text.RegularExpressions;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
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

            // click the submit button and wait for the POST request to create the model
            var submitButton = dialog.GetByRole(AriaRole.Button, new() { Name = "create" });
            var modelResponseTask = page.Page.WaitForResponseAsync(r =>
                r.Url.Contains("/models") && r.Request.Method == "POST", 
                new() { Timeout = System.Diagnostics.Debugger.IsAttached ? 0 : 15_000}
            );
            await submitButton.ClickAsync();
            
            // Wait for the POST request to complete
            var response = await modelResponseTask;
            
            // Verify the response was successful
            if (!response.Ok)
            {
                var responseBody = await response.TextAsync();
                throw new InvalidOperationException($"Model creation failed with status {response.Status}: {responseBody}");
            }
            
            // Get the model ID from the response body
            var responseBodyJson = await response.JsonAsync();
            var responseString = responseBodyJson.ToString() ?? throw new InvalidOperationException("Response body is null.");
            var modelResponse = System.Text.Json.JsonSerializer.Deserialize<ModelResponse>(responseString, BeamOsJsonSerializerContext.Default.ModelResponse) 
                ?? throw new InvalidOperationException("Failed to deserialize model response.");
            
            if (modelResponse.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Model creation returned empty GUID.");
            }
            
            // Wait for the dialog to close and navigation to complete
            // Use Expect().ToHaveURLAsync() instead of WaitForURLAsync() because this is a SPA navigation
            // that doesn't trigger a full page load event
            var expectedUrl = new Regex($"^(http|https)://localhost:\\d+/models/{modelResponse.Id}$");
            await Assertions.Expect(page.Page).ToHaveURLAsync(
                expectedUrl,
                new() { Timeout = System.Diagnostics.Debugger.IsAttached ? 0 : 15_000 }
            );

            return modelResponse.Id;
        }

        public async Task<ModelResponse> CreateTutorial()
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
                r.Url.Contains("/models") && r.Request.Method == "POST", new() { Timeout = System.Diagnostics.Debugger.IsAttached ? 0 : 5000}
            );
            await tutorialCard.ClickAsync();

            // Assert - Verify that the URL has changed to the tutorial page
            // await Assertions.Expect(page.Page).ToHaveURLAsync("/tutorial");

            // Wait for the POST request to /models and extract the modelId from the response
            var response = await modelResponseTask;

            var responseBody = await response.JsonAsync();
            var responseString = responseBody.ToString() ?? throw new InvalidOperationException("Response body is null.");
            return JsonSerializer.Deserialize<ModelResponse>(responseString, BeamOsJsonSerializerContext.Default.ModelResponse) ?? throw new InvalidOperationException("Failed to deserialize model response.");
        }
    }
}