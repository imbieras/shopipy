import { giftCardApi } from "@/core/gift-cards/services/GiftCardApi"
import GiftCardList from "./GiftCardList"
import AddGiftCardForm from "./AddGiftCardForm"

export default async function GiftCardsPage() {
  const giftCards = await giftCardApi.getGiftCards()

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Gift Cards</h1>
      <AddGiftCardForm />
      <GiftCardList giftCards={giftCards} />
    </div>
  )
}

