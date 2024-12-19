'use client';

import { useState } from 'react';
import { giftCardApi } from "@/core/gift-cards/services/GiftCardApi";

export default function GiftCardsPage() {
  const [giftCards, setGiftCards] = useState([]);
  const [editingGiftCard, setEditingGiftCard] = useState(null);
  const [isAdding, setIsAdding] = useState(false);

  const fetchGiftCards = async () => {
    try {
      const data = await giftCardApi.getGiftCards();
      setGiftCards(data);
    } catch (error) {
      console.error("Error fetching gift cards:", error);
    }
  };

  const handleDelete = async (giftCardId) => {
    try {
      await giftCardApi.deleteGiftCard(giftCardId);
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
        await giftCardApi.addGiftCard(giftCardData);
        fetchGiftCards();
        onClose();
      } catch (error) {
        console.error("Error adding gift card:", error);
      }
    };

    return (
      <form onSubmit={handleSubmit} className="space-y-4 mb-4">
        <div>
          <label htmlFor="amountOriginal">Original Amount</label>
          <input id="amountOriginal" name="amountOriginal" type="number" step="0.01" required />
        </div>
        <div>
          <label htmlFor="validFrom">Valid From</label>
          <input id="validFrom" name="validFrom" type="date" required />
        </div>
        <div>
          <label htmlFor="validUntil">Valid Until</label>
          <input id="validUntil" name="validUntil" type="date" required />
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
    return (
      <table>
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
                <button onClick={() => setEditingGiftCard(giftCard)}>Edit</button>
                <button onClick={() => handleDelete(giftCard.giftCardId)}>Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    );
  };

  return (
    <div>
      <h1>Gift Cards</h1>
      {isAdding ? (
        <AddGiftCardForm onClose={() => setIsAdding(false)} />
      ) : editingGiftCard ? (
        <UpdateGiftCardForm giftCard={editingGiftCard} onClose={() => setEditingGiftCard(null)} />
      ) : (
        <button onClick={() => setIsAdding(true)}>Add Gift Card</button>
      )}
      <GiftCardList giftCards={giftCards} />
    </div>
  );
}
