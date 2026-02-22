import { useState } from "react";
import Joyride, { type CallBackProps, STATUS } from "react-joyride";

const MISSION_1_STEPS = [
  {
    target: "body",
    placement: "center" as const,
    title: "Mission 1 — Fork the Model",
    content: "Fork this model to create your own working copy.",
    disableBeacon: true,
  },
  {
    target: "#tutorial-fork-button",
    title: "Fork the Project",
    content:
      'Click "Fork Project" to create your own copy. Every change you make will be saved as a version.',
    disableBeacon: true,
    spotlightClicks: true,
  },
];

const MISSION_2_STEPS = [
  {
    target: "body",
    placement: "center" as const,
    title: "Mission 2 — Create a Branch",
    content: "Create a branch to make your changes.",
    disableBeacon: true,
  },
  {
    target: "#tutorial-create-branch-button",
    title: "Create Branch",
    content: 'Click "+" to create a branch from your base branch.',
    disableBeacon: true,
    spotlightClicks: true,
  },
];

export const TutorialTour = ({ mission = "mission1" }: { mission?: "mission1" | "mission2" }) => {
  const [run, setRun] = useState(true);

  const handleCallback = (data: CallBackProps) => {
    const { status } = data;
    if (status === STATUS.FINISHED || status === STATUS.SKIPPED) {
      setRun(false);
    }
  };

  return (
    <Joyride
      steps={mission === "mission2" ? MISSION_2_STEPS : MISSION_1_STEPS}
      run={run}
      continuous
      showSkipButton
      disableScrolling
      styles={{
        options: {
          zIndex: 10000,
        },
      }}
      callback={handleCallback}
    />
  );
};
