import { useQuery } from '@tanstack/react-query';
import { serviceApi } from '@/core/serviceManagement/services/ServiceApi';
import { useUser } from '@/hooks/useUser';
import { Button } from "@/components/ui/button";
import { Badge } from '@/components/ui/badge';
import { Trash2, Clock } from "lucide-react";

const ServiceOrderItem = ({ item, order, role, onDelete }) => {
    const { businessId } = useUser();
    
    const { data: service, isLoading } = useQuery({
      queryKey: ['service', businessId, item.serviceId],
      queryFn: () => serviceApi.getServiceById(businessId, item.serviceId)
    });
  
    if (isLoading) return <div className="animate-pulse h-24 bg-muted rounded-lg"></div>;

    const formatPrice = (price) => {
        return new Intl.NumberFormat('en-US', {
          style: 'currency',
          currency: 'USD'
        }).format(price);
      };
  
    return (
      <div className="flex items-center justify-between p-4 border rounded-lg hover:bg-accent/50 transition-colors">
        <div className="space-y-1 flex-1">
          <div className="flex justify-between items-start">
            <div>
              <p className="font-medium">{service.serviceName}</p>
              <p className="text-sm text-muted-foreground">{service.serviceDescription}</p>
            </div>
            <Badge variant="outline" className="ml-2 flex items-center gap-1">
              <Clock className="h-4 w-4" />
              {service.serviceDuration} min
            </Badge>
          </div>
          <div className="text-sm text-muted-foreground">
            <p>Unit Price: {formatPrice(item.unitPrice)}</p>
            {service.isServiceActive ? (
              <Badge variant="success" className="mt-1">Active</Badge>
            ) : (
              <Badge variant="destructive" className="mt-1">Inactive</Badge>
            )}
          </div>
        </div>
        {(role === 'BusinessOwner' || role === 'Employee') && order.orderStatus !== 'cancelled' && (
          <Button
            variant="ghost"
            size="icon"
            onClick={() => onDelete(item.orderItemId)}
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        )}
      </div>
    );
};
  

export default ServiceOrderItem;  