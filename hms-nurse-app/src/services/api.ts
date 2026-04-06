import axios from 'axios';
import { StorageService } from './storageService';

const API_BASE_URL = 'http://localhost:5000/api'; // Update with your API URL

export const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
});

// Request interceptor to add token
api.interceptors.request.use(
  async (config) => {
    const token = await StorageService.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      // Token expired, clear storage and redirect to login
      await StorageService.clearAll();
      // Navigate to login (implement navigation logic)
    }
    return Promise.reject(error);
  }
);

