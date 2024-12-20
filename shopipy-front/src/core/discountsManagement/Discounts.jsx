'use client';

import {useState} from 'react';
import {Button} from "@/components/ui/button";
import {Input} from "@/components/ui/input";
import {Label} from "@/components/ui/label";
import {Textarea} from "@/components/ui/textarea";
import {Select, SelectContent, SelectItem, SelectTrigger, SelectValue} from "@/components/ui/select";
import {Table, TableBody, TableCell, TableHead, TableHeader, TableRow} from "@/components/ui/table";
import {discountsApi} from './services/DiscountsApi';
import {useBusiness} from "@/hooks/useUser";
import {useMutation, useQuery, useQueryClient} from '@tanstack/react-query';

// Utility function to format date for input
const formatDateForInput = (date) => {
  if (!date) return '';  // Return an empty string if no date is provided
  return new Date(date).toISOString();  // Convert the date to the correct ISO format with timezone
};

const DiscountPage = () => {
  const {businessId} = useBusiness();
  const queryClient = useQueryClient();
  const [isAdding, setIsAdding] = useState(false);
  const [editingDiscount, setEditingDiscount] = useState(null);

  // Fetch Discounts
  const {data: discounts = [], isLoading, error} = useQuery({
    queryKey: ['discounts', businessId],
    queryFn: () => discountsApi.getDiscounts(businessId),
  });

  // Mutations
  const createDiscountMutation = useMutation({
    mutationFn: (discountData) => discountsApi.createDiscount(businessId, discountData),
    onSuccess: () => {
      queryClient.invalidateQueries(['discounts', businessId]);
      setIsAdding(false);
    },
    onError: (error) => {
      alert(`Failed to create discount: ${error.message}`);
    },
  });

  const updateDiscountMutation = useMutation({
    mutationFn: (updatedDiscountData) =>
      discountsApi.updateDiscount(businessId, editingDiscount.discountId, updatedDiscountData.effectiveTo),
    onSuccess: () => {
      queryClient.invalidateQueries(['discounts', businessId]);
      setEditingDiscount(null);
    },
    onError: (error) => {
      alert(`Failed to update discount: ${error.message}`);
    },
  });

  const deleteDiscountMutation = useMutation({
    mutationFn: (discountId) => discountsApi.deleteDiscount(businessId, discountId),
    onSuccess: () => queryClient.invalidateQueries(['discounts', businessId]),
  });

  // Handle form submission for both adding and editing
  const handleDiscountSubmit = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);

    // Prepare the data, but only send the effectiveTo for updates
    if (isAdding) {
      const data = {
        categoryId: formData.get('categoryId'),
        name: formData.get('name'),
        description: formData.get('description'),
        discountValue: formData.get('discountValue'),
        discountType: formData.get('discountType'),
        effectiveFrom: formatDateForInput(formData.get('effectiveFrom')),
        effectiveTo: formatDateForInput(formData.get('effectiveTo')) || null,
      };
      await createDiscountMutation.mutateAsync(data);
    } else if (editingDiscount) {
      const data = {
        effectiveTo: formatDateForInput(formData.get('effectiveTo')) || null,
      };
      await updateDiscountMutation.mutateAsync(data);
    }
  };

  const handleDeleteDiscount = (discountId) => {
    if (window.confirm('Are you sure you want to delete this discount?')) {
      deleteDiscountMutation.mutate(discountId);
    }
  };

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error loading discounts</div>;

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Discounts</h1>

      {isAdding || editingDiscount ? (
        // Form for adding a new discount
        <form onSubmit={handleDiscountSubmit} className="space-y-4 mb-4">
          {isAdding && (
            <>
              <Label htmlFor="categoryId">Category ID</Label>
              <Input id="categoryId" name="categoryId" type="number" required/>

              <Label htmlFor="name">Name</Label>
              <Input id="name" name="name" required/>

              <Label htmlFor="description">Description</Label>
              <Textarea id="description" name="description" required/>

              <Label htmlFor="discountValue">Discount Value</Label>
              <Input id="discountValue" name="discountValue" type="number" step="0.01" required/>

              <Label htmlFor="discountType">Discount Type</Label>
              <Select name="discountType" required>
                <SelectTrigger><SelectValue placeholder="Select a discount type"/></SelectTrigger>
                <SelectContent>
                  <SelectItem value="Fixed">Fixed</SelectItem>
                  <SelectItem value="Percentage">Percentage</SelectItem>
                </SelectContent>
              </Select>

              <Label htmlFor="effectiveFrom">Effective From</Label>
              <Input id="effectiveFrom" name="effectiveFrom" type="datetime-local" required/>
            </>
          )}

          <Label htmlFor="effectiveTo">Effective To</Label>
          <Input
            id="effectiveTo"
            name="effectiveTo"
            type="datetime-local"
            defaultValue={formatDateForInput(editingDiscount?.effectiveTo) || ''}
          />

          <Button type="submit">{isAdding ? 'Add Discount' : 'Update Discount'}</Button>
          <Button type="button" variant="outline" onClick={() => {
            setIsAdding(false);
            setEditingDiscount(null);
          }}>Cancel</Button>
        </form>
      ) : (
        <Button onClick={() => setIsAdding(true)}>Add Discount</Button>
      )}

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>ID</TableHead>
            <TableHead>Category ID</TableHead>
            <TableHead>Name</TableHead>
            <TableHead>Description</TableHead>
            <TableHead>Value</TableHead>
            <TableHead>Type</TableHead>
            <TableHead>Effective From</TableHead>
            <TableHead>Effective To</TableHead>
            <TableHead>Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {discounts.map((discount) => (
            <TableRow key={discount.discountId}>
              <TableCell>{discount.discountId}</TableCell>
              <TableCell>{discount.categoryId}</TableCell>
              <TableCell>{discount.name}</TableCell>
              <TableCell>{discount.description}</TableCell>
              <TableCell>{discount.discountValue}</TableCell>
              <TableCell>{discount.discountType}</TableCell>
              <TableCell>{new Date(discount.effectiveFrom).toLocaleDateString()}</TableCell>
              <TableCell>{discount.effectiveTo ? new Date(discount.effectiveTo).toLocaleDateString() : 'N/A'}</TableCell>
              <TableCell>
                <Button onClick={() => setEditingDiscount(discount)} className="mr-2">Edit</Button>
                <Button onClick={() => handleDeleteDiscount(discount.discountId)} variant="destructive">Delete</Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
};

export default DiscountPage;
