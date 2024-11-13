import { Button, CircularProgress, FormControl, FormLabel, Link, Stack, TextField } from "@mui/material";
import { useState } from "react";

function LoginForm() {
    const [username, setUsername] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [formDisabled, setFormDisabled] = useState<boolean>(false);

    async function submitAction() {
        let formData = {
            "username": username,
            "password": password
        };

        setFormDisabled(true);

        let response = await fetch("/User/login", {
            method: "post",
            body: JSON.stringify(formData),
            headers: {
                "Content-Type": "application/json"
            }
        });

        setFormDisabled(false);

        if (response.status != 200) {
            return;
        }
    }

    return (
        <Stack sx={{ gap: 5, alignItems: "center" }}>
            <FormControl>
                <FormLabel htmlFor="username">Username</FormLabel>
                <TextField id="username" name="username" value={username} onChange={(event) => setUsername(event.target.value)}/>
            </FormControl>

            <FormControl>
                <FormLabel htmlFor="password">Password</FormLabel>
                <TextField id="password" name="password" type="password" value={password} onChange={(event) => setPassword(event.target.value)}/>
            </FormControl>

            <Link href="/register">New user? Click her to register</Link>

            <Button variant="contained" onClick={submitAction} disabled={formDisabled}>
                Submit
            </Button>

            {formDisabled && <CircularProgress />}
        </Stack>
    );
}

export default LoginForm;