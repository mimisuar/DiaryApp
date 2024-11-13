import { Button, CircularProgress, Stack } from "@mui/material";
import { useCookies } from "react-cookie";
import JournalEntry from "./interfaces/journal-entry";
import { useEffect, useState } from "react";
import JournalEditor from "./forms/journal-editor";

function Dashboard() {
    const [cookies] = useCookies(["JWT", "Username", "UserKey"]);
    const [journals, setJournals] = useState<JournalEntry[]>();

    useEffect(() => {
        if (journals === undefined) {
            getUserJournals();
        }
    });

    async function logoutUser() {
        await fetch("/User/logout", {
            method: "post",
            headers: {
                "Authorization": "Bearer " + cookies["JWT"]
            }
        });
    }

    async function getUserJournals() {
        let response = await fetch(`/Journal?username=${cookies["Username"]}`, {
            method: "post",
            headers: {
                "Authorization": "Bearer " + cookies["JWT"]
            }
        });

        if (response.status != 200) {
            return;
        }

        let newJournals: JournalEntry[] = JSON.parse(await response.text());
        setJournals(newJournals);
    }

    let body: JSX.Element;
    if (journals === undefined) {
        body = <CircularProgress />
    }
    else if (journals.length === 0) {
        body = <p>No journals found. Make one!</p>
    }
    else {
        body = <p>This is where my journals go</p>
    }

    return (
        <Stack alignItems="center" gap={5}>
            <p>Welcome, {cookies["Username"]}!</p>

            <JournalEditor />

            {body}

            <Button variant="contained" onClick={logoutUser}>Logout</Button>
        </Stack>
    );
}

export default Dashboard;