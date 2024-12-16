import axios from 'axios';
import Cookies from 'js-cookie';

const API_URL = 'https://localhost:7417/auth/login';
const TOKEN_COOKIE = 'auth_token';
const COOKIE_OPTIONS = {
    expires: 7,
    secure: false,
    sameSite: 'lax',
    path: '/',
    domain: 'localhost'
};

const authAxios = axios.create({
    withCredentials: true,
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    }
});

export const authService = {
    login: async (username, password) => {
        try {
            const response = await authAxios.post(API_URL, {
                username,
                password
            });
            
            if (response.data?.token) {
                Cookies.set(TOKEN_COOKIE, response.data.token, COOKIE_OPTIONS);

                return response.data;
            }
            throw new Error('No token received');
        } catch (error) {
            console.error('Login error details:', error);
            if (error.response) {
                // The request was made and the server responded with a status code
                // that falls out of the range of 2xx
                throw error.response.data;
            } else if (error.request) {
                throw new Error('No response received from server');
            } else {
                throw new Error('Error setting up the request');
            }
        }
    },
    logout: () => {
        Cookies.remove(TOKEN_COOKIE);
    },
    getToken: () => {
        return Cookies.get(TOKEN_COOKIE);
    },
    isAuthenticated: () => {
        const token = Cookies.get(TOKEN_COOKIE);
        return !!token;
    }
};