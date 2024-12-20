'use client'

import { useState } from 'react'
import { Button } from "@/components/ui/button"
import { 
  Table, 
  TableBody, 
  TableCell, 
  TableHead, 
  TableHeader, 
  TableRow 
} from "@/components/ui/table"
import { giftCardApi } from "@/core/gift-cards/services/GiftCardApi"
import UpdateGiftCardForm from "./UpdateGiftCardForm";

export default function GiftCardList({ giftCards }) {
  const [editingGiftCard, setEditingGiftCard] = useState(null)

  return (
    <div>
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Code</TableHead>
            <TableHead>Original Amount</TableHead>
            <TableHead>Remaining Amount</TableHead>
            <TableHead>Valid From</TableHead>
            <TableHead>Valid Until</TableHead>
            <TableHead>Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {giftCards.map((giftCard) => (
            <TableRow key={giftCard.giftCardId}>
              <TableCell>{giftCard.giftCardCode}</TableCell>
              <TableCell>${giftCard.amountOriginal.toFixed(2)}</TableCell>
              <TableCell>${giftCard.amountLeft.toFixed(2)}</TableCell>
              <TableCell>{new Date(giftCard.validFrom).toLocaleDateString()}</TableCell>
              <TableCell>{new Date(giftCard.validUntil).toLocaleDateString()}</TableCell>
              <TableCell>
                <Button onClick={() => setEditingGiftCard(giftCard)} className="mr-2">Edit</Button>
                <Button onClick={() => giftCardApi.deleteGiftCard(giftCard.giftCardId)} variant="destructive">Delete</Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
      {editingGiftCard && (
        <UpdateGiftCardForm giftCard={editingGiftCard} onClose={() => setEditingGiftCard(null)} />
      )}
    </div>
  )
}

