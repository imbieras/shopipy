import { useState } from "react";
import { ProductList } from "./components/ProductList";
import { OrderSummary } from "./components/OrderSummary";
import { Input } from "@/components/ui/input";
import { Search } from "lucide-react";
import { CategoryFilter } from "../categoryManagement/components/CategoryFilter";
import { DiscountForm } from "./components/DiscountForm";
import { PaymentSelection } from "./components/PaymentSelection";
import { TipSelection } from "./components/TipSelection";
import { useQuery } from "@tanstack/react-query";
import { categoryApi } from "../categoryManagement/services/CategoryApi";
import { useUser } from "@/hooks/useUser";

export default function Products() {
  const { businessId } = useUser();
  const [order, setOrder] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [discount, setDiscount] = useState(null);
  const [tip, setTip] = useState(0);
  const [remainingBalance, setRemainingBalance] = useState(0);

  const { data: categories = [], isLoading: isCategoriesLoading } = useQuery({
    queryKey: ["categories", businessId],
    queryFn: async () => await categoryApi.getCategories(businessId),
    enabled: !!businessId,
  });

  const addToOrder = (product, variation) => {
    setOrder((prevOrder) => {
      const existingItem = prevOrder.find(
        (item) =>
          item.productId === product.productId &&
          item.selectedVariation?.variationId === variation?.variationId
      );
      if (existingItem) {
        return prevOrder.map((item) =>
          item.productId === product.productId &&
          item.selectedVariation?.variationId === variation?.variationId
            ? { ...item, quantity: item.quantity + 1 }
            : item
        );
      }
  
      const basePrice = product.basePrice;
      const variationPrice = variation 
        ? basePrice + variation.priceModifier 
        : basePrice;
  
      return [
        ...prevOrder,
        {
          productId: product.productId,
          name: product.name,
          basePrice: basePrice,
          quantity: 1,
          selectedVariation: variation ? {
            variationId: variation.variationId,
            name: variation.name,
            basePrice: variationPrice
          } : null
        },
      ];
    });
  };

  const removeFromOrder = (productId, variationId) => {
    setOrder((prevOrder) =>
      prevOrder.filter(
        (item) =>
          !(
            item.productId === productId &&
            item.selectedVariation?.variationId === variationId
          )
      )
    );
  };

  const updateQuantity = (productId, newQuantity, variationId) => {
    setOrder((prevOrder) =>
      prevOrder.map((item) =>
        item.productId === productId &&
        item.selectedVariation?.variationId === variationId
          ? { ...item, quantity: newQuantity }
          : item
      )
    );
  };

  const applyDiscount = (newDiscount) => {
    setDiscount(newDiscount);
  };

  const removeDiscount = () => {
    setDiscount(null);
  };

  const handleTip = (tipAmount) => {
    setTip(tipAmount);
  };

  const removeTip = () => {
    setTip(0);
  };

  const calculateTotal = () => {
    const subtotal = order.reduce(
      (sum, item) => sum + (item.selectedVariation?.price || item.price) * item.quantity,
      0
    );
    // const taxTotal = order.reduce(
    //   (sum, item) => sum + (item.selectedVariation?.price || item.price) * item.quantity * item.taxRate,
    //   0
    // );
    //again bullshit ahh tax you take care of it not me :[]
    const taxTotal = 0;
    let discountAmount = 0;
    if (discount) {
      if (discount.type === 'percentage') {
        discountAmount = subtotal * (discount.value / 100);
      } else {
        discountAmount = discount.value;
      }
    }
    return subtotal + taxTotal - discountAmount + tip;
  };

  const handlePayment = (amount, paymentType) => {
    const total = calculateTotal();
    // Comment out remaining balance logic for now
    // if (remainingBalance === 0) {
    //   setRemainingBalance(total);
    // }
    // const newRemainingBalance = Math.max(0, remainingBalance - amount);
    // setRemainingBalance(newRemainingBalance);
    
    //this shit causes NAN NAN NAN NAN DOLLARS
    
    // if (newRemainingBalance === 0) {
    //   alert("Payment completed!");
    // } else {
    //   alert(`Remaining balance: $${newRemainingBalance.toFixed(2)}`);
    // }
    
    alert("Payment processed");
  };

  if (isCategoriesLoading) return <div>Loading...</div>;

  return (
    <div className="flex flex-col h-screen">
      <div className="flex flex-1 overflow-hidden">
        <div className="flex-1 flex flex-col p-4 overflow-hidden">
          <div className="flex flex-col space-y-4 mb-4">
            <div className="relative">
              <Search className="absolute left-2 top-1/2 transform -translate-y-1/2 text-gray-400" />
              <Input
                type="text"
                placeholder="Search products..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-10"
              />
            </div>
            <div>
              <CategoryFilter 
                categories={categories}
                selectedCategory={selectedCategory}
                onSelectCategory={setSelectedCategory}
              />
            </div>
          </div>
          <ProductList
            searchTerm={searchTerm}
            selectedCategory={selectedCategory}
            onAddToOrder={addToOrder}
          />
        </div>
        <div className="w-1/3 bg-muted p-4 overflow-y-auto">
          <OrderSummary
            order={order}
            discount={discount}
            tip={tip}
            remainingBalance={remainingBalance}
            onRemoveItem={removeFromOrder}
            onUpdateQuantity={updateQuantity}
            onRemoveDiscount={removeDiscount}
            onRemoveTip={removeTip}
          />
          <DiscountForm 
            onApplyDiscount={applyDiscount} 
            onRemoveDiscount={removeDiscount}
            currentDiscount={discount}
          />
          <TipSelection subtotal={calculateTotal() - tip} onSelectTip={handleTip} />
          <PaymentSelection
            totalAmount={calculateTotal()}
            remainingAmount={remainingBalance === 0 ? calculateTotal() : remainingBalance}
            onProcessPayment={handlePayment}
          />
        </div>
      </div>
    </div>
  );
}