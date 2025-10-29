import type { ReactNode } from "react"
import useMediaQuery from "@mui/material/useMediaQuery"
import SwipeableDrawer from "@mui/material/SwipeableDrawer"
import Box from "@mui/material/Box"
import { styled } from "@mui/material/styles"
import { Global } from "@emotion/react"
import { grey } from "@mui/material/colors"

type ResponsiveSecondarySidebarProps = {
    open: boolean
    onOpen: () => void
    onClose: () => void
    children: ReactNode
}

const drawerBleeding = 32

const Puller = styled('div')(({ theme }) => ({
    width: 30,
    height: 6,
    backgroundColor: grey[300],
    borderRadius: 3,
    position: 'absolute',
    top: 8,
    left: 'calc(50% - 15px)',
    zIndex: 1400,
    ...theme.applyStyles('dark', {
        backgroundColor: grey[900],
    })
}))

const ResponsiveSecondarySidebar = ({ open, onOpen, onClose, children }: ResponsiveSecondarySidebarProps) => {
    const isSmallScreen = useMediaQuery("(max-width:900px)")

    if (isSmallScreen) {
        return (
            <>
                <Global
                    styles={{
                        '.MuiDrawer-root > .MuiPaper-root': {
                            height: `calc(50% - ${String(drawerBleeding)}px)`,
                            overflow: 'visible',
                        },
                    }}
                />
                <SwipeableDrawer
                    anchor="bottom"
                    open={open}
                    onClose={onClose}
                    onOpen={onOpen}
                    swipeAreaWidth={drawerBleeding}
                    disableSwipeToOpen={false}
                    keepMounted
                    slotProps={{
                        paper: {
                            style: { borderTopLeftRadius: 16, borderTopRightRadius: 16, minHeight: 200, overflow: 'visible' },
                        },
                    }}
                >
                    <Box
                        sx={{
                            position: 'absolute',
                            top: -drawerBleeding,
                            borderTopLeftRadius: 8,
                            borderTopRightRadius: 8,
                            visibility: 'visible',
                            right: 0,
                            left: 0,
                        }}
                    >
                        <Puller />
                    </Box>
                    <Box p={2} height="100%" overflow="auto">
                        {children}
                    </Box>
                </SwipeableDrawer>
            </>
        )
    }

    // Large screens: floating sidebar on right with offset
    onOpen()
    return (
        <div className="absolute top-0 right-0 max-h-full w-96 p-2 pointer-events-none">
            <Box
                sx={{
                    width: "100%",
                    height: `100%`,
                    boxShadow: 3,
                    borderRadius: 3,
                    bgcolor: "background.paper",
                    zIndex: 1300,
                    display: open ? "block" : "none",
                    pointerEvents: "auto",
                }}
            >
                <Box height="100%" overflow="auto">
                    {children}
                </Box>
            </Box>
        </div>
    )
}

export default ResponsiveSecondarySidebar
