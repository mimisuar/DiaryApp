import { useState } from "react";
import { useParams } from "react-router-dom";
import JournalEntryView from "./interfaces/journal-entry-view";

function JournalViewer() {
    const [journalView, setJournalView] = useState<JournalEntryView>();
    let { journalId } = useParams();

    useEffect(() => {
        if (journalView === undefined) {

        }
    });

    async function getJournalView() {

    }

    return (
        <></>
    );
}

export default JournalViewer;