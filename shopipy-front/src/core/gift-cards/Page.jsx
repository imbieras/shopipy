import { useState } from "react";
import { giftCardApi } from "@/core/gift-cards/services/GiftCardApi";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useBusiness } from "@/hooks/useUser";

export default function GiftCardsPage() {
  const { businessId } = useBusiness();
  const [giftCards, setGiftCards] = useState([]);
  const [editingGiftCard, setEditingGiftCard] = useState(null);
  const [isAdding, setIsAdding] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const fetchGiftCards = async () => {
    if (!businessId) return;
    setIsLoading(true);
    try {
      const response = await giftCardApi.getGiftCards(businessId);
      setGiftCards(Array.isArray(response.data) ? response.data : []);
    } catch (error) {
      console.error("Error fetching gift cards:", error);
    } finally {
      setIsLoading(false);
    }
  };

  if (businessId && giftCards.length === 0 && !isLoading) {
    fetchGiftCards();
  }

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
      }
    };

    return (
      <form onSubmit={handleSubmit} className="space-y-4 mb-6 bg-white p-6 rounded-md shadow-md">
        <h2 className="text-lg font-semibold">Add Gift Card</h2>
        <div>
          <Label htmlFor="amountOriginal">Original Amount</Label>
          <Input id="amountOriginal" name="amountOriginal" type="number" step="0.01" required />
        </div>
        <div className="grid grid-cols-2 gap-4">
          <div>
            <Label htmlFor="validFrom">Valid From</Label>
            <Input id="validFrom" name="validFrom" type="date" required />
          </div>
          <div>
            <Label htmlFor="validUntil">Valid Until</Label>
            <Input id="validUntil" name="validUntil" type="date" required />
          </div>
        </div>
        <div className="flex justify-end space-x-4">
          <Button type="submit">Add Gift Card</Button>
          <Button type="button" variant="destructive" onClick={onClose}>
            Cancel
          </Button>
        </div>
      </form>
    );
  };

  const UpdateGiftCardForm = ({ giftCard, onClose }) => {
    const handleSubmit = async (e) => {
      e.preventDefault();
      const formData = new FormData(e.target);
      const updatedGiftCard = {
        amountOriginal: parseFloat(formData.get("amountOriginal")),
        validFrom: formData.get("validFrom"),
        validUntil: formData.get("validUntil"),
      };
      try {
        await giftCardApi.updateGiftCard(businessId, giftCard.giftCardId, updatedGiftCard);
        fetchGiftCards();
        onClose();
      } catch (error) {
        console.error("Error updating gift card:", error);
      }
    };

    return (
      <form onSubmit={handleSubmit} className="space-y-4 mb-6 bg-white p-6 rounded-md shadow-md">
        <h2 className="text-lg font-semibold">Update Gift Card</h2>
        <div>
          <Label htmlFor="amountOriginal">Original Amount</Label>
          <Input
            id="amountOriginal"
            name="amountOriginal"
            type="number"
            step="0.01"
            defaultValue={giftCard.amountOriginal}
            required
          />
        </div>
        <div className="grid grid-cols-2 gap-4">
          <div>
            <Label htmlFor="validFrom">Valid From</Label>
            <Input id="validFrom" name="validFrom" type="date" defaultValue={giftCard.validFrom} required />
          </div>
          <div>
            <Label htmlFor="validUntil">Valid Until</Label>
            <Input id="validUntil" name="validUntil" type="date" defaultValue={giftCard.validUntil} required />
          </div>
        </div>
        <div className="flex justify-end space-x-4">
          <Button type="submit">Update Gift Card</Button>
          <Button type="button" variant="outline" onClick={onClose}>
            Cancel
          </Button>
        </div>
      </form>
    );
  };

  const GiftCardList = ({ giftCards }) => {
    if (!Array.isArray(giftCards) || giftCards.length === 0) {
      return <div className="text-center mt-4">No gift cards available.</div>;
    }

    return (
      <table className="min-w-full border border-gray-200 rounded-md shadow-sm mt-4">
        <thead className="bg-gray-100">
          <tr>
            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">Code</th>
            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">Original Amount</th>
            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">Remaining Amount</th>
            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">Valid From</th>
            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">Valid Until</th>
            <th className="px-4 py-2 text-right text-sm font-semibold text-gray-600">Actions</th>
          </tr>
        </thead>
        <tbody>
          {giftCards.map((giftCard) => (
            <tr key={giftCard.giftCardId} className="border-t hover:bg-gray-50">
              <td className="px-4 py-2">{giftCard.giftCardCode}</td>
              <td className="px-4 py-2">${giftCard.amountOriginal.toFixed(2)}</td>
              <td className="px-4 py-2">${giftCard.amountLeft.toFixed(2)}</td>
              <td className="px-4 py-2">{new Date(giftCard.validFrom).toLocaleDateString()}</td>
              <td className="px-4 py-2">{new Date(giftCard.validUntil).toLocaleDateString()}</td>
              <td className="px-4 py-2 text-right">
                <div className="inline-flex space-x-2">
                  <Button onClick={() => setEditingGiftCard(giftCard)}>Edit</Button>
                  <Button variant="destructive" onClick={() => handleDelete(giftCard.giftCardId)}>
                    Delete
                  </Button>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    );
  };

  return (
    <div className="max-w-6xl mx-auto space-y-8">
      <h1 className="text-3xl font-bold text-left">Gift Cards</h1>
      {isAdding ? (
        <AddGiftCardForm onClose={() => setIsAdding(false)} />
      ) : editingGiftCard ? (
        <UpdateGiftCardForm giftCard={editingGiftCard} onClose={() => setEditingGiftCard(null)} />
      ) : (
        <div className="text-left">
          <Button onClick={() => setIsAdding(true)}>Add Gift Card</Button>
        </div>
      )}
      <GiftCardList giftCards={giftCards} />
    </div>
  );
}
