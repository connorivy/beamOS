import { useState } from "react"
import Dialog from "@mui/material/Dialog"
import DialogContent from "@mui/material/DialogContent"
import DialogActions from "@mui/material/DialogActions"
import Button from "@mui/material/Button"
import Stepper from "@mui/material/Stepper"
import Step from "@mui/material/Step"
import StepLabel from "@mui/material/StepLabel"
import Typography from "@mui/material/Typography"
import Box from "@mui/material/Box"

type TutorialWelcomeDialogProps = {
  open: boolean
  onClose: () => void
}

const steps = [
  {
    label: "Welcome",
    title: "Welcome to BeamOS!",
    content: (
      <>
        <Typography variant="body1" sx={{ mb: 2 }}>
          This interactive tutorial will help you learn the basics of structural
          analysis with BeamOS.
        </Typography>
        <Typography variant="body1" sx={{ mb: 2 }}>
          You'll learn how to:
        </Typography>
        <ul style={{ marginTop: 0 }}>
          <li>Create nodes and elements</li>
          <li>Apply loads and constraints</li>
          <li>Run structural analysis</li>
          <li>View and interpret results</li>
        </ul>
      </>
    ),
  },
  {
    label: "Creating Nodes",
    title: "Creating Nodes",
    content: (
      <Typography variant="body1">
        Learn how to create structural nodes in your model.
      </Typography>
    ),
  },
  {
    label: "Adding Elements",
    title: "Adding Elements",
    content: (
      <Typography variant="body1">
        Connect nodes with structural elements.
      </Typography>
    ),
  },
  {
    label: "Applying Loads",
    title: "Applying Loads",
    content: (
      <Typography variant="body1">
        Add loads and constraints to your structure.
      </Typography>
    ),
  },
  {
    label: "Running Analysis",
    title: "Running Analysis",
    content: (
      <Typography variant="body1">
        Execute structural analysis on your model.
      </Typography>
    ),
  },
  {
    label: "Viewing Results",
    title: "Viewing Results",
    content: (
      <Typography variant="body1">
        Interpret and visualize the analysis results.
      </Typography>
    ),
  },
]

const TutorialWelcomeDialog: React.FC<TutorialWelcomeDialogProps> = ({
  open,
  onClose,
}) => {
  const [activeStep, setActiveStep] = useState(0)

  const handleNext = () => {
    if (activeStep < steps.length - 1) {
      setActiveStep(prevStep => prevStep + 1)
    }
  }

  const handleBack = () => {
    if (activeStep > 0) {
      setActiveStep(prevStep => prevStep - 1)
    }
  }

  const handleClose = () => {
    setActiveStep(0)
    onClose()
  }

  const currentStep = steps[activeStep]

  return (
    <Dialog
      open={open}
      onClose={handleClose}
      maxWidth="sm"
      fullWidth
      slotProps={{
        paper: {
          sx: {
            minHeight: "400px",
          },
        },
      }}
    >
      <DialogContent>
        <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
          <Typography variant="h6">{currentStep.title}</Typography>
          <Box>{currentStep.content}</Box>
          <Stepper activeStep={activeStep} alternativeLabel>
            {steps.map(step => (
              <Step key={step.label}>
                <StepLabel>{step.label}</StepLabel>
              </Step>
            ))}
          </Stepper>
        </Box>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2 }}>
        <Button onClick={handleClose} aria-label="close">
          Close
        </Button>
        {activeStep > 0 && (
          <Button onClick={handleBack} aria-label="back">
            Back
          </Button>
        )}
        {activeStep < steps.length - 1 && (
          <Button onClick={handleNext} variant="contained" aria-label="next">
            Next
          </Button>
        )}
      </DialogActions>
    </Dialog>
  )
}

export default TutorialWelcomeDialog
