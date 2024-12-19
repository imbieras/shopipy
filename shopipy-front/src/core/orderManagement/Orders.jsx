import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Search } from "lucide-react";
import { Input } from "@/components/ui/input";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import { ordersApi } from "./services/ordersApi";
import { useBusiness } from "@/hooks/useUser";
import { OrderListItem } from "./components/OrderListItem";

export default function Orders() {
  const { businessId } = useBusiness();
  const queryClient = useQueryClient();
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");

  const { data: orders = [], isLoading } = useQuery({
    queryKey: ["orders", businessId],
    queryFn: () => ordersApi.getOrders(businessId),
  });

  const cancelOrderMutation = useMutation({
    mutationFn: (orderId) => ordersApi.cancelOrder(businessId, orderId),
    onSuccess: () => {
      queryClient.invalidateQueries(["orders", businessId]);
      setSelectedOrder(null);
    },
  });

  const handleOrderClick = (orderId) => {
    setSelectedOrder(orderId === selectedOrder ? null : orderId);
  };

  const handleCancelOrder = async (orderId) => {
    if (window.confirm("Are you sure you want to cancel this order?")) {
      await cancelOrderMutation.mutateAsync(orderId);
    }
  };

  const filteredOrders = orders.filter(
    (order) =>
      order.customerName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      order.orderId?.toString().includes(searchTerm)
  );

  // Separate active and cancelled orders
  const activeOrders = filteredOrders.filter(
    (order) => order.status !== "cancelled"
  );
  const cancelledOrders = filteredOrders.filter(
    (order) => order.status === "cancelled"
  );

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-96">Loading...</div>
    );
  }

  return (
    <div className="container mx-auto p-4 max-w-4xl">
      <h1 className="text-2xl font-bold mb-4">Orders</h1>
      <div className="mb-4 relative">
        <Search className="absolute left-2 top-1/2 transform -translate-y-1/2 text-gray-400" />
        <Input
          type="text"
          placeholder="Search by customer name or order ID"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="pl-10"
        />
      </div>
      <ScrollArea className="h-[calc(100vh-200px)]">
        <div className="space-y-2">
          <h2 className="text-lg font-semibold">Active Orders</h2>
          {activeOrders.map((order) => (
            <OrderListItem
              key={order.orderId}
              order={order}
              isExpanded={selectedOrder === order.orderId}
              onToggle={() => handleOrderClick(order.orderId)}
              onCancel={handleCancelOrder}
            />
          ))}

          {cancelledOrders.length > 0 && (
            <>
              <Separator className="my-6" />
              <h2 className="text-lg font-semibold text-gray-600">
                Cancelled Orders
              </h2>
              {cancelledOrders.map((order) => (
                <OrderListItem
                  key={order.orderId}
                  order={order}
                  isExpanded={selectedOrder === order.orderId}
                  onToggle={() => handleOrderClick(order.orderId)}
                  onCancel={handleCancelOrder}
                />
              ))}
            </>
          )}
        </div>
      </ScrollArea>
    </div>
  );
}
