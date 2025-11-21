import axios, { AxiosInstance } from "axios";

const apiClient: AxiosInstance = axios.create({
  baseURL: "https://localhost:7203/api", 
  headers: {
    "Content-type": "application/json",
    // 'Authorization': `Bearer ${localStorage.getItem('token')}`
    'Authorization': `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzdHJpbmdAZ21haWwuY29tIiwianRpIjoiZWViY2EyYzEtYjI0ZS00YjMxLTk4ZjAtZDVmM2FmZmQxNzU2Iiwibm9tZSI6InN0cmluZyIsInVzdWFyaW9JZCI6IjEiLCJleHAiOjE3NjM3NzUxMzksImlzcyI6Im1ldV9pc3N1ZXIiLCJhdWQiOiJtaW5oYV9hdWRpZW5jZSJ9.Oiy0QW07WXXyl1-vK9WGgHlUQmwX33fwrYtnbhH_WtQ`
  },
});

export default apiClient;

