// Services.jsx
import ScheduleService from "./ScheduleService";
import ServiceList from './components/ServiceList'
import { useEffect, useState } from "react";
import { serviceApi } from "./services/ServiceApi";
import { useUser } from "@/hooks/useUser";

export default function Services() {
  const [services, setServices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedService, setSelectedService] = useState(null);
  const { businessId } = useUser();
 
  useEffect(() => {
    const fetchServices = async () => {
      try {
        if (businessId) {
          const response = await serviceApi.getServices(businessId);
          setServices(response);
          console.log("Services: ", response);
        }
      } catch (error) {
        console.error("Error fetching services:", error);
      } finally {
        setLoading(false);
      }
    };
    fetchServices();
  }, [businessId]);

  if (loading) return <div>Loading...</div>;
 
  return (
    <main className="min-h-screen bg-gray-100 py-8">
      <div className="container mx-auto px-4">
       {!selectedService && (
        <div>
            <h1 className="text-3xl font-bold mb-8">Our Services</h1>
            <div className="mb-8">
            <ServiceList 
                services={services}
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
      </div>
    </main>
  );
}