import { create } from 'zustand';
import axiosInstance from '../services/axios';
const API_URL = 'https://localhost:7417';

const initialState = {
  id: null,
  username: null,
  email: null,
  name: null,
  role: null,
  businessId: 0,
  loggedIn: false,
  fetched: false,
  isLoading: false,
  error: null,
};

export const useUser = create((set, get) => ({
  ...initialState, // Spread initial state

  fetchUser: async (userId) => {
    if (!userId) return;
    set({ isLoading: true });
    set({id: userId})
    try {
      const response = await axiosInstance.get(API_URL + `/users/${userId}`);
      set({
        email: response.data.email,
        name: response.data.name,
        role: response.data.role,
        businessId: response.data.businessId,
        fetched: true,
        isLoading: false
      });
    } catch (error) {
      set({
        error: error.message,
        isLoading: false
      });
      throw error;
    }
  },

  setBusinessId: (businessId) => {
    set({ businessId });
  },

  // Reset all state to initial values
  clearUser: () => {
    set(initialState);
  },

  // Getters
  getUserId: () => get().id,
  getUsername: () => get().username,
  getDisplayName: () => get().displayName
}));