import axiosInstance from "@/services/axios";

export const userApi = {
  getUsers: async (businessId) => {
    const response = await axiosInstance.get(`/users`);
    return response.data.data; // Extract the 'data' field containing users
  },
  getUserById: async (businessId, userId) => {
    const response = await axiosInstance.get(`/users/${userId}`);
    return response.data;
  },
  createUser: async (businessId, userData) => {
    const response = await axiosInstance.post(`/users`, userData);
    return response.data;
  },
  updateUser: async (businessId, userId, userData) => {
    const response = await axiosInstance.put(`/users/${userId}`, userData);
    return response.data;
  },
  deleteUser: async (businessId, userId) => {
    const response = await axiosInstance.delete(`/users/${userId}`);
    return response.data;
  },
};
