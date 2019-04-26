import axios from 'axios';

export function createClient() {
    let client = axios.create({
        baseURL: process.env.REACT_APP_API_ENDPOINT
    });

    return client;
}