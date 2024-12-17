import axiosInstance from "@/services/axios";

export const productsApi = {
  createProduct: async (businessId, productData) => {
    const response = await axiosInstance.post(
      `/businesses/${businessId}/products`,
      productData
    );
    return response.data;
  },

  getAllProducts: async (businessId, { top = null, skip = null, categoryId = null } = {}) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/products`, {
      params: { top, skip, categoryId },
    });
    return response.data;
  },

  getProductById: async (businessId, productId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/products/${productId}`);
    return response.data;
  },

  updateProduct: async (businessId, productId, productData) => {
    const response = await axiosInstance.put(
      `/businesses/${businessId}/products/${productId}`,
      productData
    );
    return response.data;
  },

  deleteProduct: async (businessId, productId) => {
    const response = await axiosInstance.delete(`/businesses/${businessId}/products/${productId}`);
    return response.data;
  },
};
