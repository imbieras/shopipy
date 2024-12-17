import axiosInstance from "@/services/axios";

export const categoryApi = {
    getCategories: async (businessId) => {
        const response = await axiosInstance.get(`/businesses/${businessId}/categories`);
        return response.data;
    },
    getCategoryById: async (businessId, categoryId) => {
        const response = await axiosInstance.get(`/businesses/${businessId}/categories/${categoryId}`);
        return response.data;
    },
    createCategory: async (businessId, categoryData) => {
        const response = await axiosInstance.post(`/businesses/${businessId}/categories`, categoryData);
        return response.data;
    },
    updateCategory: async (businessId, categoryId, categoryData) => {
        const response = await axiosInstance.put(`/businesses/${businessId}/categories/${categoryId}`, categoryData);
        return response.data;
    },
    deleteCategory: async (businessId, categoryId) => {
        const response = await axiosInstance.delete(`/businesses/${businessId}/categories/${categoryId}`);
        return response.data;
    }
};