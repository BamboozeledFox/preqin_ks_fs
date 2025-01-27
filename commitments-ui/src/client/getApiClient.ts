import axios from "axios";

function getApiClient() {
    const baseUrl = import.meta.env.VITE_BASE_URL;
    return axios.create({
        baseURL: baseUrl
    });
}

export default getApiClient;