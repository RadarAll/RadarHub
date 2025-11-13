import axios, { AxiosInstance } from "axios";

const apiClient: AxiosInstance = axios.create({
  baseURL: "https://localhost:7203/api", 
  headers: {
    "Content-type": "application/json",
    // 'Authorization': `Bearer ${localStorage.getItem('token')}`
    'Authorization': `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0ZUBnbWFpbC5jb20iLCJqdGkiOiIzYjJiNjY3OC1lYWI4LTQwYjgtYWJlYy02ZjI5NjUyNDI1MjgiLCJub21lIjoiVGVzdGUiLCJ1c3VhcmlvSWQiOiIxIiwiZXhwIjoxNzYyOTk5MDQwLCJpc3MiOiJSYWRhckh1YkFQSSIsImF1ZCI6IlJhZGFySHViQVBJVXNlcnMifQ.SwkbqOocgfnsM3v6uB72Oo34SQJol-5sy21rpvWLOy0`
  },
});

export default apiClient;