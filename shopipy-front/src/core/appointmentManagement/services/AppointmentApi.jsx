import axiosInstance from "@/services/axios";

export const appointmentApi = {
  getAppointments: async (businessId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/appointments`);
    return response.data;
  },

  getAppointmentById: async (businessId, appointmentId) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/appointments/${appointmentId}`);
    return response.data;
  },

  getAvailableEmployees: async (businessId, serviceId, time) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/appointments/available-employees`, {
      params: {
        serviceId,
        time
      }
    });
    return response.data;
  },

  getEmployeeAppointments: async (businessId, employeeId, time, week = false) => {
    const response = await axiosInstance.get(`/businesses/${businessId}/appointments/employees/${employeeId}`, {
      params: {
        time,
        week
      }
    });
    return response.data;
  },
  getAvailableTimeSlots: async (businessId, employeeId, date, serviceId) => {
    const response = await axiosInstance.get(
      `/businesses/${businessId}/appointments/employees/${employeeId}/services/${serviceId}/slots`,
      {
        params: {
          date
        }
      }
    );
    return response.data;
  },

  createAppointment: async (businessId, appointmentData) => {
    const response = await axiosInstance.post(`/businesses/${businessId}/appointments`, appointmentData);
    return response.data;
  },

  updateAppointment: async (businessId, appointmentId, appointmentData) => {
    const response = await axiosInstance.put(
      `/businesses/${businessId}/appointments/${appointmentId}`,
      appointmentData
    );
    return response.data;
  },

  deleteAppointment: async (businessId, appointmentId, smsNotification = false) => {
    const response = await axiosInstance.delete(
      `/businesses/${businessId}/appointments/${appointmentId}`,
      {
        params: {
          smsNotification
        }
      }
    );
    return response.data;
  }
};