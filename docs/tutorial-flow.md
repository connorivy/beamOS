# Tutorial Flow

This document describes the desired end-to-end flow for the beamOS onboarding tutorial. Each mission introduces one core concept through a concrete action.

---

## Mission 1 — Fork the Model

**Concept:** Ownership + Everything is tracked

**Prompt:**

> "Fork this model to create your own working copy."

**After fork:**

> "This is your workspace. Every change you make is saved as a version."

**Success Checks**

- Fork created
- User lands in their workspace

---

## Mission 2 — Create a Branch

**Concept:** Safe change isolation

**Prompt:**

> "Create a branch to make your changes."

Default suggestion: `feature/add-loads`

**After branch creation:**

> "Branches let you experiment without affecting main."

**Success Checks**

- Branch created
- User switched onto branch

---

## Mission 3 — Add Structural Data

**Concept:** Modeling + Commits

Have the user:

- Add a material
- Assign a section
- Add a load case

After each action, auto-create a commit. Show a micro-toast:

> "Change committed."

After the second commit:

> "Small commits make changes easy to review."

**Success Checks**

- Required assignments complete
- Load case exists
- At least 2 commits created on branch

---

## Mission 4 — Run Analysis

**Concept:** Results are tied to a version

**Prompt:**

> "Run analysis on your branch."

**After completion:**

> "Results are saved to this version of the model."

Optionally show the commit hash reference alongside the results.

**Success Checks**

- Analysis completed
- Results artifact saved

---

## Mission 5 — Open a Pull Request

**Concept:** Controlled integration

**Prompt:**

> "Open a Pull Request to merge your branch into main."

The PR screen shows:

- Commits made
- Diff of structural changes
- Analysis result summary
- Checks (if any)

**Tutorial instruction:**

> "Review your changes, then merge."

**Success Checks**

- PR created
- Diff opened
- PR merged

---

## Mission 6 — View Main History

**Concept:** Version history as an audit trail

After merge, auto-navigate to main.

**Prompt:**

> "Main now includes your work. View its history."

Show:

- Timeline of commits
- Branch merge point
- Analysis artifact attached to commit

**Final teaching moment:**

> "Every model state is preserved. You can return to any version."

**Success Checks**

- User opens commit history
- User expands at least one previous commit
