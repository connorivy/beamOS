import AppBar from "@mui/material/AppBar"
import Box from "@mui/material/Box"
import Toolbar from "@mui/material/Toolbar"
import Typography from "@mui/material/Typography"
import Button from "@mui/material/Button"
import IconButton from "@mui/material/IconButton"
import GitHubIcon from "@mui/icons-material/GitHub"
import MenuIcon from "@mui/icons-material/Menu"
import { useNavigate } from "react-router"
import { useAuth } from "../auth/AuthContext"
import { UserProfileBadge } from "../features/home/UserProfileBadge"
import type { JSX } from "react"
import useMediaQuery from "@mui/material/useMediaQuery"
import { useTheme } from "@mui/material/styles"

type AppBarMainProps = {
  onSidebarToggle?: () => void
}

export const AppBarMain = ({
  onSidebarToggle,
}: AppBarMainProps): JSX.Element => {
  const navigate = useNavigate()
  const { user } = useAuth()
  const theme = useTheme()
  const isSmallScreen = useMediaQuery(theme.breakpoints.down("md"))

  return (
    <AppBar position="relative" sx={{ display: "flex", boxShadow: 0 }}>
      <Toolbar sx={{ justifyContent: "space-between" }}>
        <Box sx={{ display: "flex", alignItems: "center" }}>
          {isSmallScreen && onSidebarToggle && (
            <IconButton
              color="inherit"
              edge="start"
              onClick={onSidebarToggle}
              sx={{ mr: 2 }}
            >
              <MenuIcon />
            </IconButton>
          )}
          <img
            src="https://raw.githubusercontent.com/Loopple/loopple-public-assets/main/motion-tailwind/img/logos/logo-1.png"
            alt="beamOS logo"
            style={{ height: 32, marginRight: 12 }}
          />
          <Typography variant="h6" sx={{ fontWeight: 700, letterSpacing: 2 }}>
            beamOS
          </Typography>
        </Box>
        <Box>
          <Button
            color="inherit"
            href="/reliability"
            sx={{ textTransform: "none", fontWeight: 600 }}
          >
            Reliability
          </Button>
          <Button
            color="inherit"
            href="/models"
            sx={{ textTransform: "none", fontWeight: 600 }}
          >
            Models
          </Button>
          <IconButton
            color="inherit"
            href="https://github.com/connorivy/beamOS"
            target="_blank"
          >
            <GitHubIcon />
          </IconButton>
          {user ? (
            <UserProfileBadge />
          ) : (
            <Button
              color="inherit"
              sx={{ textTransform: "none", fontWeight: 600 }}
              onClick={() => {
                void navigate("/login")
              }}
            >
              Login
            </Button>
          )}
        </Box>
      </Toolbar>
    </AppBar>
  )
}

export default AppBarMain
