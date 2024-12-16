import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"

export default function CustomerForm({ customerInfo, setCustomerInfo }) {
  return (
    <div className="space-y-4">
      <h3 className="text-lg font-semibold">Customer Information</h3>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="customer-name">Name</Label>
          <Input
            id="customer-name"
            value={customerInfo.name}
            onChange={(e) => setCustomerInfo({ ...customerInfo, name: e.target.value })}
            placeholder="Enter your name"
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="customer-email">Email</Label>
          <Input
            id="customer-email"
            type="email"
            value={customerInfo.email}
            onChange={(e) => setCustomerInfo({ ...customerInfo, email: e.target.value })}
            placeholder="Enter your email"
          />
        </div>
        <div className="space-y-2 md:col-span-2">
          <Label htmlFor="customer-phone">Phone</Label>
          <Input
            id="customer-phone"
            type="tel"
            value={customerInfo.phone}
            onChange={(e) => setCustomerInfo({ ...customerInfo, phone: e.target.value })}
            placeholder="Enter your phone number"
          />
        </div>
      </div>
    </div>
  )
}

