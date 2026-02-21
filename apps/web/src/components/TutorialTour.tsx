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

export const TutorialTour = () => {
  const [run, setRun] = useState(true);

  const handleCallback = (data: CallBackProps) => {
    const { status } = data;
    if (status === STATUS.FINISHED || status === STATUS.SKIPPED) {
      setRun(false);
    }
  };

  return (
    <Joyride
      steps={MISSION_1_STEPS}
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
