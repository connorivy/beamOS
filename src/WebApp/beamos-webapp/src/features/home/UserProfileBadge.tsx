import Avatar from "@mui/material/Avatar"
import Menu from "@mui/material/Menu"
import MenuItem from "@mui/material/MenuItem"
import IconButton from "@mui/material/IconButton"
import { useState } from "react"
import { useAuth } from "../../auth/AuthContext"
import { useNavigate } from "react-router"

export const UserProfileBadge = () => {
  const { user, logout } = useAuth()
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)
  const navigate = useNavigate()

  if (!user) return null

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget)
  }
  const handleMenuClose = () => {
    setAnchorEl(null)
  }
  const handleLogout = () => {
    logout()
    handleMenuClose()
    void navigate("/")
  }

  return (
    <>
      <IconButton
        color="inherit"
        onClick={handleMenuOpen}
        size="small"
        sx={{ ml: 1 }}
      >
        <Avatar src={""} alt={user.email} sx={{ width: 32, height: 32 }}>
          {user.email[0]}
        </Avatar>
      </IconButton>
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
        onClick={handleMenuClose}
        transformOrigin={{ horizontal: "right", vertical: "top" }}
        anchorOrigin={{ horizontal: "right", vertical: "bottom" }}
      >
        <MenuItem
          onClick={() => {
            void navigate("/settings")
          }}
        >
          Settings
        </MenuItem>
        <MenuItem onClick={handleLogout}>Logout</MenuItem>
      </Menu>
    </>
  )
}
