import axios from 'axios';
import { authService } from '../core/auth/services/AuthService';

const axiosInstance = axios.create({
    baseURL: 'https://localhost:7417',
    withCredentials: true,  // Important for CORS
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    }
});

axiosInstance.interceptors.request.use(
    (config) => {
        const token = authService.getToken();
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        if (error.response?.status === 401) {
          authService.logout();
          window.location.href = '/';
        }
    }
);

axiosInstance.interceptors.response.use(
    (response) => response,
    (error) => {
        console.error('Request failed:', error);
        return Promise.reject(error);
    }
);

export default axiosInstance;