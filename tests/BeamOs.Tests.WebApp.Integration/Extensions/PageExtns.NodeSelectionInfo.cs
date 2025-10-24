using Microsoft.Playwright;

namespace BeamOs.Tests.WebApp.Integration.Extensions;

public static class PageExtns
{
    extension(IPage page)
    {
        public async Task FillOutNodeSelectionInfo(double x, double y, double z, string? nodeId = null, bool canTranslateAlongX = true, bool canTranslateAlongY = true, bool canTranslateAlongZ = true, bool canRotateAboutX = true, bool canRotateAboutY = true, bool canRotateAboutZ = true)
        {
            if (nodeId is not null)
            {
                var nodeIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
                await nodeIdInput.FillAsync(nodeId);

                // select the option in the dropdown
                var dropdownOption = page.GetByRole(
                    AriaRole.Option,
                    new() { Name = nodeId }
                );
                await dropdownOption.ClickAsync();
            }

            // fill in values for x, y, and z textboxes
            var xInput = page.GetByRole(AriaRole.Textbox, new() { Name = "x" });
            await xInput.FillAsync(x.ToString());
            var yInput = page.GetByRole(AriaRole.Textbox, new() { Name = "y" });
            await yInput.FillAsync(y.ToString());
            var zInput = page.GetByRole(AriaRole.Textbox, new() { Name = "z" });
            await zInput.FillAsync(z.ToString());

            if (canTranslateAlongX)
            {
                var canTranslateAlongXCheckbox = page.GetByRole(
                    AriaRole.Checkbox,
                    new() { Name = "translate along x" }
                );
                await canTranslateAlongXCheckbox.CheckAsync();
            }
            if (canTranslateAlongY)
            {
                var canTranslateAlongYCheckbox = page.GetByRole(
                    AriaRole.Checkbox,
                    new() { Name = "translate along y" }
                );
                await canTranslateAlongYCheckbox.CheckAsync();
            }
            if (canTranslateAlongZ)
            {
                var canTranslateAlongZCheckbox = page.GetByRole(
                    AriaRole.Checkbox,
                    new() { Name = "translate along z" }
                );
                await canTranslateAlongZCheckbox.CheckAsync();
            }
            if (canRotateAboutX)
            {
                var canRotateAboutXCheckbox = page.GetByRole(
                    AriaRole.Checkbox,
                    new() { Name = "rotate about x" }
                );
                await canRotateAboutXCheckbox.CheckAsync();
            }
            if (canRotateAboutY)
            {
                var canRotateAboutYCheckbox = page.GetByRole(
                    AriaRole.Checkbox,
                    new() { Name = "rotate about y" }
                );
                await canRotateAboutYCheckbox.CheckAsync();
            }
            if (canRotateAboutZ)
            {
                var canRotateAboutZCheckbox = page.GetByRole(
                    AriaRole.Checkbox,
                    new() { Name = "rotate about z" }
                );
                await canRotateAboutZCheckbox.CheckAsync();
            }
        }

        public async Task AssertNodeDialogValues(double x, double y, double z, string? nodeId = null, bool? canTranslateAlongX = null, bool? canTranslateAlongY = null, bool? canTranslateAlongZ = null, bool? canRotateAboutX = null, bool? canRotateAboutY = null, bool? canRotateAboutZ = null)
        {
            if (nodeId is not null)
            {
                var nodeIdInput = page.GetByRole(AriaRole.Combobox, new() { Name = "id" });
                await Assertions.Expect(nodeIdInput).ToHaveValueAsync(nodeId);
            }

            var xInput = page.GetByRole(AriaRole.Textbox, new() { Name = "x" });
            await xInput.ExpectToHaveApproximateValueAsync(x);
            var yInput = page.GetByRole(AriaRole.Textbox, new() { Name = "y" });
            await yInput.ExpectToHaveApproximateValueAsync(y);
            var zInput = page.GetByRole(AriaRole.Textbox, new() { Name = "z" });
            await zInput.ExpectToHaveApproximateValueAsync(z);
            if (canTranslateAlongX.HasValue)
            {
                var canTranslateAlongXCheckbox = page.GetByRole(
                    AriaRole.Checkbox,
                    new() { Name = "translate along x" }
                );
                if (canTranslateAlongX.Value)
                {
                    await Assertions.Expect(canTranslateAlongXCheckbox).ToBeCheckedAsync();
                }
                else
                {
                    await Assertions.Expect(canTranslateAlongXCheckbox).Not.ToBeCheckedAsync();
                }
            }

            if (canTranslateAlongY.HasValue)
            {
                var canTranslateAlongYCheckbox = page.GetByRole(
                    AriaRole.Checkbox,
                    new() { Name = "translate along y" }
                );
                if (canTranslateAlongY.Value)
                {
                    await Assertions.Expect(canTranslateAlongYCheckbox).ToBeCheckedAsync();
                }
                else
                {
                    await Assertions.Expect(canTranslateAlongYCheckbox).Not.ToBeCheckedAsync();
                }
            }

            if (canTranslateAlongZ.HasValue)
            {
                var canTranslateAlongZCheckbox = page.GetByRole(
                    AriaRole.Checkbox,
                    new() { Name = "translate along z" }
                );

                if (canTranslateAlongZ.Value)
                {
                    await Assertions.Expect(canTranslateAlongZCheckbox).ToBeCheckedAsync();
                }
                else
                {
                    await Assertions.Expect(canTranslateAlongZCheckbox).Not.ToBeCheckedAsync();
                }
            }
        }
    }
}
