import axiosInstance from '@/services/axios';

export const discountsApi = {
  getDiscounts: async (businessId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/discounts`);
    return response.data;
  },
  getDiscountById: async (businessId, discountId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/discounts/${discountId}`);
    return response.data;
  },
  createDiscount: async (businessId, discountData) => {
    const response = await axiosInstance.post(`/businesses/${businessId}/discounts`, discountData);
    return response.data;
  },
  updateDiscount: async (businessId, discountId, effectiveTo) => {
    // Directly send only effectiveTo (no need for object wrapping)
    const response = await axiosInstance.put(`/businesses/${businessId}/discounts/${discountId}`, effectiveTo);
    return response.data;
  },
  deleteDiscount: async (businessId, discountId) => {
    try {
      const response = await axiosInstance.delete(`/businesses/${businessId}/discounts/${discountId}`);
      return response.data;  // Ensure it returns a response to confirm success
    } catch (error) {
      throw new Error('Error deleting discount: ' + error.message);  // Make sure to throw errors for catching
    }
  },
};
