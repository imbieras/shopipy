'use client';

import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { discountsApi } from './services/DiscountsApi';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { useUser } from '@/hooks/useUser';

// Utility to format the date for `datetime-local` input
const formatDateForInput = (date) => {
  if (!date) return '';
  return new Date(date).toISOString().slice(0, 16);
};

const AddDiscountForm = ({ onClose }) => {
  const { businessId } = useUser(); // Assuming you are using a hook to get the businessId
  const queryClient = useQueryClient();
  const [formState, setFormState] = useState({
    categoryId: '', // Add this field
    name: '',
    description: '',
    discountValue: '',
    discountType: '',
    effectiveFrom: '',
    effectiveTo: '',
  });

  // Create mutation for adding a new discount
  const createDiscountMutation = useMutation({
    mutationFn: (discountData) => discountsApi.createDiscount(businessId, discountData), // Include businessId here
    onSuccess: () => {
      queryClient.invalidateQueries(['discounts', businessId]);
      onClose(); // Close the form after a successful submission
    },
    onError: (error) => {
      alert(`Failed to add discount: ${error.message}`);
    },
  });

  const handleChange = (e) => {
    setFormState({ ...formState, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
  
    // Convert effectiveFrom and effectiveTo to UTC if they are not null
    const effectiveFromUtc = new Date(formState.effectiveFrom).toISOString(); // Convert to UTC
    const effectiveToUtc = formState.effectiveTo ? new Date(formState.effectiveTo).toISOString() : null;
  
    const discountData = {
      ...formState,
      businessId,
      effectiveFrom: effectiveFromUtc,
      effectiveTo: effectiveToUtc, // Ensure effectiveTo is null if empty
    };
  
    createDiscountMutation.mutate(discountData); // Pass the data with businessId
  };
  
  return (
    <form onSubmit={handleSubmit} className="space-y-4 mb-4">
      <div>
        <Label htmlFor="categoryId">Category ID</Label>
        <Input
          id="categoryId"
          name="categoryId"
          type="number"
          value={formState.categoryId}
          onChange={handleChange}
          required
        />
      </div>
      <div>
        <Label htmlFor="name">Name</Label>
        <Input
          id="name"
          name="name"
          value={formState.name}
          onChange={handleChange}
          maxLength={255}
          required
        />
      </div>
      <div>
        <Label htmlFor="description">Description</Label>
        <Textarea
          id="description"
          name="description"
          value={formState.description}
          onChange={handleChange}
          maxLength={255}
          required
        />
      </div>
      <div>
        <Label htmlFor="discountValue">Discount Value</Label>
        <Input
          id="discountValue"
          name="discountValue"
          type="number"
          step="0.01"
          value={formState.discountValue}
          onChange={handleChange}
          required
        />
      </div>
      <div>
        <Label htmlFor="discountType">Discount Type</Label>
        <Select
          name="discountType"
          value={formState.discountType}
          onValueChange={(value) => setFormState({ ...formState, discountType: value })}
          required
        >
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
        <Input
          id="effectiveFrom"
          name="effectiveFrom"
          type="datetime-local"
          value={formatDateForInput(formState.effectiveFrom)}
          onChange={handleChange}
          required
        />
      </div>
      <div>
        <Label htmlFor="effectiveTo">Effective To</Label>
        <Input
          id="effectiveTo"
          name="effectiveTo"
          type="datetime-local"
          value={formatDateForInput(formState.effectiveTo)}
          onChange={handleChange}
        />
      </div>
      <div className="flex gap-2">
        <Button type="submit" isLoading={createDiscountMutation.isLoading}>
          Add Discount
        </Button>
        <Button type="button" variant="outline" onClick={onClose}>
          Cancel
        </Button>
      </div>
    </form>
  );
};


const UpdateDiscountForm = ({ discount, onClose }) => {
  const { businessId } = useUser();
  const queryClient = useQueryClient();
  const [formState, setFormState] = useState(discount);

  const updateDiscountMutation = useMutation({
    mutationFn: (updatedEffectiveTo) =>
      discountsApi.updateDiscount(businessId, discount.discountId, updatedEffectiveTo), // Send only effectiveTo
    onSuccess: () => {
      queryClient.invalidateQueries(['discounts', businessId]);
      onClose();
    },
    onError: (error) => {
      alert(`Failed to update discount: ${error.message}`);
    },
  });

  const handleChange = (e) => {
    setFormState({ ...formState, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
  
    // Convert effectiveTo to UTC if it's provided
    const effectiveToUtc = formState.effectiveTo ? new Date(formState.effectiveTo).toISOString() : null;
  
    // Directly pass only the effectiveTo field (not wrapped in an object)
    updateDiscountMutation.mutate(effectiveToUtc); // Send only effectiveTo as a plain value
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4 mb-4">
      <div>
        <Label htmlFor="categoryId">Category ID</Label>
        <Input
          id="categoryId"
          name="categoryId"
          type="number"
          value={formState.categoryId}
          onChange={handleChange}
          disabled
        />
      </div>
      <div>
        <Label htmlFor="name">Name</Label>
        <Input
          id="name"
          name="name"
          value={formState.name}
          onChange={handleChange}
          maxLength={255}
          disabled
        />
      </div>
      <div>
        <Label htmlFor="description">Description</Label>
        <Textarea
          id="description"
          name="description"
          value={formState.description}
          onChange={handleChange}
          maxLength={255}
          disabled
        />
      </div>
      <div>
        <Label htmlFor="discountValue">Discount Value</Label>
        <Input
          id="discountValue"
          name="discountValue"
          type="number"
          step="0.01"
          value={formState.discountValue}
          onChange={handleChange}
          disabled
        />
      </div>
      <div>
        <Label htmlFor="discountType">Discount Type</Label>
        <Select
          name="discountType"
          value={formState.discountType}
          onValueChange={(value) => setFormState({ ...formState, discountType: value })}
          disabled
        >
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
        <Input
          id="effectiveFrom"
          name="effectiveFrom"
          type="datetime-local"
          value={formatDateForInput(formState.effectiveFrom)}
          onChange={handleChange}
          disabled
        />
      </div>
      <div>
        <Label htmlFor="effectiveTo">Effective To</Label>
        <Input
          id="effectiveTo"
          name="effectiveTo"
          type="datetime-local"
          value={formatDateForInput(formState.effectiveTo)}
          onChange={handleChange}
          required
        />
      </div>
      <div className="flex gap-2">
        <Button type="submit" isLoading={updateDiscountMutation.isLoading}>
          Update Discount
        </Button>
        <Button type="button" variant="outline" onClick={onClose}>
          Cancel
        </Button>
      </div>
    </form>
  );
};



const DiscountList = ({ discounts, onEdit }) => {
  const { businessId } = useUser();
  const queryClient = useQueryClient();

  const deleteDiscountMutation = useMutation({
    mutationFn: (discountId) => discountsApi.deleteDiscount(businessId, discountId),
    onSuccess: () => {
      queryClient.invalidateQueries(['discounts', businessId]);
    },
    onError: (error) => {
      alert(`Failed to deactivate discount: ${error.message}`);
    },
  });

  const handleDelete = (discountId) => {
    if (window.confirm('Are you sure you want to deactivate this discount?')) {
      deleteDiscountMutation.mutate(discountId);
    }
  };

  // Filter the discounts based on effectiveTo condition
  const filteredDiscounts = discounts.filter((discount) => {
    // If effectiveTo is null, consider the discount as valid
    if (!discount.effectiveTo) return true;
  
    // Parse the effectiveTo date string
    const effectiveToDate = new Date(discount.effectiveTo);
  
    // Get the current date and time
    const currentDate = new Date();
  
    // Compare effectiveTo with the current date (including time)
    return effectiveToDate > currentDate;
  });

  return (
    <table className="min-w-full table-fixed divide-y divide-gray-200">
      <thead className="bg-gray-50">
        <tr>
          <th className="px-4 py-2 text-left">Name</th>
          <th className="px-4 py-2 text-left">Description</th>
          <th className="px-4 py-2 text-left">Value</th>
          <th className="px-4 py-2 text-left">Type</th>
          <th className="px-4 py-2 text-right">Actions</th>
        </tr>
      </thead>
      <tbody>
        {filteredDiscounts.map((discount) => (
          <tr key={discount.discountId}>
            <td className="px-4 py-2">{discount.name}</td>
            <td className="px-4 py-2">{discount.description}</td>
            <td className="px-4 py-2">{discount.discountValue}</td>
            <td className="px-4 py-2">{discount.discountType}</td>
            <td className="px-4 py-2 text-right">
              <div className="flex justify-end gap-2">
                <Button onClick={() => onEdit(discount)} className="min-w-[80px]">
                  Edit
                </Button>
                <Button
                  variant="destructive"
                  onClick={() => handleDelete(discount.discountId)}
                  className="min-w-[80px]"
                >
                  Deactivate
                </Button>
              </div>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
};


const DiscountsPage = () => {
  const { businessId } = useUser();
  const [isAdding, setIsAdding] = useState(false);
  const [editingDiscount, setEditingDiscount] = useState(null);

  const { data: discounts = [], isLoading, error } = useQuery({
    queryKey: ['discounts', businessId],
    queryFn: () => discountsApi.getDiscounts(businessId),
  });

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error loading discounts: {error.message}</div>;

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Discounts</h1>
      {isAdding ? (
        <AddDiscountForm onClose={() => setIsAdding(false)} />
      ) : editingDiscount ? (
        <UpdateDiscountForm discount={editingDiscount} onClose={() => setEditingDiscount(null)} />
      ) : (
        <Button onClick={() => setIsAdding(true)}>Add Discount</Button>
      )}
      <DiscountList discounts={discounts} onEdit={setEditingDiscount} />
    </div>
  );
};

export default DiscountsPage;
