import ScheduleService from "./ScheduleService";
import ServiceList from "./components/ServiceList";
import { useState } from "react";
import { serviceApi } from "./services/ServiceApi";
import { useUser } from "@/hooks/useUser";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import { Dialog, DialogContent } from "@/components/ui/dialog";
import ServiceManagement from "./components/ServiceManagement";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { CategoryFilter } from "../categoryManagement/components/CategoryFilter";
import { categoryApi } from "../categoryManagement/services/CategoryApi";

export default function Services() {
    const [selectedService, setSelectedService] = useState(null);
    const [showCreateDialog, setShowCreateDialog] = useState(false);
    const [selectedCategory, setSelectedCategory] = useState(null);
    const { businessId, id, role } = useUser();
    const queryClient = useQueryClient();
  
    const { data: services = [], isLoading: isServicesLoading } = useQuery({
      queryKey: ["services", businessId],
      queryFn: async () => {
        const result = await serviceApi.getServices(businessId);
        return result;
      },
      enabled: !!businessId,
      staleTime: 0,
      cacheTime: 0,
    });
  
    const { data: categories = [], isLoading: isCategoriesLoading } = useQuery({
      queryKey: ["categories", businessId],
      queryFn: async () => await categoryApi.getCategories(businessId),
      enabled: !!businessId,
    });
  
    const { mutate: createService, isLoading: isCreating } = useMutation({
      mutationFn: async (serviceData) => {
        const result = await serviceApi.createService(businessId, serviceData);
        return result;
      },
      onSuccess: async () => {
        await queryClient.invalidateQueries({
          queryKey: ["services", businessId],
        });
        setShowCreateDialog(false);
      },
      onError: (error) => {
        console.error("Error creating service: ", error);
      },
    });
  
    // Filter services based on selected category
    const filteredServices = selectedCategory
      ? services.filter(service => service.categoryId === selectedCategory)
      : services;
  
    if (isServicesLoading || isCategoriesLoading) return <div>Loading...</div>;
  
    return (
      <main className="min-h-screen bg-gray-100 py-8">
        <div className="container mx-auto px-4">
          {!selectedService && (
            <div>
              <div className="flex justify-between items-center mb-8">
                <h1 className="text-3xl font-bold">Our Services</h1>
                <div className="flex items-center gap-4">
                  <CategoryFilter 
                    categories={categories}
                    selectedCategory={selectedCategory}
                    onSelectCategory={setSelectedCategory}
                  />
                  {role === "BusinessOwner" && (
                    <Button
                      onClick={() => setShowCreateDialog(true)}
                      className="flex items-center gap-2"
                    >
                      <Plus className="h-4 w-4" />
                      Create Service
                    </Button>
                  )}
                </div>
              </div>
              <div className="mb-8">
                <ServiceList
                  services={filteredServices}
                  selectedService={selectedService}
                  onServiceSelect={setSelectedService}
                />
              </div>
            </div>
          )}
          {selectedService && (
            <ScheduleService
              selectedService={selectedService}
              onBack={() => setSelectedService(null)}
            />
          )}
          <Dialog open={showCreateDialog} onOpenChange={setShowCreateDialog}>
            <DialogContent className="max-w-4xl">
              <ServiceManagement
                onClose={() => setShowCreateDialog(false)}
                onServiceSaved={(serviceData) => createService(serviceData)}
                mode="create"
              />
            </DialogContent>
          </Dialog>
        </div>
      </main>
    );
  }