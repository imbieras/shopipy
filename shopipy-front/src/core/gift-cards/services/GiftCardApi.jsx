import axiosInstance from '@/services/axios';

export const giftCardApi = {
  // Fetch all gift cards for a specific business
  getGiftCards: async (businessId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/giftcards`);
    return response.data;
  },

  // Fetch a specific gift card by its ID
  getGiftCardById: async (businessId, giftCardId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/giftcards/${giftCardId}`);
    return response.data;
  },

  // Add a new gift card
  createGiftCard: async (businessId, giftCardData) => {
    const response = await axiosInstance.post(`/businesses/${businessId}/giftcards`, giftCardData);
    return response.data;
  },

  // Update an existing gift card
  updateGiftCard: async (businessId, giftCardId, updatedData) => {
    const response = await axiosInstance.put(`/businesses/${businessId}/giftcards/${giftCardId}`, updatedData);
    return response.data;
  },

  // Delete a gift card
  deleteGiftCard: async (businessId, giftCardId) => {
    try{
      const response = await axiosInstance.delete(`/businesses/${businessId}/giftcards/${giftCardId}`);
      return response.data;      
    } catch (e) {
      throw new Error('Error deleting giftcard: ' + e.message);
    }

  },
};
