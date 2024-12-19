'use client';

import { useState, useEffect } from 'react';
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { productsApi } from './services/ProductsAdminApi.jsx';
import { useUser } from '@/hooks/useUser';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

const ProductsPage = () => {
  const { businessId } = useUser();
  const queryClient = useQueryClient();
  const [isAdding, setIsAdding] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);
  const [showingVariations, setShowingVariations] = useState(null);
  const [isAddingVariation, setIsAddingVariation] = useState(false);
  const [editingVariation, setEditingVariation] = useState(null);

  // Fetch products
  const { data: products = [], isLoading, error } = useQuery({
    queryKey: ['products', businessId],
    queryFn: () => productsApi.getProducts(businessId),
  });

  const { data: variations, refetch: refetchVariations } = useQuery({
    queryKey: ['variations', businessId, showingVariations],
    queryFn: () => {
      if (!showingVariations || !businessId) return []; // Return empty array if not ready
      return productsApi.getProductVariations(businessId, showingVariations);  // Make sure both are passed
    },
    enabled: !!showingVariations && !!businessId,  // Ensure both values are valid
  });

  // Mutations
  const createProductMutation = useMutation({
    mutationFn: (productData) => productsApi.createProduct(businessId, productData),
    onSuccess: () => {
      queryClient.invalidateQueries(['products', businessId]);
      setIsAdding(false);
    },
    onError: (error) => {
      alert(`Failed to create product: ${error.message}`);
    },
  });

  const updateProductMutation = useMutation({
    mutationFn: (updatedProductData) => productsApi.updateProduct(businessId, editingProduct.productId, updatedProductData),
    onSuccess: () => {
      queryClient.invalidateQueries(['products', businessId]);
      setEditingProduct(null);
    },
    onError: (error) => {
      alert(`Failed to update product: ${error.message}`);
    },
  });

  const deleteProductMutation = useMutation({
    mutationFn: (productId) => productsApi.deleteProduct(businessId, productId),
    onSuccess: () => {
      queryClient.invalidateQueries(['products', businessId]);
    },
  });

  const addProductVariationMutation = useMutation({
    mutationFn: (variationData) => productsApi.createProductVariation(businessId, showingVariations, variationData),
    onSuccess: () => {
      refetchVariations();
      setIsAddingVariation(false);
    },
  });

  const updateProductVariationMutation = useMutation({
    mutationFn: ({ variationId, productId, variationData }) => {
      return productsApi.updateProductVariation(businessId, productId, variationId, variationData);
    },
    onSuccess: () => {
      refetchVariations();
    },
  });

  const deleteProductVariationMutation = useMutation({
    mutationFn: (variationId) => productsApi.deleteProductVariation(businessId, showingVariations, variationId),
    onSuccess: () => {
      refetchVariations();
    },
  });

  const handleProductSubmit = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const productState = formData.get('productState'); // Get the productState value
  
    // Check if productState is selected
    if (!productState || productState === 'Select a state') {
        alert('Please select the product state!');
        return; // Prevent form submission if state is not selected
      }
  
    const data = {
      categoryId: formData.get('categoryId'),
      name: formData.get('name'),
      description: formData.get('description'),
      basePrice: formData.get('price'), // Changed from 'price' to 'basePrice'
      productState: productState, // Add selected productState here
    };
  
    if (editingProduct) {
      await updateProductMutation.mutateAsync(data);
    } else {
      await createProductMutation.mutateAsync(data);
    }
  };

  const handleVariationSubmit = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const variationData = {
      name: formData.get('name'),
      priceModifier: formData.get('priceModifier'),
      productState: formData.get('productState'),
    };
  
    // Ensure `showingVariations` is valid (productId)
    if (!showingVariations) {
      alert('No product selected for this variation!');
      return;
    }
  
    // If editing an existing variation, include the variationId
    if (editingVariation) {
        await updateProductVariationMutation.mutateAsync({
            variationId: editingVariation.variationId, // The variation ID to update
            productId: showingVariations, // The product ID associated with the variation
            variationData: {
              ...variationData,
            },
          });
    } else {
      console.log('Adding new variation for productId:', showingVariations);
      await addProductVariationMutation.mutateAsync({
        ...variationData,
        productId: showingVariations, // Include the productId of the associated product
      });
    }
  };

  const handleDeleteProduct = (productId) => {
    if (window.confirm('Are you sure you want to delete this product?')) {
      deleteProductMutation.mutate(productId);
    }
  };

  const handleDeleteVariation = (variationId) => {
    if (window.confirm('Are you sure you want to delete this variation?')) {
      deleteProductVariationMutation.mutate(variationId);
    }
  };

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error loading products</div>;

  return (
    <div>
      <h1>Products</h1>
      {isAdding ? (
        <form onSubmit={handleProductSubmit} className="space-y-4">
          <div>
            <Label htmlFor="categoryId">Category ID</Label>
            <Input id="categoryId" name="categoryId" type="number" required />
          </div>
          <div>
            <Label htmlFor="name">Product Name</Label>
            <Input id="name" name="name" required />
          </div>
          <div>
            <Label htmlFor="price">Price</Label>
            <Input id="price" name="price" type="number" required />
          </div>
          <div>
            <Label htmlFor="description">Description</Label>
            <Textarea id="description" name="description" required />
          </div>
          <div>
        <Label htmlFor="productState">Product State</Label>
        <Select name="productState" required>
            <SelectTrigger>
            <SelectValue placeholder="Select a state" />
            </SelectTrigger>
            <SelectContent>
            <SelectItem value="Available">Available</SelectItem>
            <SelectItem value="Unavailable">Unavailable</SelectItem>
            </SelectContent>
        </Select>
        </div>
          <Button type="submit" isLoading={createProductMutation.isLoading}>Add Product</Button>
          <Button type="button" variant="outline" onClick={() => setIsAdding(false)}>Cancel</Button>
        </form>
      ) : editingProduct ? (
        <form onSubmit={handleProductSubmit} className="space-y-4">
          <input type="hidden" name="productId" value={editingProduct.productId} />
          <div>
            <Label htmlFor="categoryId">Category ID</Label>
            <Input id="categoryId" name="categoryId" type="number" defaultValue={editingProduct.categoryId} required />
          </div>
          <div>
            <Label htmlFor="name">Product Name</Label>
            <Input id="name" name="name" defaultValue={editingProduct.name} required />
          </div>
          <div>
            <Label htmlFor="price">Price</Label>
            <Input id="price" name="price" type="number" defaultValue={editingProduct.basePrice} required />
          </div>
          <div>
            <Label htmlFor="description">Description</Label>
            <Textarea id="description" name="description" defaultValue={editingProduct.description} required />
          </div>
          <div>
            <Label htmlFor="productState">Product State</Label>
            <Select name="productState" required>
            <SelectTrigger>
                <SelectValue placeholder="Select a state" />
            </SelectTrigger>
            <SelectContent>
                <SelectItem value="Available">Available</SelectItem>
                <SelectItem value="Unavailable">Unavailable</SelectItem>
            </SelectContent>
            </Select>
            </div>
          <Button type="submit" isLoading={updateProductMutation.isLoading}>Update Product</Button>
          <Button type="button" variant="outline" onClick={() => setEditingProduct(null)}>Cancel</Button>
        </form>
      ) : (
        <Button onClick={() => setIsAdding(true)}>Add Product</Button>
      )}

<Table>
  <TableHeader>
    <TableRow>
      <TableHead>ID</TableHead>
      <TableHead>Category</TableHead>
      <TableHead>Name</TableHead>
      <TableHead>Price</TableHead>
      <TableHead>State</TableHead> {/* Added this line for the product state */}
      <TableHead>Actions</TableHead>
    </TableRow>
  </TableHeader>
  <TableBody>
    {products.map((product) => (
      <TableRow key={product.productId}>
        <TableCell>{product.productId}</TableCell>
        <TableCell>{product.categoryId}</TableCell>
        <TableCell>{product.name}</TableCell>
        <TableCell>{product.basePrice}</TableCell>
        <TableCell>{product.productState}</TableCell> {/* Displaying the product state here */}
        <TableCell>
          <Button onClick={() => setEditingProduct(product)} className="mr-2">Edit</Button>
          <Button onClick={() => handleDeleteProduct(product.productId)} variant="destructive" className="mr-2">Delete</Button>
          <Button onClick={() => setShowingVariations(product.productId)}>Variations</Button>
        </TableCell>
      </TableRow>
    ))}
  </TableBody>
</Table>

      {showingVariations && (
  <div>
    <h2>Variations</h2>
    <Button onClick={() => setIsAddingVariation(true)}>Add Variation</Button>
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Name</TableHead>
          <TableHead>Price Modifier</TableHead>
          <TableHead>State</TableHead>
          <TableHead>Actions</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        {variations?.map((variation) => (
          <TableRow key={variation.variationId}>
            <TableCell>{variation.name}</TableCell>
            <TableCell>{variation.priceModifier}</TableCell>
            <TableCell>{variation.productState}</TableCell>
            <TableCell>
              <Button onClick={() => setEditingVariation(variation)} className="mr-2">Edit</Button>
              <Button onClick={() => handleDeleteVariation(variation.variationId)} variant="destructive" className="mr-2">Delete</Button>
            </TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>

    {(isAddingVariation || editingVariation) && (
  <form onSubmit={handleVariationSubmit} className="space-y-4">
    <div>
      <Label htmlFor="name">Variation Name</Label>
      <Input id="name" name="name" defaultValue={editingVariation ? editingVariation.name : ''} required />
    </div>
    <div>
      <Label htmlFor="priceModifier">Price Modifier</Label>
      <Input
        id="priceModifier"
        name="priceModifier"
        type="number"
        defaultValue={editingVariation ? editingVariation.priceModifier : ''} 
        required
      />
    </div>
    <div>
      <Label htmlFor="productState">State</Label>
      <Select name="productState" defaultValue={editingVariation ? editingVariation.productState : ''} required>
        <SelectTrigger>
          <SelectValue placeholder="Select a state" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="Available">Available</SelectItem>
          <SelectItem value="Unavailable">Unavailable</SelectItem>
        </SelectContent>
      </Select>
    </div>
    <Button type="submit">{editingVariation ? 'Update Variation' : 'Add Variation'}</Button>
    <Button type="button" variant="outline" onClick={() => { setIsAddingVariation(false); setEditingVariation(null); }}>
      Cancel
    </Button>
  </form>
)}
  </div>
)}
    </div>
  );
};

export default ProductsPage;
