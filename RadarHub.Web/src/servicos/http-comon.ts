import axios, { AxiosInstance } from "axios";

const apiClient: AxiosInstance = axios.create({
  baseURL: "https://localhost:7203/api", 
  headers: {
    "Content-type": "application/json",
    // 'Authorization': `Bearer ${localStorage.getItem('token')}`
    'Authorization': `Bearer ${localStorage.getItem('token')}`
  },
});

export default apiClient;

