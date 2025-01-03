import { useState } from "react";
import { useParams } from "react-router-dom";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { ordersApi } from "../services/ordersApi";
import { productsApi } from "@/core/productManagement/services/productsApi";
import { serviceApi } from "@/core/serviceManagement/services/ServiceApi";
import { categoryApi } from "@/core/categoryManagement/services/CategoryApi";
import { useBusiness, useUser } from "@/hooks/useUser";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import AddDiscountDialog from "./AddDiscountDialog";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTrigger,
  DialogTitle,
} from "@/components/ui/dialog";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem,
} from "@/components/ui/select";
import { CategoryFilter } from "@/core/categoryManagement/components/CategoryFilter";
import { Package, Wrench, Plus } from "lucide-react";
import ProductOrderItem from "./ProductOrderItem";
import ServiceOrderItem from "./ServiceOrderItem";
import { CardElement, useStripe, useElements } from "@stripe/react-stripe-js";

const useOrder = () => {
  const { orderId } = useParams();
  const { businessId } = useBusiness();
  const { data: order, isLoading: orderLoading } = useQuery({
    queryKey: ["order", businessId, orderId],
    queryFn: () => ordersApi.getOrderById(businessId, orderId, true),
  });
  return { order, orderLoading };
};

const StripeDialog = ({ paymentId, onPaymentSuccess }) => {
  const stripe = useStripe();
  const elements = useElements();
  const [processing, setProcessing] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const handleCardPayment = async () => {
    if (!stripe || !elements || !paymentId) return;
    setProcessing(true);
    setErrorMessage("");

    const cardElement = elements.getElement(CardElement);
    if (!cardElement) {
      setProcessing(false);
      return;
    }

    const { error, paymentIntent } = await stripe.confirmCardPayment(
      paymentId,
      {
        payment_method: {
          card: cardElement,
        },
      }
    );

    setProcessing(false);

    if (error) {
      console.error("Payment error:", error);
      setErrorMessage(error.message || "An unknown error occurred.");
    } else if (paymentIntent && paymentIntent.status === "succeeded") {
      console.log("Payment successful:", paymentIntent);
      if (onPaymentSuccess) {
        onPaymentSuccess(paymentIntent);
      }
    }
  };

  return (
    <div className="space-y-4">
      <div className="space-y-2">
        <label>Card Details</label>
        <CardElement />
      </div>
      {errorMessage && <p className="text-red-500">{errorMessage}</p>}
      <Button
        onClick={handleCardPayment}
        disabled={processing || !stripe || !elements}
        className="w-full bg-blue-600"
      >
        {processing ? "Processing..." : "Confirm Payment"}
      </Button>
    </div>
  );
};

const PaymentDialog = () => {
  const [isOpen, setIsOpen] = useState(false);
  const { order } = useOrder();
  const [paymentType, setPaymentType] = useState("Cash");
  const [paymentAmount, setPaymentAmount] = useState("");
  const { businessId } = useBusiness();
  const elements = useElements();
  const stripe = useStripe();
  const [paymentId, setPaymentId] = useState(undefined);

  const handlePayment = async (e) => {
    e.preventDefault();
    // Add payment handling logic here
    console.log("Payment Type:", paymentType);
    console.log("Payment Amount:", paymentAmount);
    var response = await ordersApi.createPayment(businessId, order.orderId, {
      amountPaid: paymentAmount,
      paymentMethod: paymentType,
    });
    if (response.stripePaymentId) {
      setPaymentId(response.stripePaymentId);
    }
  };

  const handleStripeSuccess = (paymentIntent) => {
    // Handle successful Stripe payment here
    console.log("Stripe payment succeeded:", paymentIntent);
    // You might want to close the dialog, or refresh the order
    setIsOpen(false);
  };

  if (!order) return null;

  return (
    <Dialog open={isOpen} onOpenChange={setIsOpen}>
      <DialogTrigger asChild>
        <Button
          className="flex items-center gap-2 bg-blue-600"
          disabled={order.orderStatus !== "Open"}
        >
          Pay
        </Button>
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Payment</DialogTitle>
        </DialogHeader>
        {paymentId ? (
          <StripeDialog
            paymentId={paymentId}
            onPaymentSuccess={handleStripeSuccess}
          />
        ) : (
          <form onSubmit={handlePayment} className="space-y-4">
            <div className="space-y-2">
              <label>Payment Type</label>
              <Select
                value={paymentType}
                onValueChange={(value) => {
                  setPaymentType(value);
                }}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select type" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Cash">Cash</SelectItem>
                  <SelectItem value="Card">Card</SelectItem>
                  <SelectItem value="GiftCard">GiftCard</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <label htmlFor="paymentAmount">Payment Amount</label>
              <Input
                id="paymentAmount"
                type="number"
                min="0"
                step="0.01"
                value={paymentAmount}
                onChange={(e) => setPaymentAmount(e.target.value)}
                required
                placeholder="Enter payment amount"
              />
            </div>

            <Button type="submit" className="w-full" disabled={!paymentAmount}>
              Pay
            </Button>
          </form>
        )}
      </DialogContent>
    </Dialog>
  );
};

