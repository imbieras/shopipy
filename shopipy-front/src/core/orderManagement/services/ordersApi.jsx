import axiosInstance from "@/services/axios";

export const ordersApi = {
  getOrderById: async (businessId, orderId, withItems = true) => {
    const response = await axiosInstance.get(
      `/businesses/${businessId}/orders/${orderId}`,
      {
        params: { withItems },
      }
    );
    return response.data;
  },

  getOrders: async (businessId) => {
    const response = await axiosInstance.get(
      `/businesses/${businessId}/orders`
    );
    return response.data;
  },

  getProductItems: async (businessId, orderId) => {
    const response = await axiosInstance.get(
      `/businesses/${businessId}/orders/${orderId}/product-items`
    );
    return response.data;
  },

  getServiceItems: async (businessId, orderId) => {
    const response = await axiosInstance.get(
      `/businesses/${businessId}/orders/${orderId}/service-items`
    );
    return response.data;
  },

  createOrderItem: async (businessId, orderId, orderItemData) => {
    const response = await axiosInstance.post(
      `/businesses/${businessId}/orders/${orderId}/items`,
      orderItemData
    );
    return response.data;
  },

  updateOrderItem: async (businessId, orderId, orderItemId, orderItemData) => {
    const response = await axiosInstance.put(
      `/businesses/${businessId}/orders/${orderId}/items/${orderItemId}`,
      orderItemData
    );
    return response.data;
  },

  cancelOrder: async (businessId, orderId) => {
    const response = await axiosInstance.post(
      `/businesses/${businessId}/orders/${orderId}/cancel`
    );
    return response.data;
  },

  deleteOrderItem: async (businessId, orderId, orderItemId) => {
    const response = await axiosInstance.delete(
      `/businesses/${businessId}/orders/${orderId}/items/${orderItemId}`
    );
    return response.data;
  },

  createPayment: async (businessId, orderId, payment) => {
    const response = await axiosInstance.post(
      `/businesses/${businessId}/orders/${orderId}/payments`,
      payment
    );
    return response.data;
  },
};
