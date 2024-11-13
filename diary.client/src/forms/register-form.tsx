import { Button, CircularProgress, FormControl, FormLabel, Stack, TextField } from "@mui/material";
import { useState } from "react";

function RegisterForm() {
    const [username, setUsername] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [confirm, setConfirm] = useState<string>("");
    const [formDisabled, setFormDisabled] = useState<boolean>(false);

    async function submitAction() {
        if (password !== confirm) {
            return;
        }

        let formData = {
            "username": username,
            "password": password
        };

        setFormDisabled(true);

        let response = await fetch("/User/register", {
            method: "post",
            body: JSON.stringify(formData),
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (response.status != 200) {
            setFormDisabled(false);
            return;
        }

        response = await fetch("/User/login", {
            method: "post",
            body: JSON.stringify(formData),
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (response.status != 200) {
            setFormDisabled(false);
            return;
        }

        setFormDisabled(false);
    }

    return (
        <Stack sx={{ gap: 5, alignItems: "center" }}>
            <FormControl>
                <FormLabel htmlFor="username">Username</FormLabel>
                <TextField id="username" name="username" value={username} onChange={(event) => setUsername(event.target.value)} />
            </FormControl>

            <FormControl>
                <FormLabel htmlFor="password">Password</FormLabel>
                <TextField id="password" name="password" type="password" value={password} onChange={(event) => setPassword(event.target.value)} />
            </FormControl>

            <FormControl>
                <FormLabel htmlFor="confirm">Confirm password</FormLabel>
                <TextField id="confirm" name="confirm" type="password" value={confirm} onChange={(event) => setConfirm(event.target.value)} />
            </FormControl>

            <Button variant="contained" onClick={submitAction} disabled={formDisabled}>
                Register
            </Button>

            {formDisabled && <CircularProgress />}
        </Stack>
    );
}

export default RegisterForm;