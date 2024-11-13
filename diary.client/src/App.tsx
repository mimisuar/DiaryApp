import { useCookies } from 'react-cookie';
import './App.css';

import Dashboard from './dashboard';
import LoginForm from './forms/login-form';

function App() {
    const [cookies] = useCookies(["UserKey", "JWT", "Username"]);

    if (cookies["JWT"] === undefined) {
        return <LoginForm />;
    }

    return <Dashboard />;
}

export default App;