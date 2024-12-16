import { Card, CardContent, CardDescription, CardHeader, CardTitle, CardFooter } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Clock, DollarSign, Sparkles, Pencil, Trash2 } from "lucide-react"

export function ServiceCard({ 
    service, 
    selectedService, 
    onServiceSelect, 
    onEditClick, 
    onDeleteClick, 
    isBusinessOwner 
  }) {
    return (
      <Card
        key={service.serviceId || service.id}
        className={`transition-all hover:shadow-lg ${
          selectedService?.serviceId === service.serviceId ? 'ring-2 ring-primary' : ''
        } ${!service.isServiceActive ? 'opacity-75' : ''}`}
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
        <CardFooter className="flex flex-col gap-2">
          {service.isServiceActive ? (
            <Button
              className="w-full"
              variant={selectedService?.serviceId === service.serviceId ? "secondary" : "default"}
              onClick={() => onServiceSelect(service)}
            >
              {selectedService?.serviceId === service.serviceId ? 'Selected' : 'Select Service'}
            </Button>
          ) : (
            <Button
              className="w-full"
              variant="secondary"
              disabled
            >
              Service Inactive
            </Button>
          )}
          {isBusinessOwner && (
            <div className="flex gap-2 w-full">
              <Button
                variant="outline"
                className="flex-1"
                onClick={(e) => {
                  e.stopPropagation();
                  onEditClick(service, e);
                }}
              >
                <Pencil className="h-4 w-4 mr-2" />
                Edit
              </Button>
              <Button
                variant="destructive"
                className="flex-1"
                onClick={(e) => {
                  e.stopPropagation();
                  onDeleteClick(service, e);
                }}
              >
                <Trash2 className="h-4 w-4 mr-2" />
                Delete
              </Button>
            </div>
          )}
        </CardFooter>
      </Card>
    );
  }