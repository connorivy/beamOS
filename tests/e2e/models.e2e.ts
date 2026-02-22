import { expect, test, type Page } from "@playwright/test";
import { kassimaliExample3_8Project } from "../fixtures/Kassimali_MatrixAnalysisOfStructures2ndEd/Kassimali_Example3_8.fixture";

const TUTORIAL_PROJECT_ID = kassimaliExample3_8Project.project.id;
const DEFAULT_FEATURE_BRANCH = "feature/add-loads";

const completeMission1ByForkingProject = async (page: Page) => {
    await page.goto("/tutorial");

    const mission1Visible = await page.getByText("Mission 1 — Fork the Model").isVisible();
    if (mission1Visible) {
        await expect(page.getByText("Fork this model to create your own working copy.")).toBeVisible();
        await page.getByRole("button", { name: /Next/ }).click();
        await expect(page.getByText("Fork the Project")).toBeVisible();
    }

    await expect(page.locator("#tutorial-fork-button")).toBeEnabled({ timeout: 30_000 });
    await page.locator("#tutorial-fork-button").evaluate((el) =>
        (el as HTMLButtonElement).click(),
    );

    await expect
        .poll(async () => (await page.locator("#tutorial-project-id").innerText()).trim(), {
            timeout: 30_000,
        })
        .not.toBe(TUTORIAL_PROJECT_ID);

    const branchValue = await page.getByRole("combobox", { name: "Branch" }).innerText();
    expect(branchValue).toBe("main");
    const newProjectId = (await page.locator("#tutorial-project-id").innerText()).trim();
    expect(newProjectId).toBeDefined();
    expect(newProjectId).not.toBe(TUTORIAL_PROJECT_ID);

    await expect(page.getByRole("heading", { name: "beamOS Editor" })).toBeVisible();
    return newProjectId;
};

const completeMission2ByCreatingBranch = async (page: Page) => {
    const mission2Visible = await page.getByText("Mission 2 — Create a Branch").isVisible();
    if (mission2Visible) {
        await expect(page.getByText("Create a branch to make your changes.")).toBeVisible();
        await page.getByRole("button", { name: /Next/ }).click();
    }
    await expect(page.getByText("Create Branch")).toBeVisible();

    await page.locator("#tutorial-create-branch-button").evaluate((el) =>
        (el as HTMLButtonElement).click(),
    );

    await expect(page.getByRole("dialog", { name: "Create branch" })).toBeVisible();
    await page.getByLabel("Branch name").fill(DEFAULT_FEATURE_BRANCH);
    await page.getByRole("button", { name: "Create" }).evaluate((el) =>
        (el as HTMLButtonElement).click(),
    );

    await expect(page).toHaveURL("/tutorial");
    await expect(page.getByRole("combobox", { name: "Branch" })).toHaveText(DEFAULT_FEATURE_BRANCH);
};

test("clicking tutorial card navigates to the editor", async ({ page }) => {
    await page.goto("/models");

    await expect(page.getByRole("heading", { name: "Sample Models" })).toBeVisible();

    const tutorialCard = page.getByRole("link", { name: /Tutorial/ });
    await expect(tutorialCard).toBeVisible();

    await tutorialCard.click();

    await expect(page).toHaveURL("/tutorial");
});

test("tutorial editor: complete Mission 1+2 and create backend branch", async ({ page, request }) => {
    const newProjectId = await completeMission1ByForkingProject(page);
    if (!newProjectId) {
        throw new Error("Expected Mission 1 to navigate to a forked project.");
    }

    await completeMission2ByCreatingBranch(page);

    const branchResponse = await request.get(
        `/api/projects/${newProjectId}/branches/${encodeURIComponent(DEFAULT_FEATURE_BRANCH)}`,
    );
    expect(branchResponse.ok()).toBeTruthy();
});
