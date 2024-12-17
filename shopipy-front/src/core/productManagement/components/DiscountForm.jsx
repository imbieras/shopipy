import { useState, useEffect } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group"
import { Label } from "@/components/ui/label"

export function DiscountForm({ onApplyDiscount, onRemoveDiscount, currentDiscount }) {
  const [discountType, setDiscountType] = useState('percentage')
  const [discountValue, setDiscountValue] = useState('')

  useEffect(() => {
    if (currentDiscount) {
      setDiscountType(currentDiscount.type)
      setDiscountValue(currentDiscount.value.toString())
    } else {
      setDiscountType('percentage')
      setDiscountValue('')
    }
  }, [currentDiscount])

  const handleApplyDiscount = () => {
    const value = parseFloat(discountValue)
    if (!isNaN(value) && value > 0) {
      onApplyDiscount({ type: discountType, value })
    }
  }

  return (
    <div className="mt-6 space-y-4">
      <h3 className="font-semibold">Apply Discount</h3>
      <RadioGroup 
        value={discountType} 
        onValueChange={(value) => setDiscountType(value)}
      >
        <div className="flex items-center space-x-2">
          <RadioGroupItem value="percentage" id="percentage" />
          <Label htmlFor="percentage">Percentage</Label>
        </div>
        <div className="flex items-center space-x-2">
          <RadioGroupItem value="amount" id="amount" />
          <Label htmlFor="amount">Fixed Amount</Label>
        </div>
      </RadioGroup>
      <div className="flex space-x-2">
        <Input
          type="number"
          placeholder={discountType === 'percentage' ? 'Enter percentage' : 'Enter amount'}
          value={discountValue}
          onChange={(e) => setDiscountValue(e.target.value)}
          className="w-full"
        />
        <Button onClick={handleApplyDiscount}>Apply</Button>
      </div>
    </div>
  )
}

