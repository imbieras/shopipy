import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group"
import { Label } from "@/components/ui/label"
import { useUser } from "@/hooks/useUser"
import { useQuery } from "@tanstack/react-query"
import { appointmentApi } from "@/core/appointmentManagement/services/AppointmentApi"

export default function StaffList({
    selectedService,
    selectedDate,
    selectedTime,
    selectedStaff,
    onSelectStaff
  }) {
    const { businessId } = useUser();
    const getDateTime = () => {
      if (!selectedDate) return null;
      const dateTime = new Date(selectedDate);
      dateTime.setHours(0, 0, 0, 0);
      return dateTime;
    };
 
    const { data: availableStaff = [], isLoading, error } = useQuery({
      queryKey: ['availableStaff', businessId, selectedService?.serviceId, selectedDate],
      queryFn: async () => {
        const dateTime = getDateTime();
        if (!dateTime) return [];
       
        return await appointmentApi.getAvailableEmployees(
          businessId,
          selectedService.serviceId,
          dateTime.toISOString()
        );
      },
      enabled: !!(businessId && selectedService?.serviceId && selectedDate)
    });

  if (!selectedService) return null;

  if (!selectedDate) { // Removed selectedTime check
    return (
      <Card>
        <CardHeader>
          <CardTitle>Available Staff</CardTitle>
          <CardDescription>Please select a date first</CardDescription>
        </CardHeader>
      </Card>
    );
  }

  if (isLoading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Loading Staff</CardTitle>
          <CardDescription>Please wait while we check staff availability...</CardDescription>
        </CardHeader>
      </Card>
    );
  }

  if (error) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Error Loading Staff</CardTitle>
          <CardDescription>Failed to load available staff. Please try again.</CardDescription>
        </CardHeader>
      </Card>
    );
  }

  if (availableStaff.length === 0) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>No Staff Available</CardTitle>
          <CardDescription>No staff members are available on this date. Please try another date.</CardDescription>
        </CardHeader>
      </Card>
    );
  }

  return (
    <RadioGroup
      value={selectedStaff?.toString()}
      onValueChange={(value) => onSelectStaff(value)}
    >
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {availableStaff.map((staff) => (
          <Card
            key={staff.employee_id}
            className={selectedStaff === staff.employee_id ? "border-primary" : ""}
          >
            <CardHeader>
              <CardTitle>{staff.employee_name}</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex items-center space-x-2">
                <RadioGroupItem
                  value={staff.employee_id.toString()}
                  id={`staff-${staff.employee_id}`}
                />
                <Label htmlFor={`staff-${staff.employee_id}`}>Select this staff member</Label>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>
    </RadioGroup>
  );
}