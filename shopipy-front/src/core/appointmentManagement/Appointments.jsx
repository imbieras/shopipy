import { useState, useEffect } from 'react'
import { Clock, User, ChevronRight, Search, Calendar } from 'lucide-react'
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Badge } from "@/components/ui/badge"
import { ScrollArea } from "@/components/ui/scroll-area"
import { format } from "date-fns"
import { useUser } from "@/hooks/useUser"
import { appointmentApi } from './services/AppointmentApi'
import { serviceApi } from '../serviceManagement/services/ServiceApi'
import { AppointmentDetailsModal } from './components/AdditionalDetailsDialog'
import { CancelAppointmentDialog } from './components/CancelAppointmentDialog'

export default function Appointments() {
  const [appointments, setAppointments] = useState([])
  const [selectedAppointment, setSelectedAppointment] = useState(null)
  const [searchTerm, setSearchTerm] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [viewingWeek, setViewingWeek] = useState(false)
  const [isDetailsModalOpen, setIsDetailsModalOpen] = useState(false)
  const [isCancelDialogOpen, setIsCancelDialogOpen] = useState(false)
  const [appointmentToCancel, setAppointmentToCancel] = useState(null)
  const [appointmentDetails, setAppointmentDetails] = useState(null)
  const [serviceDetails, setServiceDetails] = useState(null)
  const { businessId, id } = useUser()

  useEffect(() => {
    fetchAppointments()
  }, [businessId, viewingWeek])

  const fetchAppointments = async () => {
    try {
      setLoading(true)
      const currentDate = new Date()
      const response = await appointmentApi.getEmployeeAppointments(
        businessId,
        id,
        currentDate.toISOString(),
        viewingWeek
      )
      setAppointments(response)
    } catch (err) {
      setError('Failed to fetch appointments')
      console.error('Error fetching appointments:', err)
    } finally {
      setLoading(false)
    }
  }

  const filteredAppointments = appointments.filter(appointment =>
    appointment.customer_name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    appointment.appointment_id.toString().includes(searchTerm)
  )

  const handleAppointmentClick = (appointmentId) => {
    setSelectedAppointment(appointmentId === selectedAppointment ? null : appointmentId)
  }

  const handleCancelAppointment = (appointmentId) => {
    setAppointmentToCancel(appointmentId)
    setIsCancelDialogOpen(true)
  }

  const confirmCancelAppointment = async () => {
    try {
      await appointmentApi.deleteAppointment(businessId, appointmentToCancel, true)
      fetchAppointments()
      setIsCancelDialogOpen(false)
      setAppointmentToCancel(null)
      setSelectedAppointment(null)
    } catch (error) {
      console.error('Error canceling appointment:', error)
      alert('Failed to cancel appointment')
    }
  }

  const handleViewDetails = async (appointmentId) => {
    try {
      const appointmentData = await appointmentApi.getAppointmentById(businessId, appointmentId)
      const serviceData = await serviceApi.getServiceById(businessId, appointmentData.serviceId)
      
      setAppointmentDetails(appointmentData)
      setServiceDetails(serviceData)
      setIsDetailsModalOpen(true)
    } catch (error) {
      console.error('Error fetching appointment details:', error)
      alert('Failed to load appointment details')
    }
  }

  const formatAppointmentTime = (startTime) => {
    return format(new Date(startTime), 'h:mm a')
  }

  const getStatusBadge = (startTime) => {
    const appointmentDate = new Date(startTime)
    const now = new Date()
    
    if (appointmentDate < now) {
      return <Badge className="bg-blue-500 text-white">Completed</Badge>
    }
    if (appointmentDate.toDateString() === now.toDateString()) {
      return <Badge className="bg-green-500 text-white">Today</Badge>
    }
    return <Badge className="bg-gray-500 text-white">Upcoming</Badge>
  }

  if (loading) return <div className="container mx-auto p-4">Loading appointments...</div>
  if (error) return <div className="container mx-auto p-4 text-red-500">{error}</div>

  return (
    <div className="container mx-auto p-4 max-w-4xl">
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">My Appointments</h1>
        <Button 
          variant="outline"
          onClick={() => setViewingWeek(!viewingWeek)}
        >
          View {viewingWeek ? 'Today' : 'Week'}
        </Button>
      </div>

      <div className="mb-4 relative">
        <Search className="absolute left-2 top-1/2 transform -translate-y-1/2 text-gray-400" />
        <Input
          type="text"
          placeholder="Search by customer name or appointment ID"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="pl-10"
        />
      </div>

      <ScrollArea className="h-[calc(100vh-200px)]">
        {filteredAppointments.map((appointment) => (
          <div
            key={appointment.appointment_id}
            className={`mb-4 p-4 border rounded-lg cursor-pointer transition-colors ${
              selectedAppointment === appointment.appointment_id ? 'bg-blue-50 border-blue-500' : 'bg-white hover:bg-gray-50'
            }`}
            onClick={() => handleAppointmentClick(appointment.appointment_id)}
          >
            <div className="flex justify-between items-center">
              <div>
                <h2 className="text-lg font-semibold">{appointment.customer_name}</h2>
                <p className="text-sm text-gray-500">Appointment #{appointment.appointment_id}</p>
              </div>
              <ChevronRight className={`transition-transform ${selectedAppointment === appointment.appointment_id ? 'rotate-90' : ''}`} />
            </div>

            <div className="mt-2 flex justify-between items-center">
              <div className="flex items-center space-x-4">
                <Badge variant="secondary" className="flex items-center">
                  <Calendar className="w-4 h-4 mr-1" />
                  {format(new Date(appointment.start_time), 'MMM d, yyyy')}
                </Badge>
                <Badge variant="secondary" className="flex items-center">
                  <Clock className="w-4 h-4 mr-1" />
                  {formatAppointmentTime(appointment.start_time)}
                </Badge>
              </div>
              {getStatusBadge(appointment.start_time)}
            </div>

            {selectedAppointment === appointment.appointment_id && (
              <div className="mt-4 space-y-2">
                <div className="text-sm text-gray-600">
                  <p><strong>Service:</strong> {appointment.service_name}</p>
                  <p><strong>Duration:</strong> Until {formatAppointmentTime(appointment.end_time)}</p>
                </div>
                <div className="flex justify-end space-x-2">
                  <Button 
                    variant="destructive" 
                    onClick={(e) => {
                      e.stopPropagation()
                      handleCancelAppointment(appointment.appointment_id)
                    }}
                  >
                    Cancel Appointment
                  </Button>
                  <Button 
                    variant="secondary"
                    onClick={(e) => {
                      e.stopPropagation()
                      handleViewDetails(appointment.appointment_id)
                    }}
                  >
                    View Details
                  </Button>
                </div>
              </div>
            )}
          </div>
        ))}
      </ScrollArea>
      <CancelAppointmentDialog
        isOpen={isCancelDialogOpen}
        onClose={() => setIsCancelDialogOpen(false)}
        onConfirm={confirmCancelAppointment}
      />

      <AppointmentDetailsModal
        isOpen={isDetailsModalOpen}
        onClose={() => setIsDetailsModalOpen(false)}
        appointmentDetails={appointmentDetails}
        serviceDetails={serviceDetails}
      />
    </div>
  )
}