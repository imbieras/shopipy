import axiosInstance from "@/services/axios";

export const businessApi = {
  getBusinesses: async (businessId) => {
    const response = await axiosInstance.get(`/businesses`);
    return response.data;
  },
  
  createBusiness: async (businessData) => {
    const response = await axiosInstance.post(`/businesses`,businessData);
    return response.data;
  },

  getBusinessById: async (businessId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}`);
    return response.data;
  },
  
  updateBusiness: async (businessId, businessData) => {
    const response = await axiosInstance.put(`/businesses/${businessId}`, businessData);
    return response.data;
  },
  
  deleteBusiness: async (businessId) => {
    const response = await axiosInstance.delete(`/businesses/${businessId}`);
    return response.data;
  }
};