import type React from "react";
import { useTheme } from "@mui/material/styles";

type DarkPaperProps = {
    children?: React.ReactNode;
    style?: React.CSSProperties;
    className?: string;
}

const DarkPaper: React.FC<DarkPaperProps> = ({ children, style, className }) => {
    const theme = useTheme();
    return (
        <div
            className={className}
            style={{
                borderLeft: "1px solid #222",
                background: theme.palette.background.paper,
                ...style,
            }}
        >
            {children}
        </div>
    );
};

export default DarkPaper;
