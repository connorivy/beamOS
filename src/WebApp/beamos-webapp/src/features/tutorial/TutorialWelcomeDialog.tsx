import { useState } from "react"
import Dialog from "@mui/material/Dialog"
import DialogContent from "@mui/material/DialogContent"
import DialogActions from "@mui/material/DialogActions"
import Button from "@mui/material/Button"
import Stepper from "@mui/material/Stepper"
import Step from "@mui/material/Step"
import Typography from "@mui/material/Typography"
import Box from "@mui/material/Box"
import { StepButton, Tooltip } from "@mui/material"
import type { IStructuralAnalysisApiClientV1 } from "../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"

type TutorialWelcomeDialogProps = {
  open: boolean
  onClose: () => void
  modelId: string | null
  apiClient: IStructuralAnalysisApiClientV1
}

type StepContentProps = {
  isImporting: boolean
  importCompleted: boolean
  onImport: () => void
  modelId: string | null
}

const getSteps = (props: StepContentProps) => [
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
    label: "BIM-First Approach",
    title: "BIM-First Approach",
    content: (
      <>
        <Typography variant="body1" sx={{ mb: 2 }}>
          BeamOS is designed to be <strong>BIM-first</strong>. This means that your BIM model is the source of truth for your BeamOS geometry.
        </Typography>
        <Typography variant="body1" sx={{ mb: 2 }}>
          You are not supposed to do your geometry modeling in BeamOS. The only modeling that you do in BeamOS is to make slight adjustments for connectivity.
        </Typography>
        <Typography variant="body1" sx={{ mb: 2 }}>
          A Revit plugin is in development that users will be able to use to send their data into BeamOS. For the purpose of this tutorial, you can import some sample data below.
        </Typography>
        <Box sx={{ display: 'flex', justifyContent: 'center', my: 2 }}>
          <Button 
            onClick={props.onImport} 
            variant="contained" 
            color="primary"
            disabled={props.isImporting || !props.modelId}
            aria-label="import"
          >
            {props.isImporting ? "Importing..." : "Import Sample Data"}
          </Button>
        </Box>
        {props.importCompleted && (
          <Typography variant="body2" color="success.main" sx={{ textAlign: 'center' }}>
            ✓ Sample data imported successfully
          </Typography>
        )}
      </>
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
  modelId,
  apiClient,
}) => {
  const [activeStep, setActiveStep] = useState(0)
  const [isImporting, setIsImporting] = useState(false)
  const [importCompleted, setImportCompleted] = useState(false)

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

  const handleImportSampleData = async () => {
    if (!modelId) {
      console.error("Model ID is not available")
      return
    }

    setIsImporting(true)
    setImportCompleted(false)
    try {
      console.log("Starting import of Kassimali_Example3_8 data for model:", modelId)
      
      // Create a model proposal with all the data instead of adding directly to the model
      await apiClient.createModelProposal(modelId, {
        description: "Tutorial sample data - Kassimali Example 3.8",
        createNodeProposals: [
          {
            id: 1,
            locationPoint: { x: 12, y: 16, z: 0, lengthUnit: 2 }, // Foot = 2
            restraint: { canTranslateAlongX: true, canTranslateAlongY: true, canTranslateAlongZ: false, canRotateAboutX: false, canRotateAboutY: false, canRotateAboutZ: true },
          },
          {
            id: 2,
            locationPoint: { x: 0, y: 0, z: 0, lengthUnit: 2 },
            restraint: { canTranslateAlongX: false, canTranslateAlongY: false, canTranslateAlongZ: false, canRotateAboutX: false, canRotateAboutY: false, canRotateAboutZ: true },
          },
          {
            id: 3,
            locationPoint: { x: 12, y: 0, z: 0, lengthUnit: 2 },
            restraint: { canTranslateAlongX: false, canTranslateAlongY: false, canTranslateAlongZ: false, canRotateAboutX: false, canRotateAboutY: false, canRotateAboutZ: true },
          },
          {
            id: 4,
            locationPoint: { x: 24, y: 0, z: 0, lengthUnit: 2 },
            restraint: { canTranslateAlongX: false, canTranslateAlongY: false, canTranslateAlongZ: false, canRotateAboutX: false, canRotateAboutY: false, canRotateAboutZ: true },
          },
        ],
        createMaterialProposals: [
          {
            id: 992,
            modulusOfElasticity: 29000,
            modulusOfRigidity: 1,
            pressureUnit: 4, // KilopoundForcePerSquareInch = 4
          },
        ],
        createSectionProfileProposals: [
          {
            id: 8,
            name: "8",
            lengthUnit: 1, // Inch = 1
            area: 8,
            strongAxisMomentOfInertia: 1,
            weakAxisMomentOfInertia: 1,
            polarMomentOfInertia: 1,
            strongAxisShearArea: 1,
            weakAxisShearArea: 1,
            strongAxisPlasticSectionModulus: 1,
            weakAxisPlasticSectionModulus: 1,
          },
          {
            id: 6,
            name: "6",
            lengthUnit: 1,
            area: 6,
            strongAxisMomentOfInertia: 1,
            weakAxisMomentOfInertia: 1,
            polarMomentOfInertia: 1,
            strongAxisShearArea: 1,
            weakAxisShearArea: 1,
            strongAxisPlasticSectionModulus: 1,
            weakAxisPlasticSectionModulus: 1,
          },
        ],
        loadCaseProposals: [
          { id: 1, name: "Load Case 1" },
        ],
        loadCombinationProposals: [
          { id: 1, loadCaseFactors: { "1": 1.0 } },
          { id: 2, loadCaseFactors: { "1": 1.0 } },
        ],
        pointLoadProposals: [
          {
            id: 1,
            nodeId: 1,
            loadCaseId: 1,
            force: { value: 150, unit: 1 }, // KilopoundForce = 1
            direction: { x: 1, y: 0, z: 0 },
          },
          {
            id: 2,
            nodeId: 1,
            loadCaseId: 1,
            force: { value: 300, unit: 1 },
            direction: { x: 0, y: -1, z: 0 },
          },
        ],
        createElement1dProposals: [
          {
            id: 1,
            startNodeId: { proposedId: 2, existingId: undefined },
            endNodeId: { proposedId: 1, existingId: undefined },
            materialId: { proposedId: 992, existingId: undefined },
            sectionProfileId: { proposedId: 8, existingId: undefined },
          },
          {
            id: 2,
            startNodeId: { proposedId: 3, existingId: undefined },
            endNodeId: { proposedId: 1, existingId: undefined },
            materialId: { proposedId: 992, existingId: undefined },
            sectionProfileId: { proposedId: 6, existingId: undefined },
          },
          {
            id: 3,
            startNodeId: { proposedId: 4, existingId: undefined },
            endNodeId: { proposedId: 1, existingId: undefined },
            materialId: { proposedId: 992, existingId: undefined },
            sectionProfileId: { proposedId: 8, existingId: undefined },
          },
        ],
      })

      console.log("Sample data imported successfully as model proposal")
      setImportCompleted(true)
    } catch (error) {
      console.error("Error importing sample data:", error)
    } finally {
      setIsImporting(false)
    }
  }

  const steps = getSteps({
    isImporting,
    importCompleted,
    onImport: handleImportSampleData,
    modelId,
  })

  const currentStep = steps[activeStep]

  // Determine if the next button should be disabled
  // On step 1 (BIM-First), disable next until import is complete
  const isNextDisabled = activeStep === 1 && !importCompleted

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
          <Stepper activeStep={activeStep} alternativeLabel nonLinear>
            {steps.map(step => (
              <Step key={step.label}>
                <StepButton onClick={() => setActiveStep(steps.indexOf(step))}>
                  {step.label}
                </StepButton>
                {/* <StepLabel>{step.label}</StepLabel> */}
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
          <Tooltip 
            title={isNextDisabled ? "Please import sample data before proceeding" : ""}
            arrow
          >
            <span>
              <Button 
                onClick={handleNext} 
                variant="contained" 
                aria-label="next"
                disabled={isNextDisabled}
              >
                Next
              </Button>
            </span>
          </Tooltip>
        )}
      </DialogActions>
    </Dialog>
  )
}

export default TutorialWelcomeDialog
