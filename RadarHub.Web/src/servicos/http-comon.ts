import axios, { AxiosInstance } from "axios";

const apiClient: AxiosInstance = axios.create({
  baseURL: "https://localhost:7203/api", 
  headers: {
    "Content-type": "application/json",
    // 'Authorization': `Bearer ${localStorage.getItem('token')}`
    'Authorization': `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzdHJpbmdAZ21haWwuY29tIiwianRpIjoiZWI5NTc1ZWItODQzYy00ODI0LTg2MjYtM2ZmNmJiZWE1ZmFkIiwibm9tZSI6InN0cmluZyIsInVzdWFyaW9JZCI6IjEiLCJleHAiOjE3NjM3Njc1NzgsImlzcyI6Im1ldV9pc3N1ZXIiLCJhdWQiOiJtaW5oYV9hdWRpZW5jZSJ9.2C3PmjIYWWN3d1_O9lmQ4t_0cOJuCE7yuyBFCVjSw7o`
  },
});

export default apiClient;

