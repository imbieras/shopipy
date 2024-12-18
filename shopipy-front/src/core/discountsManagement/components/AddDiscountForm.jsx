'use client'

import { useState } from 'react';
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { createDiscount } from '../services/DiscountsApi';
import { useUser } from '@/hooks/useUser';

export default function AddDiscountForm() {
  const [isOpen, setIsOpen] = useState(false);
  const { businessId } = useUser(); // Getting the businessId from the store
  
  if (!isOpen) {
    return <Button onClick={() => setIsOpen(true)}>Add Discount</Button>;
  }

  const handleSubmit = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const discountData = {
      categoryId: formData.get('categoryId'),
      name: formData.get('name'),
      description: formData.get('description'),
      discountValue: formData.get('discountValue'),
      discountType: formData.get('discountType'),
      effectiveFrom: formData.get('effectiveFrom'),
      effectiveTo: formData.get('effectiveTo') || null
    };

    await createDiscount(businessId, discountData);
    setIsOpen(false); // Close the form after submitting
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4 mb-4">
      <div>
        <Label htmlFor="categoryId">Category ID</Label>
        <Input id="categoryId" name="categoryId" type="number" required />
      </div>
      <div>
        <Label htmlFor="name">Name</Label>
        <Input id="name" name="name" maxLength={255} required />
      </div>
      <div>
        <Label htmlFor="description">Description</Label>
        <Textarea id="description" name="description" maxLength={255} required />
      </div>
      <div>
        <Label htmlFor="discountValue">Discount Value</Label>
        <Input id="discountValue" name="discountValue" type="number" step="0.01" required />
      </div>
      <div>
        <Label htmlFor="discountType">Discount Type</Label>
        <Select name="discountType" required>
          <SelectTrigger>
            <SelectValue placeholder="Select a discount type" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="Fixed">Fixed</SelectItem>
            <SelectItem value="Percentage">Percentage</SelectItem>
          </SelectContent>
        </Select>
      </div>
      <div>
        <Label htmlFor="effectiveFrom">Effective From</Label>
        <Input id="effectiveFrom" name="effectiveFrom" type="datetime-local" required />
      </div>
      <div>
        <Label htmlFor="effectiveTo">Effective To</Label>
        <Input id="effectiveTo" name="effectiveTo" type="datetime-local" />
      </div>
      <Button type="submit">Add Discount</Button>
      <Button type="button" variant="outline" onClick={() => setIsOpen(false)} className="ml-2">Cancel</Button>
    </form>
  );
}
