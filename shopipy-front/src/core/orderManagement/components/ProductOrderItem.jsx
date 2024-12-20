import { useQuery } from "@tanstack/react-query";
import { productsApi } from "@/core/productManagement/services/productsApi";
import { discountsApi } from "@/core/discountsManagement/services/DiscountsApi"; // Import your discount API
import { useBusiness } from "@/hooks/useUser";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Pencil, Trash2 } from "lucide-react";
import { useState } from "react";

const ProductOrderItem = ({ item, order, role, onEdit, onDelete, onDiscount }) => {
  const { businessId } = useBusiness();
  const [selectedDiscount, setSelectedDiscount] = useState(null);

  const { data: product, isLoading: isProductLoading } = useQuery({
    queryKey: ["product", businessId, item.productId],
    queryFn: () => productsApi.getProductById(businessId, item.productId),
  });

  const { data: discounts = [], isLoading: isDiscountsLoading } = useQuery({
    queryKey: ["discounts", businessId],
    queryFn: () => discountsApi.getDiscounts(businessId),
  });

  const formatPrice = (price) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(price);
  };

  const applyDiscount = (discount) => {
    const discountedPrice =
      discount.discountType === "Percentage"
        ? item.unitPrice - (item.unitPrice * discount.discountValue) / 100
        : item.unitPrice - discount.discountValue;

    setSelectedDiscount({
      ...discount,
      discountedPrice,
    });

    if (onDiscount) {
      onDiscount(item.orderItemId, discount, discountedPrice);
    }
  };

  if (isProductLoading || isDiscountsLoading)
    return <div className="animate-pulse h-24 bg-muted rounded-lg"></div>;

  return (
    <div className="flex items-center justify-between p-4 border rounded-lg hover:bg-accent/50 transition-colors">
      <div className="space-y-1 flex-1">
        <div className="flex justify-between items-start">
          <div>
            <p className="font-medium">{product.name}</p>
            <p className="text-sm text-muted-foreground">
              {product.description}
            </p>
          </div>
          <Badge variant="outline" className="ml-2">
            {product.productState}
          </Badge>
        </div>
        <div className="text-sm text-muted-foreground space-y-1">
          <p>Quantity: {item.productQuantity}</p>
          <p>
            Unit Price:{" "}
            {selectedDiscount
              ? `${formatPrice(selectedDiscount.discountedPrice)} (Discounted)`
              : formatPrice(item.unitPrice)}
          </p>
          <p>
            Total:{" "}
            {selectedDiscount
              ? `${formatPrice(
                  selectedDiscount.discountedPrice * item.productQuantity
                )} (Discounted)`
              : formatPrice(item.unitPrice * item.productQuantity)}
          </p>
        </div>
      </div>
      {(role === "BusinessOwner" || role === "Employee") &&
        order.orderStatus !== "cancelled" && (
          <div className="flex flex-col gap-2 ml-4">
            <Select
              onValueChange={(discountId) => {
                const selected = discounts.find(
                  (discount) => discount.discountId === parseInt(discountId)
                );
                if (selected) applyDiscount(selected);
              }}
            >
              <SelectTrigger>
                <SelectValue placeholder="Apply Discount" />
              </SelectTrigger>
              <SelectContent>
                {discounts.map((discount) => (
                  <SelectItem key={discount.discountId} value={discount.discountId.toString()}>
                    {discount.name} -{" "}
                    {discount.discountType === "Percentage"
                      ? `${discount.discountValue}%`
                      : `$${discount.discountValue}`}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            <div className="flex gap-2">
              <Button variant="ghost" size="icon" onClick={() => onEdit(item)}>
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
          </div>
        )}
    </div>
  );
};

export default ProductOrderItem;
