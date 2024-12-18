'use client'

import { useState } from 'react';
import { Button } from "@/components/ui/button";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { deleteDiscount } from '../services/DiscountsApi';
import { useUser } from '@/hooks/useUser';
import UpdateDiscountForm from './EditDiscountForm';

export default function DiscountList({ discounts }) {
  const [editingDiscount, setEditingDiscount] = useState(null);
  const { businessId } = useUser(); // Getting the businessId from the store

  const handleDelete = async (discountId) => {
    await deleteDiscount(businessId, discountId); // Passing the businessId for deletion
  };

  return (
    <div>
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
        {discounts && discounts.length > 0 ? (
          discounts.map((discount) => (
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
                <Button onClick={() => handleDelete(discount.discountId)} variant="destructive">Delete</Button>
              </TableCell>
            </TableRow>
          ))
        ) : (
          <TableRow>
            <TableCell colSpan="9">No discounts available</TableCell>
          </TableRow>
        )}
      </TableBody>

      </Table>
      {editingDiscount && (
        <UpdateDiscountForm discount={editingDiscount} onClose={() => setEditingDiscount(null)} />
      )}
    </div>
  );
}
