import axiosInstance from "@/services/axios";

export const serviceApi = {
  getServices: async (businessId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/services`);
    return response.data;
  },
  
  createService: async (businessId, serviceData) => {
    const response = await axiosInstance.post(`/businesses/${businessId}/services`, serviceData);
    return response.data;
  },

  getServiceById: async (businessId, serviceId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/services/${serviceId}`);
    return response.data;
  },
  
  updateService: async (businessId, serviceId, serviceData) => {
    const response = await axiosInstance.put(`/businesses/${businessId}/services/${serviceId}`, serviceData);
    return response.data;
  },
  
  deleteService: async (businessId, serviceId) => {
    const response = await axiosInstance.delete(`/businesses/${businessId}/services/${serviceId}`);
    return response.data;
  }
};