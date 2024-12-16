import { useState } from 'react'
import { format } from 'date-fns'
import { Calendar } from "@/components/ui/calendar"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { ChevronLeft } from "lucide-react"
import StaffList from './components/StaffList'
import CustomerForm from './components/CustomerForm'
import ServiceManagement from './components/ServiceManagement'
import { Switch } from '@/components/ui/switch'
import { Label } from '@/components/ui/label'
import { appointmentApi } from '../appointmentManagement/services/AppointmentApi'
import { useUser } from '@/hooks/useUser'

const timeSlots = [
  '09:00', '09:30', '10:00', '10:30', '11:00', '11:30',
  '13:00', '13:30', '14:00', '14:30', '15:00', '15:30',
  '16:00', '16:30', '17:00'
]

export default function ScheduleService({ selectedService, onBack }) {
  const [selectedStaff, setSelectedStaff] = useState(null)
  const [selectedDate, setSelectedDate] = useState(undefined)
  const [selectedTime, setSelectedTime] = useState(null)
  const [sendSms, setSendSms] = useState(false);
  const [customerInfo, setCustomerInfo] = useState({ name: '', email: '', phone: '' })
  const [isManagingServices, setIsManagingServices] = useState(false)
  const { businessId } = useUser();

  const handleBookAppointment = async () => {
    if (!selectedService || !selectedStaff || !selectedDate || !selectedTime || 
        !customerInfo.name || !customerInfo.email || !customerInfo.phone) {
      alert('Please fill in all required information before booking.');
      return;
    }
  
    try {
      // Combine date and time for the API
      const appointmentDateTime = new Date(selectedDate);
      const [hours, minutes] = selectedTime.split(':');
      appointmentDateTime.setHours(parseInt(hours), parseInt(minutes), 0, 0);
  
      const appointmentData = {
        employeeId: selectedStaff,
        serviceId: selectedService.serviceId || selectedService.id,
        customerName: customerInfo.name,
        customerEmail: customerInfo.email,
        customerPhone: customerInfo.phone,
        startTime: appointmentDateTime.toISOString(),
        sendSmsNotification: true // You might want to make this configurable
      };
  
      const response = await appointmentApi.createAppointment(businessId, appointmentData);
      
      alert('Appointment booked successfully!');
      onBack();
      
    } catch (error) {
      if (error.response?.data?.message) {
        alert(error.response.data.message);
      } else if (error.message.includes('past')) {
        alert('Cannot create appointments in the past.');
      } else if (error.message.includes('already booked')) {
        alert('This time slot is already booked. Please choose another time.');
      } else {
        alert('Failed to book appointment. Please try again.');
      }
      console.error('Error booking appointment:', error);
    }
  };

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
            <CardTitle>Schedule {selectedService?.serviceName || selectedService?.name}</CardTitle>
            <CardDescription className="mt-2">
              ${selectedService?.servicePrice} • {selectedService?.serviceDuration || selectedService?.duration} minutes
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
            <div className="flex-1">
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Select a Time
              </label>
              <div className="grid grid-cols-3 gap-2">
                {timeSlots.map((time) => (
                  <Button
                    key={time}
                    variant={selectedTime === time ? "default" : "outline"}
                    className="w-full"
                    onClick={() => setSelectedTime(time)}
                  >
                    {time}
                  </Button>
                ))}
              </div>
            </div>
          </div>
          <CustomerForm customerInfo={customerInfo} setCustomerInfo={setCustomerInfo} />
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
            <CardDescription>Edit or delete services using the service ID from your SMS.</CardDescription>
          </CardHeader>
          <CardContent>
            <Button onClick={() => setIsManagingServices(!isManagingServices)}>
              {isManagingServices ? 'Hide Service Management' : 'Show Service Management'}
            </Button>
            <ServiceManagement services={[selectedService]} />
          </CardContent>
        </Card>
      )}
    </div>
  )
}