import axios, { AxiosInstance } from "axios";

const apiClient: AxiosInstance = axios.create({
  baseURL: "http://localhost:5176/api", 
  headers: {
    "Content-type": "application/json",
    // 'Authorization': `Bearer ${localStorage.getItem('token')}`
    'Authorization': `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJyYXlzc2FAZ21haWwuY29tIiwianRpIjoiMzY0OTE2OWYtMThmZi00ZTZiLThhODgtNDJkNDdhZmExYzdiIiwibm9tZSI6IlJheXNzYSBHb21pZGVzIiwidXN1YXJpb0lkIjoiMSIsImV4cCI6MTc2MjgxNzc0NiwiaXNzIjoiUmFkYXJHb3ZBUEkiLCJhdWQiOiJSYWRhckdvdkFQSVVzZXJzIn0.480YX_iqmX5K_A81lpyntAhikpbqV0iNprYinT-CKz8`
  },
});

export default apiClient;