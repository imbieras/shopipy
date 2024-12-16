import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
  } from "@/components/ui/dialog"
  import { format } from "date-fns"
  import { Separator } from "@/components/ui/separator"
  import { 
    Clock, 
    Mail, 
    Phone, 
    Calendar, 
    DollarSign,
    UserRound 
  } from "lucide-react"
  
  export function AppointmentDetailsModal({ 
    isOpen, 
    onClose, 
    appointmentDetails,
    serviceDetails 
  }) {
    if (!appointmentDetails) return null
  
    return (
      <Dialog open={isOpen} onOpenChange={onClose}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>Appointment Details</DialogTitle>
            <DialogDescription>
              Appointment #{appointmentDetails.appointmentId}
            </DialogDescription>
          </DialogHeader>
  
          <div className="space-y-6">
            {/* Customer Information */}
            <div>
              <h3 className="text-lg font-semibold mb-3">Customer Information</h3>
              <div className="grid gap-2">
                <div className="flex items-center gap-2">
                  <UserRound className="h-4 w-4 text-gray-500" />
                  <span>{appointmentDetails.customerName}</span>
                </div>
                <div className="flex items-center gap-2">
                  <Mail className="h-4 w-4 text-gray-500" />
                  <span>{appointmentDetails.customerEmail}</span>
                </div>
                <div className="flex items-center gap-2">
                  <Phone className="h-4 w-4 text-gray-500" />
                  <span>{appointmentDetails.customerPhone}</span>
                </div>
              </div>
            </div>
  
            <Separator />
  
            {/* Service Information */}
            <div>
              <h3 className="text-lg font-semibold mb-3">Service Details</h3>
              <div className="grid gap-2">
                <p className="text-base">{serviceDetails?.serviceName}</p>
                <div className="flex items-center gap-2">
                  <Clock className="h-4 w-4 text-gray-500" />
                  <span>{serviceDetails?.serviceDuration} minutes</span>
                </div>
                <div className="flex items-center gap-2">
                  <DollarSign className="h-4 w-4 text-gray-500" />
                  <span>${serviceDetails?.servicePrice}</span>
                </div>
                <p className="text-sm text-gray-600">
                  {serviceDetails?.serviceDescription}
                </p>
              </div>
            </div>
  
            <Separator />
  
            <div>
              <h3 className="text-lg font-semibold mb-3">Schedule</h3>
              <div className="grid gap-2">
                <div className="flex items-center gap-2">
                  <Calendar className="h-4 w-4 text-gray-500" />
                  <span>
                    {format(new Date(appointmentDetails.startTime), 'EEEE, MMMM d, yyyy')}
                  </span>
                </div>
                <div className="flex items-center gap-2">
                  <Clock className="h-4 w-4 text-gray-500" />
                  <span>
                    {format(new Date(appointmentDetails.startTime), 'h:mm a')} - 
                    {format(new Date(appointmentDetails.endTime), 'h:mm a')}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    )
  }