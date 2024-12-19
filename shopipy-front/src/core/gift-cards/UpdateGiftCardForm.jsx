'use client'

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { giftCardApi } from "@/core/gift-cards/services/GiftCardApi"

export default function UpdateGiftCardForm({ giftCard, onClose }) {
  return (
    <form action={ giftCardApi.updateGiftCard } className="space-y-4 mb-4">
      <input type="hidden" name="giftCardId" value={giftCard.giftCardId} />
      <div>
        <Label htmlFor="amountLeft">Remaining Amount</Label>
        <Input id="amountLeft" name="amountLeft" type="number" step="0.01" defaultValue={giftCard.amountLeft} required />
      </div>
      <div>
        <Label htmlFor="validFrom">Valid From</Label>
        <Input id="validFrom" name="validFrom" type="date" defaultValue={giftCard.validFrom} required />
      </div>
      <div>
        <Label htmlFor="validUntil">Valid Until</Label>
        <Input id="validUntil" name="validUntil" type="date" defaultValue={giftCard.validUntil} required />
      </div>
      <Button type="submit">Update Gift Card</Button>
      <Button type="button" variant="outline" onClick={onClose} className="ml-2">Cancel</Button>
    </form>
  )
}
