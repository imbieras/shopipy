import axiosInstance from "@/services/axios";

export const productVariationApi = {
  createVariation: async (businessId, productId, variationData) => {
    const response = await axiosInstance.post(
      `/businesses/${businessId}/products/${productId}/variations`,
      variationData
    );
    return response.data;
  },

  getAllVariationsOfProduct: async (businessId, productId, { top = null, skip = null } = {}) => {
    const response = await axiosInstance.get(
      `/businesses/${businessId}/products/${productId}/variations`,
      { params: { top, skip } }
    );
    return response.data;
  },

  getVariationById: async (businessId, productId, variationId) => {
    const response = await axiosInstance.get(
      `/businesses/${businessId}/products/${productId}/variations/${variationId}`
    );
    return response.data;
  },

  updateVariation: async (businessId, productId, variationId, variationData) => {
    const response = await axiosInstance.put(
      `/businesses/${businessId}/products/${productId}/variations/${variationId}`,
      variationData
    );
    return response.data;
  },

  deleteVariation: async (businessId, productId, variationId) => {
    const response = await axiosInstance.delete(
      `/businesses/${businessId}/products/${productId}/variations/${variationId}`
    );
    return response.data;
  },
};
