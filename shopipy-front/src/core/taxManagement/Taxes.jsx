'use client';

import { useState } from 'react';
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { taxesApi } from './services/TaxesApi';
import { useBusiness } from "@/hooks/useUser";
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

// Utility function to format date for input
const formatDateForInput = (date) => {
  if (!date) return '';  // Return an empty string if no date is provided
  return new Date(date).toISOString();  // Convert the date to the correct ISO format with timezone
};

const TaxRatesPage = () => {
  const { businessId } = useBusiness();
  const queryClient = useQueryClient();
  const [isAdding, setIsAdding] = useState(false);
  const [editingTaxRate, setEditingTaxRate] = useState(null);

  // Fetch tax rates
  const { data: taxRates = [], isLoading, error } = useQuery({
    queryKey: ['taxRates', businessId],
    queryFn: () => taxesApi.getTaxRates(businessId),
  });

  // Mutations
  const createTaxRateMutation = useMutation({
    mutationFn: (taxRateData) => taxesApi.createTaxRate(businessId, taxRateData),
    onSuccess: () => {
      queryClient.invalidateQueries(['taxRates', businessId]);
      setIsAdding(false);
    },
    onError: (error) => {
      alert(`Failed to create tax rate: ${error.message}`);
    },
  });

  const updateTaxRateMutation = useMutation({
    mutationFn: ({ taxRateId, effectiveTo }) =>
      taxesApi.updateTaxRate(businessId, taxRateId, effectiveTo),
    onSuccess: () => {
      queryClient.invalidateQueries(['taxRates', businessId]);
      setEditingTaxRate(null);
    },
    onError: (error) => {
      alert(`Failed to update tax rate: ${error.message}`);
    },
  });

  const deleteTaxRateMutation = useMutation({
    mutationFn: (taxRateId) => taxesApi.deleteTaxRate(businessId, taxRateId),
    onSuccess: () => queryClient.invalidateQueries(['taxRates', businessId]),
  });

  // Handle Form Submission
  const handleTaxRateSubmit = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);

    if (isAdding) {
      const newTaxRate = {
        categoryId: formData.get('categoryId'),
        name: formData.get('name'),
        rate: parseFloat(formData.get('rate')) / 100,
        effectiveFrom: formatDateForInput(formData.get('effectiveFrom')),
        effectiveTo: formatDateForInput(formData.get('effectiveTo')) || null,
      };
      await createTaxRateMutation.mutateAsync(newTaxRate);
    } else if (editingTaxRate) {
      const updatedData = {
        taxRateId: editingTaxRate.taxRateId,
        effectiveTo: formatDateForInput(formData.get('effectiveTo')) || null,
      };
      await updateTaxRateMutation.mutateAsync(updatedData);
    }
  };

  const handleDeleteTaxRate = (taxRateId) => {
    if (window.confirm('Are you sure you want to delete this tax rate?')) {
      deleteTaxRateMutation.mutate(taxRateId);
    }
  };

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error loading tax rates</div>;

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Tax Rates</h1>

      {isAdding || editingTaxRate ? (
        <form onSubmit={handleTaxRateSubmit} className="space-y-4 mb-4">
          {isAdding && (
            <>
              <div>
                <Label htmlFor="categoryId">Category ID</Label>
                <Input id="categoryId" name="categoryId" type="number" required/>
              </div>

              <div>
                <Label htmlFor="name">Name</Label>
                <Input id="name" name="name" required/>
              </div>

              <div>
                <Label htmlFor="rate">Rate (Multiplier)</Label>
                <Input id="rate" name="rate" type="number" step="0.01" required/>
              </div>

              <div>
                <Label htmlFor="effectiveFrom">Effective From</Label>
                <Input id="effectiveFrom" name="effectiveFrom" type="datetime-local" required/>
              </div>
            </>
          )}

          <Label htmlFor="effectiveTo">Effective To</Label>
          <Input
            id="effectiveTo"
            name="effectiveTo"
            type="datetime-local"
            defaultValue={formatDateForInput(editingTaxRate?.effectiveTo) || ''}
          />

          <Button type="submit">{isAdding ? 'Add Tax Rate' : 'Update Tax Rate'}</Button>
          <Button type="button" variant="outline" onClick={() => { setIsAdding(false); setEditingTaxRate(null); }} className="ml-2">Cancel</Button>
        </form>
      ) : (
        <Button onClick={() => setIsAdding(true)}>Add Tax Rate</Button>
      )}

      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>ID</TableHead>
            <TableHead>Category ID</TableHead>
            <TableHead>Name</TableHead>
            <TableHead>Rate</TableHead>
            <TableHead>Effective From</TableHead>
            <TableHead>Effective To</TableHead>
            <TableHead>Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {taxRates.map((taxRate) => (
            <TableRow key={taxRate.taxRateId}>
              <TableCell>{taxRate.taxRateId}</TableCell>
              <TableCell>{taxRate.categoryId}</TableCell>
              <TableCell>{taxRate.name}</TableCell>
              <TableCell>{(taxRate.rate * 100).toFixed(2)}%</TableCell>
              <TableCell>{new Date(taxRate.effectiveFrom).toLocaleDateString()}</TableCell>
              <TableCell>{taxRate.effectiveTo ? new Date(taxRate.effectiveTo).toLocaleDateString() : 'N/A'}</TableCell>
              <TableCell>
                <Button onClick={() => setEditingTaxRate(taxRate)} className="mr-2">Edit</Button>
                <Button onClick={() => handleDeleteTaxRate(taxRate.taxRateId)} variant="destructive">Delete</Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
};

export default TaxRatesPage;
