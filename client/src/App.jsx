import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import LoginForm from "./components/auth/LoginForm";
import RegisterForm from "./components/auth/RegisterForm";
import Dashboard from "./pages/Dashboard";
import Home from "./pages/Home";
import "./App.css";
import BookAdmin from './components/admin/AdminPage' 

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/register" element={<RegisterForm />} />
        <Route path="/login" element={<LoginForm />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/bookadmin" element={<BookAdmin />} />



      </Routes>
    </BrowserRouter>
  );
}

export default App;