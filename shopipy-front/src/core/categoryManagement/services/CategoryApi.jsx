import axiosInstance from "@/services/axios";

export const categoryApi = {
    getCategories: async (businessId) => {
        const response = await axiosInstance.get(`/businesses/${businessId}/categories`);
        return response.data;
    },

    getCategoryById: async (businessId, categoryId) => {
        const response = await axiosInstance.get(`/businesses/${businessId}/categories/${categoryId}`);
        return response.data;
    }
};