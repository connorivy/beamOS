import type React from "react"
import Drawer from "@mui/material/Drawer"
import useMediaQuery from "@mui/material/useMediaQuery"
import { useTheme } from "@mui/material/styles"

type SidebarMainProps = {
  open: boolean
  onClose: () => void
  drawerWidth?: number
  children?: React.ReactNode
}

const SidebarMain: React.FC<SidebarMainProps> = ({
  open,
  onClose,
  drawerWidth = 240,
  children,
}) => {
  const theme = useTheme()
  const isLargeScreen = useMediaQuery(theme.breakpoints.up("md"))

  const appBarHeight = 64 // Default MUI AppBar height
  const appBarHeightPx = String(appBarHeight)
  return (
    <Drawer
      variant={isLargeScreen ? "persistent" : "temporary"}
      open={open || isLargeScreen}
      onClose={onClose}
      ModalProps={{ keepMounted: true }}
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        backgroundColor: "background.paper",
        // "& .MuiDrawer-paper": {
        //   width: drawerWidth,
        //   boxSizing: "border-box",
        //   ...(isLargeScreen
        //     ? {
        //       top: `${appBarHeightPx}px`,
        //       height: `calc(100% - ${appBarHeightPx}px)`,
        //     }
        //     : {}),
        // },
      }}
    >
      {children}
    </Drawer>
  )
}

export default SidebarMain
