import 'bootstrap/dist/css/bootstrap.min.css';
import React from "react";
import {Route, Routes} from "react-router-dom";
import Layout from "./components/Layout";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import LinksPage from "./pages/LinksPage";
import {useUserStore} from "./stores/UserStore";
import ProtectedRoute from "./components/ProtectedRoute";

function App() {

    const user = useUserStore(state => state.user);

    return (
        <Routes>
            <Route path={"/"} element={<Layout/>}>
                <Route index></Route>
                <Route path={"links"} element={
                    <ProtectedRoute>
                        <LinksPage/>
                    </ProtectedRoute>
                }/>
                <Route path={"auth/login"} element={<LoginPage/>}/>
                <Route path={"auth/register"} element={<RegisterPage/>}/>
            </Route>
        </Routes>
    )
}

export default App
