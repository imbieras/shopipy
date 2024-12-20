import axiosInstance from "@/services/axios";

export const productsApi = {
  getProducts: async (businessId, top = null, skip = null, categoryId = null) => {
    const params = { top, skip, categoryId };
    const response = await axiosInstance.get(`/businesses/${businessId}/products`, { params });
    return response.data.data; // Extract the 'data' field containing products
  },

  getProductById: async (businessId, productId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/products/${productId}`);
    return response.data;
  },

  createProduct: async (businessId, productData) => {
    const response = await axiosInstance.post(`/businesses/${businessId}/products`, productData);
    return response.data;
  },

  updateProduct: async (businessId, productId, productData) => {
    const response = await axiosInstance.put(`/businesses/${businessId}/products/${productId}`, productData);
    return response.data;
  },

  deleteProduct: async (businessId, productId) => {
    const response = await axiosInstance.delete(`/businesses/${businessId}/products/${productId}`);
    return response.data;
  },

  // Product Variations API

  getProductVariations: async (businessId, productId, top = null, skip = null) => {
    const params = { top, skip };
    const response = await axiosInstance.get(`/businesses/${businessId}/products/${productId}/variations`, { params });
    return response.data.data; // Extract 'data' containing variations
  },

  getProductVariationById: async (businessId, productId, variationId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/products/${productId}/variations/${variationId}`);
    return response.data;
  },

  createProductVariation: async (businessId, productId, variationData) => {
    const response = await axiosInstance.post(`/businesses/${businessId}/products/${productId}/variations`, variationData);
    return response.data;
  },

  updateProductVariation: async (businessId, productId, variationId, variationData) => {
    const response = await axiosInstance.put(`/businesses/${businessId}/products/${productId}/variations/${variationId}`, variationData);
    return response.data;
  },

  deleteProductVariation: async (businessId, productId, variationId) => {
    const response = await axiosInstance.delete(`/businesses/${businessId}/products/${productId}/variations/${variationId}`);
    return response.data;
  }
};
