'use client';

import { useState } from "react";
import { giftCardApi } from "@/core/gift-cards/services/GiftCardApi";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useUser } from "@/hooks/useUser";

export default function GiftCardsPage() {
  const { businessId } = useUser();
  const [giftCards, setGiftCards] = useState([]);
  const [editingGiftCard, setEditingGiftCard] = useState(null);
  const [isAdding, setIsAdding] = useState(false);

const fetchGiftCards = async () => {
  try {
    const response = await giftCardApi.getGiftCards(businessId);
    console.log("Fetched Gift Cards:", response); // Debug log
    setGiftCards(Array.isArray(response.data) ? response.data : []); // Extract `data` array
  } catch (error) {
    console.error("Error fetching gift cards:", error);
  }
};

  const handleDelete = async (giftCardId) => {
    try {
      await giftCardApi.deleteGiftCard(businessId, giftCardId);
      fetchGiftCards();
    } catch (error) {
      console.error("Error deleting gift card:", error);
    }
  };

  const AddGiftCardForm = ({ onClose }) => {
    const handleSubmit = async (e) => {
      e.preventDefault();
      const formData = new FormData(e.target);
      const giftCardData = {
        amountOriginal: parseFloat(formData.get("amountOriginal")),
        validFrom: formData.get("validFrom"),
        validUntil: formData.get("validUntil"),
      };
      try {
        await giftCardApi.createGiftCard(businessId, giftCardData);
        fetchGiftCards();
        onClose();
      } catch (error) {
        console.error("Error adding gift card:", error);
        alert(`Error adding gift card: ${error.response?.data?.message || error.message}`);
      }
    };

    return (
      <form onSubmit={handleSubmit} className="space-y-4 mb-4">
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
        <button type="submit">Add Gift Card</button>
        <button type="button" onClick={onClose}>Cancel</button>
      </form>
    );
  };

  const UpdateGiftCardForm = ({ giftCard, onClose }) => {
    const handleSubmit = async (e) => {
      e.preventDefault();
      const formData = new FormData(e.target);
      const updatedGiftCard = {
        amountLeft: parseFloat(formData.get("amountLeft")),
        validFrom: formData.get("validFrom"),
        validUntil: formData.get("validUntil"),
      };
      try {
        await giftCardApi.updateGiftCard(giftCard.giftCardId, updatedGiftCard);
        fetchGiftCards();
        onClose();
      } catch (error) {
        console.error("Error updating gift card:", error);
      }
    };

    return (
      <form onSubmit={handleSubmit} className="space-y-4 mb-4">
        <input type="hidden" name="giftCardId" value={giftCard.giftCardId} />
        <div>
          <label htmlFor="amountLeft">Remaining Amount</label>
          <input
            id="amountLeft"
            name="amountLeft"
            type="number"
            step="0.01"
            defaultValue={giftCard.amountLeft}
            required
          />
        </div>
        <div>
          <label htmlFor="validFrom">Valid From</label>
          <input id="validFrom" name="validFrom" type="date" defaultValue={giftCard.validFrom} required />
        </div>
        <div>
          <label htmlFor="validUntil">Valid Until</label>
          <input id="validUntil" name="validUntil" type="date" defaultValue={giftCard.validUntil} required />
        </div>
        <button type="submit">Update Gift Card</button>
        <button type="button" onClick={onClose}>Cancel</button>
      </form>
    );
  };

  const GiftCardList = ({ giftCards }) => {
    if (!Array.isArray(giftCards) || giftCards.length === 0) {
      return <div>No gift cards available.</div>;
    }

    return (
      <table className="min-w-full table-fixed divide-y divide-gray-200">
        <thead>
          <tr>
            <th>Code</th>
            <th>Original Amount</th>
            <th>Remaining Amount</th>
            <th>Valid From</th>
            <th>Valid Until</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {giftCards.map((giftCard) => (
            <tr key={giftCard.giftCardId}>
              <td>{giftCard.giftCardCode}</td>
              <td>${giftCard.amountOriginal.toFixed(2)}</td>
              <td>${giftCard.amountLeft.toFixed(2)}</td>
              <td>{new Date(giftCard.validFrom).toLocaleDateString()}</td>
              <td>{new Date(giftCard.validUntil).toLocaleDateString()}</td>
              <td>
                <Button onClick={() => setEditingGiftCard(giftCard)}>Edit</Button>
                <Button variant="destructive" onClick={() => handleDelete(giftCard.giftCardId)}>
                  Delete
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    );
  };

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Gift Cards</h1>
      {isAdding ? (
        <AddGiftCardForm onClose={() => setIsAdding(false)} />
      ) : editingGiftCard ? (
        <UpdateGiftCardForm giftCard={editingGiftCard} onClose={() => setEditingGiftCard(null)} />
      ) : (
        <Button onClick={() => setIsAdding(true)}>Add Gift Card</Button>
      )}
      <GiftCardList giftCards={giftCards} />
    </div>
  );
}
