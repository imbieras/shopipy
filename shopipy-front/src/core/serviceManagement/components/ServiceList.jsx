import { Card, CardContent, CardDescription, CardHeader, CardTitle, CardFooter } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Clock, DollarSign, Sparkles } from "lucide-react"

export default function ServiceList({ services, selectedService, onServiceSelect }) {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      {services.map((service) => (
        <Card 
          key={service.serviceId || service.id} 
          className={`transition-all hover:shadow-lg ${
            selectedService?.serviceId === service.serviceId ? 'ring-2 ring-primary' : ''
          }`}
        >
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Sparkles className="h-5 w-5 text-primary" />
              {service.serviceName || service.name}
            </CardTitle>
            <CardDescription className="flex items-center gap-4">
              <span className="flex items-center gap-1">
                <DollarSign className="h-4 w-4" />
                {service.serviceBasePrice || service.price}
              </span>
              <span className="flex items-center gap-1">
                <Clock className="h-4 w-4" />
                {service.serviceDuration || service.duration} min
              </span>
            </CardDescription>
          </CardHeader>
          <CardContent>
            <p className="text-sm text-gray-600">
              {service.serviceDescription || service.description}
            </p>
          </CardContent>
          <CardFooter>
            <Button 
              className="w-full"
              variant={selectedService?.serviceId === service.serviceId ? "secondary" : "default"}
              onClick={() => onServiceSelect(service)}
            >
              {selectedService?.serviceId === service.serviceId ? 'Selected' : 'Select Service'}
            </Button>
          </CardFooter>
        </Card>
      ))}
    </div>
  );
}