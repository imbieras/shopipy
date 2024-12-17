import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"

export function ProductVariationModal({
  product,
  onClose,
  onAddToOrder,
}) {
  return (
    <Dialog open={true} onOpenChange={onClose}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Choose {product.name} Variation</DialogTitle>
        </DialogHeader>
        <div className="grid grid-cols-2 gap-4 mt-4">
          {product.variations?.map((variation) => (
            <Button
              key={variation.id}
              variant="outline"
              onClick={() => onAddToOrder(variation)}
            >
              <span className="font-bold">{variation.name}</span>
              <span>${variation.price.toFixed(2)}</span>
            </Button>
          ))}
        </div>
      </DialogContent>
    </Dialog>
  )
}

