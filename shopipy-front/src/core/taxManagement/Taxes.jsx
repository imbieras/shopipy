// TaxManagementPage.js
'use client';

import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { useUser } from '@/hooks/useUser';
import { taxesApi } from './services/TaxesApi'; // Assuming this file handles API calls for taxes

// Utility to format the date for `datetime-local` input
const formatDateForInput = (date) => {
  if (!date) return '';
  return new Date(date).toISOString().slice(0, 16);
};

const AddTaxForm = ({ onClose }) => {
  const { businessId } = useUser();
  const queryClient = useQueryClient();
  const [formState, setFormState] = useState({
    categoryId: '',
    name: '',
    rate: '',
    effectiveFrom: '',
    effectiveTo: '',
  });

  // Create mutation for adding a new tax rate
  const createTaxRateMutation = useMutation({
    mutationFn: (taxData) => taxesApi.createTaxRate(businessId, taxData),
    onSuccess: () => {
      queryClient.invalidateQueries(['taxRates', businessId]);
      onClose();
    },
    onError: (error) => {
      alert(`Failed to add tax rate: ${error.message}`);
    },
  });

  const handleChange = (e) => {
    setFormState({ ...formState, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    const effectiveFromUtc = new Date(formState.effectiveFrom).toISOString();
    const effectiveToUtc = formState.effectiveTo ? new Date(formState.effectiveTo).toISOString() : null;

    const taxData = {
      ...formState,
      businessId,
      effectiveFrom: effectiveFromUtc,
      effectiveTo: effectiveToUtc,
    };

    createTaxRateMutation.mutate(taxData);
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
        <Label htmlFor="rate">Tax Rate</Label>
        <Input
          id="rate"
          name="rate"
          type="number"
          step="0.01"
          value={formState.rate}
          onChange={handleChange}
          required
        />
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
        <Button type="submit" isLoading={createTaxRateMutation.isLoading}>
          Add Tax Rate
        </Button>
        <Button type="button" variant="outline" onClick={onClose}>
          Cancel
        </Button>
      </div>
    </form>
  );
};

const UpdateTaxForm = ({ tax, onClose }) => {
  const { businessId } = useUser();
  const queryClient = useQueryClient();
  const [formState, setFormState] = useState(tax);

  const updateTaxRateMutation = useMutation({
    mutationFn: (updatedEffectiveTo) =>
      taxesApi.updateTaxRate(businessId, tax.taxRateId, updatedEffectiveTo),
    onSuccess: () => {
      queryClient.invalidateQueries(['taxRates', businessId]);
      onClose();
    },
    onError: (error) => {
      alert(`Failed to update tax rate: ${error.message}`);
    },
  });

  const handleChange = (e) => {
    setFormState({ ...formState, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    const effectiveToUtc = formState.effectiveTo ? new Date(formState.effectiveTo).toISOString() : null;

    updateTaxRateMutation.mutate(effectiveToUtc);
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
        <Label htmlFor="rate">Tax Rate</Label>
        <Input
          id="rate"
          name="rate"
          type="number"
          value={formState.rate}
          onChange={handleChange}
          disabled
        />
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
        <Button type="submit" isLoading={updateTaxRateMutation.isLoading}>
          Update Tax Rate
        </Button>
        <Button type="button" variant="outline" onClick={onClose}>
          Cancel
        </Button>
      </div>
    </form>
  );
};

const TaxList = ({ taxes, onEdit }) => {
    const { businessId } = useUser();
    const queryClient = useQueryClient();
  
    const deleteTaxRateMutation = useMutation({
      mutationFn: (taxId) => taxesApi.deleteTaxRate(businessId, taxId),
      onSuccess: () => {
        queryClient.invalidateQueries(['taxRates', businessId]);
      },
      onError: (error) => {
        alert(`Failed to deactivate tax rate: ${error.message}`);
      },
    });
  
    const handleDelete = (taxId) => {
      if (window.confirm('Are you sure you want to deactivate this tax rate?')) {
        deleteTaxRateMutation.mutate(taxId);
      }
    };
  
    // Filter the taxes based on effectiveTo condition
    const filteredTaxes = taxes.filter((tax) => {
      // If effectiveTo is null, consider the tax as valid
      if (!tax.effectiveTo) return true;
  
      // Parse the effectiveTo date string
      const effectiveToDate = new Date(tax.effectiveTo);
  
      // Get the current date and time
      const currentDate = new Date();
  
      // Compare effectiveTo with the current date (including time)
      return effectiveToDate > currentDate;
    });
  
    return (
      <table className="min-w-full table-fixed divide-y divide-gray-200">
        <thead className="bg-gray-50">
          <tr>
            <th className="px-4 py-2 text-left">Category</th>
            <th className="px-4 py-2 text-left">Name</th>
            <th className="px-4 py-2 text-left">Rate</th>
            <th className="px-4 py-2 text-left">Effective From</th>
            <th className="px-4 py-2 text-left">Effective To</th>
            <th className="px-4 py-2 text-right">Actions</th>
          </tr>
        </thead>
        <tbody>
          {filteredTaxes.map((tax) => (
            <tr key={tax.taxRateId}>
              <td className="px-4 py-2">{tax.categoryId}</td>
              <td className="px-4 py-2">{tax.name}</td>
              <td className="px-4 py-2">{tax.rate}</td>
              <td className="px-4 py-2">{new Date(tax.effectiveFrom).toLocaleString()}</td>
              <td className="px-4 py-2">{tax.effectiveTo ? new Date(tax.effectiveTo).toLocaleString() : 'N/A'}</td>
              <td className="px-4 py-2 text-right">
                <div className="flex justify-end gap-2">
                  <Button onClick={() => onEdit(tax)} className="min-w-[80px]">
                    Edit
                  </Button>
                  <Button
                    variant="destructive"
                    onClick={() => handleDelete(tax.taxRateId)}
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
  

  const TaxManagementPage = () => {
    const { businessId } = useUser();
    const [isAdding, setIsAdding] = useState(false);
    const [editingTax, setEditingTax] = useState(null);
  
    const { data: taxes = [], isLoading, error } = useQuery({
      queryKey: ['taxRates', businessId],
      queryFn: () => taxesApi.getTaxRates(businessId),
    });
  
    if (isLoading) return <div>Loading...</div>;
    if (error) return <div>Error loading tax rates: {error.message}</div>;
  
    return (
      <div className="space-y-6">
        <h1 className="text-3xl font-bold">Tax Rates</h1>
        {isAdding ? (
          <AddTaxForm onClose={() => setIsAdding(false)} />
        ) : editingTax ? (
          <UpdateTaxForm tax={editingTax} onClose={() => setEditingTax(null)} />
        ) : (
          <Button onClick={() => setIsAdding(true)}>Add Tax Rate</Button>
        )}
        <TaxList taxes={taxes} onEdit={setEditingTax} />
      </div>
    );
  };
  

export default TaxManagementPage;
