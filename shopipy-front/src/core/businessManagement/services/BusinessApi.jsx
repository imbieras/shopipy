import axiosInstance from "@/services/axios";

export const businessApi = {
  getBusinesses: async () => {
    const response = await axiosInstance.get(`/businesses`);
    return response.data;
  },

  createBusiness: async (businessData) => {
    const response = await axiosInstance.post(`/businesses`, businessData);
    return response.data;
  },

  updateBusiness: async (businessId, businessData) => {
    const response = await axiosInstance.put(`/businesses/${businessId}`, businessData);
    return response.data;
  },
};