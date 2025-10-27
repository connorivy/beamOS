import type React from "react";
import { useState } from "react";
import Drawer from '@mui/material/Drawer';
import useMediaQuery from '@mui/material/useMediaQuery';
import AppBarMain from "../components/AppBarMain";
import ApartmentIcon from '@mui/icons-material/Apartment';
import FunctionsIcon from '@mui/icons-material/Functions';

const drawerWidth = 220;

import SelectionInfo from "../features/editors/selection-info/SelectionInfo";
import { IconButton, Tooltip } from "@mui/material";
import ResultsInfo from "../features/results-viewing/ResultsInfo";

const ResponsiveIconSidebarLayout: React.FC<{ canvasId: string, children?: React.ReactNode }> = ({ canvasId, children }) => {
    const [sidebarOpen, setSidebarOpen] = useState(false);
    const [selectedSidebar, setSelectedSidebar] = useState<string>('physical');
    const isMobile = useMediaQuery('(max-width:900px)');

    // Icon sidebar items
    const icons = [
        { id: "physical", label: "Physical Model", icon: <ApartmentIcon /> },
        { id: "analytical", label: "Analytical Model", icon: <FunctionsIcon /> },
    ];

    // Handle icon click for sidebar selection and mobile toggle
    const handleIconClick = (id: string) => {
        setSelectedSidebar(id);
        if (isMobile) {
            setSidebarOpen(true);
        }
    };

    // Keyboard navigation for sidebar toggle
    const handleKeyDown = (event: React.KeyboardEvent<HTMLButtonElement>) => {
        if (event.key === 'Enter' || event.key === ' ') {
            handleIconClick('nav');
        }
    };

    // Sidebar content for each icon
    const sidebarContents: Record<string, React.ReactNode> = {
        physical: <SelectionInfo canvasId={canvasId} />,
        analytical: <ResultsInfo canvasId={canvasId} />,
    };

    return (
        <div className="flex flex-col h-screen w-screen">
            <AppBarMain />
            <div className="flex flex-row h-full w-full">
                {/* Icon Sidebar with highlight and separator */}
                <div className="flex flex-col items-center" style={{ minWidth: 56, borderRight: '1px solid #222', background: '#18181c', paddingTop: '8px' }}>
                    {icons.map(icon => (
                        <Tooltip key={icon.id} title={icon.label} placement="right" arrow>
                            <IconButton
                                onClick={() => { handleIconClick(icon.id); }}
                                onKeyDown={handleKeyDown}
                                aria-label={icon.label}
                                // size="large"
                                sx={{
                                    color: selectedSidebar === icon.id ? 'primary.contrastText' : 'grey.400',
                                    backgroundColor: selectedSidebar === icon.id ? 'primary.main' : 'inherit',
                                    boxShadow: selectedSidebar === icon.id ? 2 : 0,
                                    marginBottom: 2,
                                    width: 48,
                                    height: 48,
                                    '&:hover': {
                                        backgroundColor: selectedSidebar === icon.id ? 'primary.dark' : 'grey.900',
                                    },
                                }}
                            >
                                {icon.icon}
                            </IconButton>
                        </Tooltip>
                    ))}
                </div>
                {/* Main Sidebar Content with left shadow for separation */}
                {isMobile ? (
                    <Drawer
                        anchor="left"
                        open={sidebarOpen}
                        onClose={() => { setSidebarOpen(false); }}
                        ModalProps={{ keepMounted: true }}
                        sx={{
                            '& .MuiDrawer-paper': {
                                width: drawerWidth,
                                boxShadow: '4px 0 12px rgba(0,0,0,0.12)',
                                borderLeft: '1px solid #222',
                            },
                            width: drawerWidth,
                        }}
                        role="dialog"
                        aria-modal="true"
                        aria-label="Sidebar content"
                    >
                        {sidebarContents[selectedSidebar]}
                    </Drawer>
                ) : (
                    <div className="w-96 shadow-lg" style={{ boxShadow: '4px 0 12px rgba(0,0,0,0.12)', borderLeft: '1px solid #222', background: '#18181c' }}>
                        {sidebarContents[selectedSidebar]}
                    </div>
                )}
                {/* Main Content */}
                <div className="h-full w-full relative">
                    {children}
                </div>
            </div>
        </div>
    );
};

export default ResponsiveIconSidebarLayout;
