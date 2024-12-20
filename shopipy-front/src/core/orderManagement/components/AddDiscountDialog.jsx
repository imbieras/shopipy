import React, { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { useQueryClient, useQuery } from "@tanstack/react-query";
import { ordersApi } from "../services/ordersApi";
import { discountsApi } from "@/core/discountsManagement/services/DiscountsApi";

const AddDiscountDialog = ({ orderId, orderStatus, businessId }) => {
  const [isDiscountOpen, setIsDiscountOpen] = useState(true); // Automatically open the form
  const [discountAmount, setDiscountAmount] = useState("");
  const [selectedDiscount, setSelectedDiscount] = useState(null);
  const queryClient = useQueryClient();

  const { data: discounts = [], isLoading: isDiscountsLoading } = useQuery({
    queryKey: ["discounts", businessId],
    queryFn: () => discountsApi.getDiscounts(businessId),
  });

  const handleAddDiscount = async (e) => {
    e.preventDefault();
    try {
      await ordersApi.applyDiscount(businessId, orderId, {
        discountId: selectedDiscount?.discountId,
        discountAmount: parseFloat(discountAmount),
      });
      queryClient.invalidateQueries(["order", businessId, orderId]);
      setIsDiscountOpen(false);
      setDiscountAmount("");
      setSelectedDiscount(null);
    } catch (error) {
      console.error("Failed to apply discount:", error);
    }
  };

  return (
    <Dialog open={isDiscountOpen} onOpenChange={setIsDiscountOpen}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Apply Discount</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleAddDiscount} className="space-y-4">
          <div className="space-y-2">
            <label htmlFor="discount">Available Discounts</label>
            <Select
              onValueChange={(discountId) => {
                const selected = discounts.find(
                  (discount) => discount.discountId === parseInt(discountId)
                );
                setSelectedDiscount(selected);
                setDiscountAmount(
                  selected.discountType === "Percentage"
                    ? (selected.discountValue / 100).toFixed(2)
                    : selected.discountValue
                );
              }}
            >
              <SelectTrigger>
                <SelectValue placeholder="Choose a discount" />
              </SelectTrigger>
              <SelectContent>
                {isDiscountsLoading ? (
                  <div className="text-muted-foreground">Loading discounts...</div>
                ) : (
                  discounts.map((discount) => (
                    <SelectItem key={discount.discountId} value={discount.discountId.toString()}>
                      {discount.name} -{" "}
                      {discount.discountType === "Percentage"
                        ? `${discount.discountValue}%`
                        : `$${discount.discountValue}`}
                    </SelectItem>
                  ))
                )}
              </SelectContent>
            </Select>
          </div>
          <Button type="submit" className="w-full" disabled={!selectedDiscount && !discountAmount}>
            Apply Discount
          </Button>
        </form>
      </DialogContent>
    </Dialog>
  );
};

export default AddDiscountDialog;
