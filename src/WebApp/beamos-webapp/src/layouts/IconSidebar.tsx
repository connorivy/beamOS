import type React from "react";
import IconButton from '@mui/material/IconButton';
import Box from '@mui/material/Box';

const iconBarWidth = 56;

export type IconSidebarProps = {
    icons: { id: string; label: string; icon: React.ReactNode }[];
    onIconClick: (id: string) => void;
    handleKeyDown?: (event: React.KeyboardEvent<HTMLButtonElement>) => void;
};

const IconSidebar: React.FC<IconSidebarProps> = ({ icons, onIconClick, handleKeyDown }) => (
    <Box
        sx={{
            width: iconBarWidth,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            zIndex: 2,
            pt: 1,
        }}
    >
        {icons.map((icon) => (
            <IconButton
                key={icon.id}
                aria-label={icon.label}
                color="inherit"
                sx={{ mb: 2, fontSize: '1.5rem' }}
                onClick={() => { onIconClick(icon.id); }}
                tabIndex={0}
                onKeyDown={icon.id === 'nav' && handleKeyDown ? handleKeyDown : undefined}
            >
                {icon.icon}
            </IconButton>
        ))}
    </Box>
);

export default IconSidebar;
