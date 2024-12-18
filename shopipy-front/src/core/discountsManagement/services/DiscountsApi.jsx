import axiosInstance from "@/services/axios";

// Get all discounts for a business
export const getDiscounts = async (businessId) => {
  const response = await axiosInstance.get(`/businesses/${businessId}/discounts`);
  return response.data; // Return the list of discounts
};

// Get a discount by its ID
export const getDiscountById = async (businessId, discountId) => {
  const response = await axiosInstance.get(`/businesses/${businessId}/discounts/${discountId}`);
  return response.data; // Return the discount details
};

// Create a new discount
export const createDiscount = async (businessId, discountData) => {
  const response = await axiosInstance.post(`/businesses/${businessId}/discounts`, discountData);
  return response.data; // Return the created discount
};

// Update an existing discount
export const updateDiscount = async (businessId, discountId, discountData) => {
  const response = await axiosInstance.put(`/businesses/${businessId}/discounts/${discountId}`, discountData);
  return response.data; // Return the updated discount
};

// Delete a discount by its ID
export const deleteDiscount = async (businessId, discountId) => {
  const response = await axiosInstance.delete(`/businesses/${businessId}/discounts/${discountId}`);
  return response.data; // Return success or failure response
};
