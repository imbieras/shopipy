import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"

export function OrderSummary({
  order,
  discount,
  tip,
  remainingBalance,
  onRemoveItem,
  onUpdateQuantity,
  onRemoveDiscount,
  onRemoveTip,
}) {
    const subtotal = order.reduce(
        (sum, item) => sum + (item.selectedVariation?.basePrice || item.basePrice) * item.quantity,
        0
    );
  
  //tax bullshit im not touching rn
//   const taxTotal = order.reduce(
//     (sum, item) => sum + (item.selectedVariation?.BasePrice || item.BasePrice) * item.quantity * item.taxRate,
//     0
//   )
  const taxTotal = 0;

  let discountAmount = 0
  if (discount) {
    if (discount.type === 'percentage') {
      discountAmount = subtotal * (discount.value / 100)
    } else {
      discountAmount = discount.value
    }
  }

  const total = subtotal + taxTotal - discountAmount + tip

  return (
    <div className="space-y-4">
      <h2 className="text-xl font-bold">Order Summary</h2>
      {order.map((item) => (
        <div key={`${item.productId}-${item.selectedVariation?.variationId}`} className="flex flex-col space-y-2">
            <div className="flex items-center justify-between">
            <span className="font-medium">{item.name}</span>
            <div className="flex items-center space-x-2">
                <Input
                type="number"
                min="1"
                value={item.quantity}
                onChange={(e) =>
                    onUpdateQuantity(
                    item.productId, 
                    parseInt(e.target.value, 10), 
                    item.selectedVariation?.variationId
                    )
                }
                className="w-16"
                />
                <Button
                variant="destructive"
                size="sm"
                onClick={() => onRemoveItem(item.productId, item.selectedVariation?.variationId)}
                >
                Remove
                </Button>
            </div>
            </div>
            {item.selectedVariation && (
            <div className="flex justify-between text-sm text-muted-foreground">
                <span>{item.selectedVariation.name}</span>
                <span>${(item.selectedVariation.basePrice * item.quantity).toFixed(2)}</span>
            </div>
            )}
            {!item.selectedVariation && (
            <div className="flex justify-end text-sm text-muted-foreground">
                <span>${(item.basePrice * item.quantity).toFixed(2)}</span>
            </div>
            )}
        </div>
        ))}
      <div className="space-y-2 text-sm">
        <div className="flex justify-between">
          <span>Subtotal:</span>
          <span>${subtotal.toFixed(2)}</span>
        </div>
        <div className="flex justify-between">
          <span>Tax:</span>
          <span>${taxTotal.toFixed(2)}</span>
        </div>
        {discount && (
          <div className="flex justify-between text-green-600">
            <span>Discount:</span>
            <div className="flex items-center space-x-2">
              <span>-${discountAmount.toFixed(2)}</span>
              <Button variant="ghost" size="sm" onClick={onRemoveDiscount}>
                Remove
              </Button>
            </div>
          </div>
        )}
        {tip > 0 && (
          <div className="flex justify-between text-blue-600">
            <span>Tip:</span>
            <div className="flex items-center space-x-2">
              <span>${tip.toFixed(2)}</span>
              <Button variant="ghost" size="sm" onClick={onRemoveTip}>
                Remove
              </Button>
            </div>
          </div>
        )}
      </div>
      <div className="text-xl font-bold mt-4 flex justify-between">
        <span>Total:</span>
        <span>${total.toFixed(2)}</span>
      </div>
      <div className="text-lg font-semibold mt-2 flex justify-between text-blue-600">
        <span>Remaining Balance:</span>
        <span>${remainingBalance.toFixed(2)}</span>
      </div>
    </div>
  )
}

