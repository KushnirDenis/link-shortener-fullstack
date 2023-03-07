import axios from "axios";
import i18n from "./i18n";

export const apiV1 = axios.create({
    baseURL: "http://localhost:4000/api/v1",
})