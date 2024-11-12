import { Button, FormControl, FormLabel, Stack, TextField } from "@mui/material";
import { useState } from "react";

function LoginForm() {
    const [username, setUsername] = useState<string>("");
    const [password, setPassword] = useState<string>("");

    async function submitAction() {
        let formData = {
            "username": username,
            "password": password
        };

        let response = await fetch("/User/login", {
            method: "post",
            body: JSON.stringify(formData)
        });

        if (response.status != 200) {
            return;
        }
    }

    return (
        <Stack>
            <FormControl sx={{ gap: 5 }}>
                <FormLabel htmlFor="username">Username</FormLabel>
                <TextField id="username" name="username" value={username} onChange={(event) => setUsername(event.target.value)}/>
            </FormControl>

            <FormControl>
                <FormLabel htmlFor="password">Password</FormLabel>
                <TextField id="password" name="password" type="password" value={password} onChange={(event) => setPassword(event.target.value)}/>
            </FormControl>

            <Button variant="contained" onClick={submitAction}>
                Submit
            </Button>
        </Stack>
    );
}

export default LoginForm;