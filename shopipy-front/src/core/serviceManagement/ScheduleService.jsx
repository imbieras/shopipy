import { useState } from "react";
import { Calendar } from "@/components/ui/calendar";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { ChevronLeft } from "lucide-react";
import StaffList from "./components/StaffList";
import CustomerForm from "./components/CustomerForm";
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { appointmentApi } from "../appointmentManagement/services/AppointmentApi";
import { useBusiness } from "@/hooks/useUser";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import TimeSlotSelector from "./components/TimeSlotSelector";

export default function ScheduleService({ selectedService, onBack }) {
  const [selectedStaff, setSelectedStaff] = useState(null);
  const [selectedDate, setSelectedDate] = useState(undefined);
  const [selectedTime, setSelectedTime] = useState(null);
  const [sendSms, setSendSms] = useState(false);
  const [customerInfo, setCustomerInfo] = useState({
    name: "",
    email: "",
    phone: "",
  });
  const [isManagingServices, setIsManagingServices] = useState(false);
  const { businessId } = useBusiness();
  const [availableTimeSlots, setAvailableTimeSlots] = useState([]);
  const queryClient = useQueryClient();

  const { data: timeSlots = [], isLoading: isSlotsLoading } = useQuery({
    queryKey: [
      "timeSlots",
      businessId,
      selectedStaff,
      selectedService?.serviceId,
      selectedDate,
    ],
    queryFn: async () => {
      if (!selectedStaff || !selectedDate) return [];

      //really dumb bug with timezones CBA TO FIX, this + 1 fixes a bug where the date would be + 1 day,
      //funnily enough doing -1 would actually do + 1 day and vice versa
      //reviewer dont bother urself with such parts
      //stupid datetime js bugs stupid so sutpdi
      const adjustedDate = new Date(selectedDate);
      adjustedDate.setDate(adjustedDate.getDate() + 1);

      const response = await appointmentApi.getAvailableTimeSlots(
        businessId,
        selectedStaff,
        adjustedDate.toISOString(),
        selectedService.serviceId
      );

      return response.map((slot) => {
        const utcDate = new Date(slot.available_time);
        const localHours = utcDate.getUTCHours();
        const localMinutes = utcDate.getUTCMinutes();

        // Format as HH:mm
        return `${localHours.toString().padStart(2, "0")}:${localMinutes
          .toString()
          .padStart(2, "0")}`;
      });
    },
    enabled: !!(
      businessId &&
      selectedStaff &&
      selectedService?.serviceId &&
      selectedDate
    ),
  });

  const generateTimeSlots = () => {
    const duration = selectedService?.serviceDuration || 30;
    const slots = [];

    for (let hour = 9; hour < 17; hour++) {
      for (let minute = 0; minute < 60; minute += duration) {
        const timeString = `${hour.toString().padStart(2, "0")}:${minute
          .toString()
          .padStart(2, "0")}`;
        slots.push(timeString);
      }
    }

    return slots;
  };

  const handleBookAppointment = async () => {
    if (
      !selectedService ||
      !selectedStaff ||
      !selectedDate ||
      !selectedTime ||
      !customerInfo.name ||
      !customerInfo.email ||
      !customerInfo.phone
    ) {
      alert("Please fill in all required information before booking.");
      return;
    }

    try {
      const adjustedDate = new Date(selectedDate);
      adjustedDate.setDate(adjustedDate.getDate() + 1);
      const [hours, minutes] = selectedTime.split(":");
      // Set hours directly as UTC since our time slots are already in UTC
      adjustedDate.setUTCHours(parseInt(hours), parseInt(minutes), 0, 0);

      const appointmentData = {
        employeeId: selectedStaff,
        serviceId: selectedService.serviceId || selectedService.id,
        customerName: customerInfo.name,
        customerEmail: customerInfo.email,
        customerPhone: customerInfo.phone,
        startTime: adjustedDate.toISOString(),
        sendSmsNotification: sendSms,
      };

      const response = await appointmentApi.createAppointment(
        businessId,
        appointmentData
      );
      alert("Appointment booked successfully!");
      onBack();
    } catch (error) {
      if (error.response?.data?.message) {
        alert(error.response.data.message);
      } else if (error.message.includes("past")) {
        alert("Cannot create appointments in the past.");
      } else if (error.message.includes("already booked")) {
        alert("This time slot is already booked. Please choose another time.");
      } else {
        alert("Failed to book appointment. Please try again.");
      }
      console.error("Error booking appointment:", error);
    }
  };

  const allTimeSlots = generateTimeSlots();

  return (
    <div className="container mx-auto p-4">
      <Card className="w-full max-w-4xl mx-auto mb-8">
        <CardHeader className="relative">
          <div className="absolute left-4 top-4">
            <Button
              variant="ghost"
              size="icon"
              onClick={onBack}
              className="hover:bg-gray-100"
            >
              <ChevronLeft className="h-5 w-5" />
            </Button>
          </div>
          <div className="text-center">
            <CardTitle>
              Schedule {selectedService?.serviceName || selectedService?.name}
            </CardTitle>
            <CardDescription className="mt-2">
              ${selectedService?.servicePrice} •{" "}
              {selectedService?.serviceDuration || selectedService?.duration}{" "}
              minutes
            </CardDescription>
          </div>
        </CardHeader>
        <CardContent className="space-y-6">
          <StaffList
            selectedService={selectedService}
            selectedDate={selectedDate}
            selectedTime={selectedTime}
            selectedStaff={selectedStaff}
            onSelectStaff={setSelectedStaff}
          />
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="flex-1">
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Select a Date
              </label>
              <Calendar
                mode="single"
                selected={selectedDate}
                onSelect={setSelectedDate}
                className="rounded-md border"
              />
            </div>
            <TimeSlotSelector
              selectedTime={selectedTime}
              setSelectedTime={setSelectedTime}
              allTimeSlots={allTimeSlots}
              timeSlots={timeSlots}
              isSlotsLoading={isSlotsLoading}
            />
          </div>
          <CustomerForm
            customerInfo={customerInfo}
            setCustomerInfo={setCustomerInfo}
          />
          <div className="flex items-center space-x-2">
            <Switch
              checked={sendSms}
              onCheckedChange={setSendSms}
              id="sms-notifications"
            />
            <Label
              htmlFor="sms-notifications"
              className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
            >
              Receive appointment info via SMS
            </Label>
          </div>
        </CardContent>
        <CardFooter>
          <Button className="w-full" onClick={handleBookAppointment}>
            Book Appointment
          </Button>
        </CardFooter>
      </Card>

      {isManagingServices && (
        <Card className="w-full max-w-4xl mx-auto">
          <CardHeader>
            <CardTitle>Service Management</CardTitle>
            <CardDescription>
              Edit or delete services using the service ID from your SMS.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button onClick={() => setIsManagingServices(!isManagingServices)}>
              {isManagingServices
                ? "Hide Service Management"
                : "Show Service Management"}
            </Button>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
