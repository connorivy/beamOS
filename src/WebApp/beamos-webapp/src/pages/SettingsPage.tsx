
import { Tabs, Tab, Box, Paper } from "@mui/material";
import { useState } from "react";
import UserSettingsTab from "./UserSettingsTab";
import DeveloperSettingsTab from "./DeveloperSettingsTab";


const tabConfig = [
    { label: "User" },
    { label: "Developer" },
];

export default function SettingsPage() {
    const [tabIdx, setTabIdx] = useState(0);

    return (
        <Box sx={{ maxWidth: 700, mx: "auto", mt: 6 }}>
            {/* <Paper elevation={3} sx={{ p: 3, bgcolor: "background.paper" }}> */}
            <Tabs
                value={tabIdx}
                onChange={(_, idx) => { setTabIdx(Number(idx)); }}
                aria-label="settings tabs"
            >
                {tabConfig.map(tab => (
                    <Tab key={tab.label} label={tab.label} />
                ))}
            </Tabs>
            <Box sx={{ mt: 3 }}>
                {tabIdx === 0 && <UserSettingsTab />}
                {tabIdx === 1 && <DeveloperSettingsTab />}
            </Box>
            {/* </Paper> */}
        </Box>
    );
}
