import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"


export function TipSelection({ subtotal, onSelectTip }) {
  const [customTip, setCustomTip] = useState("")

  const tipPercentages = [10, 15, 20]

  const handleTipSelection = (percentage) => {
    const tipAmount = (subtotal * percentage) / 100
    onSelectTip(tipAmount)
  }

  const handleCustomTip = () => {
    const tipAmount = parseFloat(customTip)
    if (!isNaN(tipAmount) && tipAmount >= 0) {
      onSelectTip(tipAmount)
    }
  }

  return (
    <div className="mt-6 space-y-4">
      <h3 className="font-semibold">Add Tip</h3>
      <div className="flex space-x-2">
        {tipPercentages.map((percentage) => (
          <Button
            key={percentage}
            variant="outline"
            onClick={() => handleTipSelection(percentage)}
            className="flex-1"
          >
            {percentage}%
          </Button>
        ))}
      </div>
      <div className="flex space-x-2">
        <Input
          type="number"
          placeholder="Custom tip amount"
          value={customTip}
          onChange={(e) => setCustomTip(e.target.value)}
          className="flex-1"
        />
        <Button onClick={handleCustomTip}>Add</Button>
      </div>
    </div>
  )
}

