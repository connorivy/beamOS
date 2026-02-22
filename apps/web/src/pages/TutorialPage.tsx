import CallSplitRoundedIcon from "@mui/icons-material/CallSplitRounded";
import { Alert, Button } from "@mui/material";
import { useState } from "react";
import { apiClient } from "../api/client";
import { TutorialTour } from "../components/TutorialTour";
import { ModelRevisionEditorPage } from "./ModelRevisionEditorPage";
import { TUTORIAL_PROJECT_ID } from "./ModelsPage";

export const TutorialPage = () => {
  const [projectId, setProjectId] = useState(TUTORIAL_PROJECT_ID);
  const [branchName, setBranchName] = useState("main");
  const [mission, setMission] = useState<"mission1" | "mission2">("mission1");
  const [isForkLoading, setIsForkLoading] = useState(false);
  const [forkError, setForkError] = useState<string | null>(null);

  const handleFork = async () => {
    setIsForkLoading(true);
    setForkError(null);
    try {
      const { data, error } = await apiClient.POST("/api/projects/{projectId}/fork", {
        params: { path: { projectId } },
        body: { name: "Tutorial (fork)" },
      });
      if (error || !data) {
        setForkError("Failed to fork the project. Please try again.");
        return;
      }
      setProjectId(data.id);
      setBranchName("main");
      setMission("mission2");
    } catch {
      setForkError("An unexpected error occurred.");
    } finally {
      setIsForkLoading(false);
    }
  };

  return (
    <>
      <span id="tutorial-project-id" hidden>
        {projectId}
      </span>
      <ModelRevisionEditorPage
        routeStateOverride={{ projectId, branchName }}
        onBranchChange={setBranchName}
        defaultActivePanel="revisionControl"
        revisionControlActions={(
          <>
            {forkError ? <Alert severity="error">{forkError}</Alert> : null}
            {mission === "mission1" ? (
              <Button
                id="tutorial-fork-button"
                variant="contained"
                startIcon={<CallSplitRoundedIcon />}
                onClick={() => void handleFork()}
                disabled={isForkLoading}
                fullWidth
              >
                {isForkLoading ? "Forking…" : "Fork Project"}
              </Button>
            ) : null}
          </>
        )}
      />
      <TutorialTour mission={mission} />
    </>
  );
};
