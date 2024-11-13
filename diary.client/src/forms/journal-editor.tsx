import { Button, CircularProgress, FormControl, FormLabel, Stack, TextField } from "@mui/material";
import { useState } from "react";
import { useCookies } from "react-cookie";

function JournalEditor() {
    const [cookies] = useCookies(["Username", "JWT", "UserKey"]);
    const [title, setTitle] = useState<string>("");
    const [body, setBody] = useState<string>("");

    const [formDisabled, setFormDisabled] = useState<boolean>(false);

    async function submitBody() {
        let formData = {
            "username": cookies["Username"],
            "title": title,
            "body": body,
            "encryptedkey": cookies["UserKey"]
        };

        setFormDisabled(true);

        let response = await fetch("/Journal/create", {
            method: "post",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + cookies["JWT"]
            },
            body: JSON.stringify(formData)
        });

        setFormDisabled(false);

        if (response.status !== 200) {
            return;
        }
    }

    return (
        <Stack gap={3}>
            <FormControl>
                <FormLabel htmlFor="title">Title</FormLabel>
                <TextField id="title" name="title" value={title} onChange={(event) => setTitle(event.target.value) } />
            </FormControl>

            <FormControl>
                <FormLabel htmlFor="body">Body</FormLabel>
                <TextField id="body" name="body" value={body} onChange={(event) => setBody(event.target.value)} multiline rows={4} />
            </FormControl>

            {formDisabled && <CircularProgress />}

            <Button disabled={formDisabled} variant="contained" type="submit" onClick={submitBody}>Create</Button>
        </Stack>
    );
}

export default JournalEditor;