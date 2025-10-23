import { Box } from "@mui/material"
import { ApiTokenDisplay } from "./ApiTokenDisplay"
import ApiUsageDisplay from "./ApiUsageDisplay"

export default function DeveloperSettingsTab() {
  return (
    <Box>
      <ApiTokenDisplay />
      <ApiUsageDisplay />
    </Box>
  )
}
