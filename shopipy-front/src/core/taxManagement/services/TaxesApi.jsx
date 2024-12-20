import axiosInstance from '@/services/axios';

export const taxesApi = {
  getTaxRates: async (businessId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/taxrates`);
    return response.data;
  },
  getTaxRateById: async (businessId, taxId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/taxrates/${taxId}`);
    return response.data;
  },
  createTaxRate: async (businessId, taxData) => {
    const response = await axiosInstance.post(`/businesses/${businessId}/taxrates`, taxData);
    return response.data;
  },
  updateTaxRate: async (businessId, taxId, effectiveTo) => {
    const response = await axiosInstance.put(`/businesses/${businessId}/taxrates/${taxId}`, effectiveTo);
    return response.data;
  },
  deleteTaxRate: async (businessId, taxId) => {
    const response = await axiosInstance.delete(`/businesses/${businessId}/taxrates/${taxId}`);
    return response.data;
  }
};
