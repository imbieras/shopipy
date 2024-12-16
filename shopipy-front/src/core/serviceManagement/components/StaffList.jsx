import { useEffect, useState } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group"
import { Label } from "@/components/ui/label"
import { appointmentApi } from "@/core/appointmentManagement/services/AppointmentApi"
import { useUser } from "@/hooks/useUser"

export default function StaffList({ selectedService, selectedDate, selectedTime, selectedStaff, onSelectStaff }) {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(null)
  const [availableStaff, setAvailableStaff] = useState([])
  const { businessId } = useUser()

  useEffect(() => {
    const fetchAvailableStaff = async () => {
      if (!selectedService || !selectedDate || !selectedTime) return

      try {
        setLoading(true)
        setError(null)

        // Combine date and time for the API call
        const dateTime = new Date(selectedDate)
        const [hours, minutes] = selectedTime.split(':')
        dateTime.setHours(parseInt(hours), parseInt(minutes), 0, 0)

        const response = await appointmentApi.getAvailableEmployees(
          businessId,
          selectedService.serviceId || selectedService.id,
          dateTime.toISOString()
        )

        setAvailableStaff(response)
      } catch (err) {
        setError("Failed to load available staff. Please try again.")
        console.error("Error fetching available staff:", err)
      } finally {
        setLoading(false)
      }
    }

    fetchAvailableStaff()
  }, [businessId, selectedService, selectedDate, selectedTime])

  if (!selectedService) {
    return null
  }

  if (loading) {
    return <div>Loading available staff...</div>
  }

  if (error) {
    return <div className="text-red-500">{error}</div>
  }

  if (!selectedDate || !selectedTime) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Available Staff</CardTitle>
          <CardDescription>Please select a date and time first</CardDescription>
        </CardHeader>
      </Card>
    )
  }

  if (availableStaff.length === 0) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>No Staff Available</CardTitle>
          <CardDescription>No staff members are available at the selected time. Please try another time slot.</CardDescription>
        </CardHeader>
      </Card>
    )
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
  )
}