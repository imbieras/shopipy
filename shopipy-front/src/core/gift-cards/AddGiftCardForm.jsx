'use client'

import { useState } from 'react'
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { giftCardApi } from "@/core/gift-cards/services/GiftCardApi"

export default function AddGiftCardForm() {
  const [isOpen, setIsOpen] = useState(false)

  if (!isOpen) {
    return <Button onClick={() => setIsOpen(true)}>Add Gift Card</Button>
  }

  return (
    <form action={giftCardApi.addGiftCard} className="space-y-4 mb-4">
      <div>
        <Label htmlFor="amountOriginal">Original Amount</Label>
        <Input id="amountOriginal" name="amountOriginal" type="number" step="0.01" required />
      </div>
      <div>
        <Label htmlFor="validFrom">Valid From</Label>
        <Input id="validFrom" name="validFrom" type="date" required />
      </div>
      <div>
        <Label htmlFor="validUntil">Valid Until</Label>
        <Input id="validUntil" name="validUntil" type="date" required />
      </div>
      <Button type="submit">Add Gift Card</Button>
      <Button type="button" variant="outline" onClick={() => setIsOpen(false)} className="ml-2">Cancel</Button>
    </form>
  )
}

