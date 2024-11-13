import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import RegisterForm from './forms/register-form.tsx'
import JournalViewer from './journal-viewer.tsx'

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<App />} />
                <Route path="register" element={<RegisterForm />} />
                <Route path="/view/:journalIdParam" element={<JournalViewer />} />
            </Routes>
        </BrowserRouter>
    </StrictMode>
)
