import { useState } from "react"
import { useUser } from "@/hooks/useUser"
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog"
import { Dialog, DialogContent } from "@/components/ui/dialog"
import { serviceApi } from "../services/ServiceApi"
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { ServiceCard } from "./ServiceCard"
import ServiceManagement from './ServiceManagement'

export default function ServiceList({ services, selectedService, onServiceSelect }) {
  const { role, businessId } = useUser()
  const [showDeleteDialog, setShowDeleteDialog] = useState(false)
  const [serviceToDelete, setServiceToDelete] = useState(null)
  const [showEditDialog, setShowEditDialog] = useState(false)
  const [serviceToEdit, setServiceToEdit] = useState(null)
  const queryClient = useQueryClient()

  // Separate active and inactive services
  const activeServices = services.filter(service => service.isServiceActive)
  const inactiveServices = services.filter(service => !service.isServiceActive)

  // Delete mutation
  const { mutate: deleteService } = useMutation({
    mutationFn: async (serviceId) => {
      await serviceApi.deleteService(businessId, serviceId)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['services', businessId] })
      setShowDeleteDialog(false)
      setServiceToDelete(null)
    },
    onError: (error) => {
      console.error("Error deleting service:", error)
      alert("Failed to delete service")
    }
  })

  const handleDeleteClick = (service, e) => {
    setServiceToDelete(service)
    setShowDeleteDialog(true)
  }

  const handleEditClick = (service, e) => {
    setServiceToEdit(service)
    setShowEditDialog(true)
  }

  const confirmDelete = () => {
    if (serviceToDelete) {
      deleteService(serviceToDelete.serviceId)
    }
  }

  return (
    <>
      <div className="space-y-8">
        {/* Active Services Section */}
        {activeServices.length > 0 && (
          <div className="space-y-4">
            <h2 className="text-xl font-semibold">Active Services</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {activeServices.map((service) => (
                <ServiceCard
                  key={service.serviceId}
                  service={service}
                  selectedService={selectedService}
                  onServiceSelect={onServiceSelect}
                  onEditClick={handleEditClick}
                  onDeleteClick={handleDeleteClick}
                  isBusinessOwner={role === 'BusinessOwner'}
                />
              ))}
            </div>
          </div>
        )}

        {/* Inactive Services Section */}
        {inactiveServices.length > 0 && (
          <div className="space-y-4">
            <div className="flex items-center gap-4">
              <div className="flex-1 h-px bg-gray-200" />
              <h2 className="text-xl font-semibold text-gray-600">Inactive Services</h2>
              <div className="flex-1 h-px bg-gray-200" />
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {inactiveServices.map((service) => (
                <ServiceCard
                  key={service.serviceId}
                  service={service}
                  selectedService={selectedService}
                  onServiceSelect={onServiceSelect}
                  onEditClick={handleEditClick}
                  onDeleteClick={handleDeleteClick}
                  isBusinessOwner={role === 'BusinessOwner'}
                />
              ))}
            </div>
          </div>
        )}
      </div>

      <AlertDialog open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Service</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete this service? This action cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete}>Delete</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      <Dialog open={showEditDialog} onOpenChange={setShowEditDialog}>
        <DialogContent className="max-w-4xl">
          <ServiceManagement
            service={serviceToEdit}
            onClose={() => setShowEditDialog(false)}
          />
        </DialogContent>
      </Dialog>
    </>
  )
}