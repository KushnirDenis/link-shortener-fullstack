import 'bootstrap/dist/css/bootstrap.min.css';
import React from "react";
import {Route, Routes} from "react-router-dom";
import Layout from "./components/Layout";
import LoginPage from "./pages/LoginPage";

function App() {

    return (
        <Routes>
            <Route path={"/"} element={<Layout/>}>
                <Route index></Route>
                <Route path={"auth/login"} element={<LoginPage/>}></Route>
            </Route>
        </Routes>
    )
}

export default App
