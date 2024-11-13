import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import JournalEntryView from "./interfaces/journal-entry-view";
import { useCookies } from "react-cookie";
import { CircularProgress, Stack } from "@mui/material";

function JournalViewer() {
    const [cookies] = useCookies(["JWT", "Username", "UserKey"]);
    const [journalView, setJournalView] = useState<JournalEntryView>();
    const navigate = useNavigate();
    let { journalIdParam } = useParams();

    useEffect(() => {
        if (journalView === undefined) {
            getJournalView();
        }
    });

    async function getJournalView() {
        let journalId = parseInt(journalIdParam);

        let journalViewData = {
            "username": cookies["Username"],
            "journalId": journalId,
            "encryptedkey": cookies["UserKey"]
        };

        let response = await fetch(`/Journal/view/${journalId}`, {
            method: "post",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + cookies["JWT"]
            },
            body: JSON.stringify(journalViewData)
        });

        if (response.status != 200) {
            navigate("/");
            return;
        }

        setJournalView(JSON.parse(await response.text()));
    }

    if (journalView === undefined) {
        return <CircularProgress />;
    }

    return (
        <Stack>
            <p>{journalView.title}</p>
            <p>{journalView.createdOn.toDateString()}</p>
            <p>{journalView.body}</p>
        </Stack>
    );
}

export default JournalViewer;