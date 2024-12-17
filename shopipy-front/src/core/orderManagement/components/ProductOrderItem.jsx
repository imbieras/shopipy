import { useQuery } from '@tanstack/react-query';
import { productsApi } from '@/core/productManagement/services/productsApi';
import { useUser } from '@/hooks/useUser';
import { Button } from "@/components/ui/button";
import { Badge } from '@/components/ui/badge';
import { Pencil, Trash2 } from "lucide-react";

const ProductOrderItem = ({ item, order, role, onEdit, onDelete }) => {
  const { businessId } = useUser();
  
  const { data: product, isLoading } = useQuery({
    queryKey: ['product', businessId, item.productId],
    queryFn: () => productsApi.getProductById(businessId, item.productId)
  });

  const formatPrice = (price) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(price);
  };

  if (isLoading) return <div className="animate-pulse h-24 bg-muted rounded-lg"></div>;

  return (
    <div className="flex items-center justify-between p-4 border rounded-lg hover:bg-accent/50 transition-colors">
      <div className="space-y-1 flex-1">
        <div className="flex justify-between items-start">
          <div>
            <p className="font-medium">{product.name}</p>
            <p className="text-sm text-muted-foreground">{product.description}</p>
          </div>
          <Badge variant="outline" className="ml-2">
            {product.productState}
          </Badge>
        </div>
        <div className="text-sm text-muted-foreground space-y-1">
          <p>Quantity: {item.productQuantity}</p>
          <p>Unit Price: {formatPrice(item.unitPrice)}</p>
          <p>Total: {formatPrice(item.unitPrice * item.productQuantity)}</p>
        </div>
      </div>
      {(role === 'BusinessOwner' || role === 'Employee') && order.orderStatus !== 'cancelled' && (
        <div className="flex gap-2 ml-4">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => onEdit(item)}
          >
            <Pencil className="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="icon"
            onClick={() => onDelete(item.orderItemId)}
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>
      )}
    </div>
  );
};

export default ProductOrderItem;