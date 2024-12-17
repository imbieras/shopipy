import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"

export function PaymentSelection({ totalAmount, remainingAmount, onProcessPayment }) {
  const [paymentAmount, setPaymentAmount] = useState("")
  const [selectedPaymentType, setSelectedPaymentType] = useState()

  const paymentTypes = [
    { id: "cash", label: "Cash" },
    { id: "card", label: "Card" },
    { id: "giftcard", label: "Gift Card" },
  ]

  const handleProcessPayment = () => {
    const amount = parseFloat(paymentAmount)
    if (!isNaN(amount) && amount > 0 && amount <= remainingAmount && selectedPaymentType) {
      onProcessPayment(amount, selectedPaymentType)
      setPaymentAmount("")
      setSelectedPaymentType(undefined)
    } else {
      alert("Please enter a valid payment amount and select a payment type")
    }
  }

  return (
    <div className="mt-6 space-y-4">
      <h3 className="font-semibold">Process Payment</h3>
      <div className="flex items-center space-x-2">
        <Input
          type="number"
          step="0.01"
          placeholder="Enter payment amount"
          value={paymentAmount}
          onChange={(e) => setPaymentAmount(e.target.value)}
          className="flex-1"
        />
        <Select onValueChange={setSelectedPaymentType} value={selectedPaymentType}>
          <SelectTrigger className="w-[180px]">
            <SelectValue placeholder="Payment Type" />
          </SelectTrigger>
          <SelectContent>
            {paymentTypes.map((type) => (
              <SelectItem key={type.id} value={type.id}>
                {type.label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>
      <div className="flex justify-between items-center">
        {/* <span>Remaining: ${remainingAmount.toFixed(2)}</span> */}
        <Button onClick={handleProcessPayment} disabled={remainingAmount === 0}>
          Process Payment
        </Button>
      </div>
    </div>
  )
}

