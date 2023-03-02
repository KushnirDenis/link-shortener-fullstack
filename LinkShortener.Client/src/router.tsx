import {createBrowserRouter, Navigate} from "react-router-dom";
import LoginPage from "./pages/LoginPage";
import React from "react";
import RegisterPage from "./pages/RegisterPage";
import Layout from "./components/Layout";
import App from "./App";

export const Router = createBrowserRouter([
    {
        path: "/",
        element: <Layout />,
        children: [
            {
                index: true,
                element: <App />
            },
            {
                path: "auth/login",
                element: <LoginPage/>
            },
            {
                path: "auth/register",
                element: <RegisterPage/>
            },
        ],
    },
    {
        path: "*",
        element: <Navigate to={"/"}/>
    },
]);