export default function OrderDetails() {
  const { orderId } = useParams();
  const { role } = useUser();
  const { businessId } = useBusiness();
  const queryClient = useQueryClient();
  const [editItem, setEditItem] = useState(null);
  const [quantity, setQuantity] = useState("");
  const [isAddItemOpen, setIsAddItemOpen] = useState(false);
  const [itemType, setItemType] = useState("product");
  const [selectedItemId, setSelectedItemId] = useState("");
  const [newQuantity, setNewQuantity] = useState("1");
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [isDiscountOpen, setIsDiscountOpen] = useState(false);
  const [discountAmount, setDiscountAmount] = useState("");
  const { order, orderLoading } = useOrder();

  const { data: categories = [] } = useQuery({
    queryKey: ["categories", businessId],
    queryFn: () => categoryApi.getCategories(businessId),
  });

  const { data: productItems = [], isLoading: productsLoading } = useQuery({
    queryKey: ["orderProductItems", businessId, orderId],
    queryFn: async () => {
      try {
        const response = await ordersApi.getProductItems(businessId, orderId);
        return response;
      } catch (error) {
        if (error.response?.status === 404) {
          return [];
        }
        throw error;
      }
    },
  });

  const { data: serviceItems = [], isLoading: servicesLoading } = useQuery({
    queryKey: ["orderServiceItems", businessId, orderId],
    queryFn: async () => {
      try {
        const response = await ordersApi.getServiceItems(businessId, orderId);
        return response;
      } catch (error) {
        if (error.response?.status === 404) {
          return [];
        }
        throw error;
      }
    },
  });

  const { data: availableProducts = [] } = useQuery({
    queryKey: ["products", businessId, selectedCategory],
    queryFn: async () => {
      const response = await productsApi.getAllProducts(businessId, {
        categoryId: selectedCategory,
      });
      return response.data;
    },
  });

  const { data: availableServices = [] } = useQuery({
    queryKey: ["services", businessId],
    queryFn: () => serviceApi.getServices(businessId),
  });

  // Filter services based on selected category
  const filteredServices = selectedCategory
    ? availableServices.filter(
        (service) => service.categoryId === selectedCategory
      )
    : availableServices;

  const updateItemMutation = useMutation({
    mutationFn: ({ orderItemId, data }) =>
      ordersApi.updateOrderItem(businessId, orderId, orderItemId, data),
    onSuccess: () => {
      queryClient.invalidateQueries(["orderProductItems", businessId, orderId]);
      queryClient.invalidateQueries(["orderServiceItems", businessId, orderId]);
      setEditItem(null);
    },
  });

  const deleteItemMutation = useMutation({
    mutationFn: (orderItemId) =>
      ordersApi.deleteOrderItem(businessId, orderId, orderItemId),
    onSuccess: () => {
      queryClient.invalidateQueries(["orderProductItems", businessId, orderId]);
      queryClient.invalidateQueries(["orderServiceItems", businessId, orderId]);
    },
  });

  const createItemMutation = useMutation({
    mutationFn: (orderItemData) =>
      ordersApi.createOrderItem(businessId, orderId, orderItemData),
    onSuccess: () => {
      queryClient.invalidateQueries(["orderProductItems", businessId, orderId]);
      queryClient.invalidateQueries(["orderServiceItems", businessId, orderId]);
      setIsAddItemOpen(false);
      resetAddItemForm();
    },
  });

  
  const handleUpdateItem = (e) => {
    e.preventDefault();
    updateItemMutation.mutate({
      orderItemId: editItem.orderItemId,
      data: { productQuantity: parseInt(quantity) },
    });
  };

  const handleDeleteItem = (orderItemId) => {
    if (window.confirm("Are you sure you want to delete this item?")) {
      deleteItemMutation.mutate(orderItemId);
    }
  };

  const handleAddItem = (e) => {
    e.preventDefault();
    const itemData = {
      ...(itemType === "product"
        ? {
            ProductId: parseInt(selectedItemId),
            productQuantity: parseInt(newQuantity),
          }
        : {
            ServiceId: parseInt(selectedItemId),
          }),
    };
    createItemMutation.mutate(itemData);
  };

  const resetAddItemForm = () => {
    setItemType("product");
    setSelectedItemId("");
    setNewQuantity("1");
    setSelectedCategory(null);
  };

  const getStatusColor = (orderStatus) => {
    if (!orderStatus) return "bg-gray-500";

    switch (orderStatus.toLowerCase()) {
      case "pending":
        return "bg-yellow-500";
      case "inprogress":
        return "bg-blue-500";
      case "completed":
        return "bg-green-500";
      case "cancelled":
        return "bg-red-500";
      default:
        return "bg-gray-500";
    }
  };

  const formatPrice = (price) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(price);
  };

  const calculateTotalPrice = () => {
    let total = 0;

    if (productItems?.length > 0) {
      total += productItems.reduce(
        (sum, item) => sum + item.unitPrice * item.productQuantity,
        0
      );
    }

    if (serviceItems?.length > 0) {
      total += serviceItems.reduce((sum, item) => sum + item.unitPrice, 0);
    }

    return formatPrice(total);
  };

  if (orderLoading || productsLoading || servicesLoading) {
    return (
      <div className="flex justify-center items-center h-96">Loading...</div>
    );
  }


  const handleAddDiscount = async (e) => {
    e.preventDefault();
    try {
      await ordersApi.applyDiscount(businessId, orderId, {
        discountAmount: parseFloat(discountAmount),
      });
      queryClient.invalidateQueries(["order", businessId, orderId]);
      setIsDiscountOpen(false);
      setDiscountAmount("");
    } catch (error) {
      console.error("Failed to apply discount:", error);
    }
  };
  
  // Render Section
  <Dialog open={isDiscountOpen} onOpenChange={setIsDiscountOpen}>
    <DialogTrigger asChild>
      <Button className="flex items-center gap-2" disabled={order.orderStatus === "cancelled"}>
        Add Discount
      </Button>
    </DialogTrigger>
    <DialogContent>
      <DialogHeader>
        <DialogTitle>Apply Discount</DialogTitle>
      </DialogHeader>
      <form onSubmit={handleAddDiscount} className="space-y-4">
        <div className="space-y-2">
          <label htmlFor="discountAmount">Discount Amount</label>
          <Input
            id="discountAmount"
            type="number"
            min="0"
            step="0.01"
            value={discountAmount}
            onChange={(e) => setDiscountAmount(e.target.value)}
            required
            placeholder="Enter discount amount"
          />
        </div>
        <Button type="submit" className="w-full" disabled={!discountAmount}>
          Apply Discount
        </Button>
      </form>
    </DialogContent>
  </Dialog>


  return (
    <Card className="max-w-4xl mx-auto my-8">
      <CardHeader>
        <div className="flex justify-between items-center">
          <CardTitle>Order #{orderId}</CardTitle>
          <p className="text-lg font-medium">Total: {calculateTotalPrice()}</p>
          <div className="flex gap-2 items-center">
            <PaymentDialog />
            <Dialog open={isAddItemOpen} onOpenChange={setIsAddItemOpen}>
              <DialogTrigger asChild>
                <Button
                  className="flex items-center gap-2"
                  disabled={order.orderStatus === "cancelled"}
                >
                  <Plus className="h-4 w-4" /> Add Item
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Add Order Item</DialogTitle>
                </DialogHeader>
                <form onSubmit={handleAddItem} className="space-y-4">
                  <div className="space-y-2">
                    <label>Item Type</label>
                    <Select
                      value={itemType}
                      onValueChange={(value) => {
                        setItemType(value);
                        setSelectedItemId("");
                      }}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Select type" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="product">Product</SelectItem>
                        <SelectItem value="service">Service</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  <div className="space-y-2">
                    <label>Category</label>
                    <CategoryFilter
                      categories={categories}
                      selectedCategory={selectedCategory}
                      onSelectCategory={(value) => {
                        setSelectedCategory(value);
                        setSelectedItemId("");
                      }}
                    />
                  </div>

                  <div className="space-y-2">
                    <label>
                      {itemType === "product" ? "Product" : "Service"}
                    </label>
                    <Select
                      value={selectedItemId}
                      onValueChange={setSelectedItemId}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder={`Select ${itemType}`} />
                      </SelectTrigger>
                      <SelectContent>
                        {itemType === "product"
                          ? availableProducts.map((product) => (
                              <SelectItem
                                key={product.productId}
                                value={product.productId.toString()}
                              >
                                {product.name} -{" "}
                                {formatPrice(product.basePrice)}
                              </SelectItem>
                            ))
                          : filteredServices.map((service) => (
                              <SelectItem
                                key={service.serviceId}
                                value={service.serviceId.toString()}
                              >
                                {service.serviceName} -{" "}
                                {formatPrice(service.servicePrice)}
                              </SelectItem>
                            ))}
                      </SelectContent>
                    </Select>
                  </div>

                  {itemType === "product" && (
                    <div className="space-y-2">
                      <label htmlFor="quantity">Quantity</label>
                      <Input
                        id="quantity"
                        type="number"
                        min="1"
                        value={newQuantity}
                        onChange={(e) => setNewQuantity(e.target.value)}
                        required
                      />
                    </div>
                  )}

                  <Button
                    type="submit"
                    className="w-full"
                    disabled={!selectedItemId}
                  >
                    Add Item
                  </Button>
                </form>
              </DialogContent>
            </Dialog>
            <Dialog open={isDiscountOpen} onOpenChange={setIsDiscountOpen}>
            <DialogTrigger asChild>
              <Button className="flex items-center gap-2" disabled={order.orderStatus === "cancelled"}>
                Add Discount
              </Button>
            </DialogTrigger>
            <DialogContent>
                {<AddDiscountDialog
              orderId={orderId}
              orderStatus={order.orderStatus}
              businessId={businessId}
            />} 
            </DialogContent>
          </Dialog>
            <Badge
              className={`${getStatusColor(order.orderStatus)} text-white`}
            >
              {order.orderStatus}
            </Badge>
          </div>
        </div>
      </CardHeader>
      <CardContent className="space-y-6">
        {/* Product Items Section */}
        {productItems?.length > 0 ? (
          <div className="space-y-4">
            <div className="flex items-center gap-2">
              <Package className="h-5 w-5" />
              <h3 className="text-lg font-semibold">Products</h3>
            </div>
            <div className="space-y-3">
              {productItems.map((item) => (
                <ProductOrderItem
                  key={item.orderItemId}
                  item={item}
                  order={order}
                  role={role}
                  onEdit={(item) => {
                    setEditItem(item);
                    setQuantity(item.productQuantity.toString());
                  }}
                  onDelete={handleDeleteItem}
                />
              ))}
            </div>
          </div>
        ) : (
          !productsLoading && (
            <div className="text-center py-4 text-muted-foreground">
              No product items in this order
            </div>
          )
        )}

        {/* Service Items Section */}
        {serviceItems?.length > 0 ? (
          <>
            {productItems?.length > 0 && <Separator className="my-6" />}
            <div className="space-y-4">
              <div className="flex items-center gap-2">
                <Wrench className="h-5 w-5" />
                <h3 className="text-lg font-semibold">Services</h3>
              </div>
              <div className="space-y-3">
                {serviceItems.map((item) => (
                  <ServiceOrderItem
                    key={item.orderItemId}
                    item={item}
                    order={order}
                    role={role}
                    onDelete={handleDeleteItem}
                  />
                ))}
              </div>
            </div>
          </>
        ) : (
          !servicesLoading && (
            <div className="text-center py-4 text-muted-foreground">
              No service items in this order
            </div>
          )
        )}
      </CardContent>

      <Dialog
        open={editItem !== null}
        onOpenChange={(open) => !open && setEditItem(null)}
      >
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Edit Product Quantity</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleUpdateItem} className="space-y-4">
            <div className="space-y-2">
              <label htmlFor="quantity">Quantity</label>
              <Input
                id="quantity"
                type="number"
                min="1"
                value={quantity}
                onChange={(e) => setQuantity(e.target.value)}
                required
              />
            </div>
            <Button type="submit" className="w-full">
              Update Quantity
            </Button>
          </form>
        </DialogContent>
      </Dialog>
    </Card>
  );
}
