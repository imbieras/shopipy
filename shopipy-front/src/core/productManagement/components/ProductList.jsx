import { Button } from "@/components/ui/button";
import { ProductVariationModal } from "./ProductVariationModal";
import { useQuery } from "@tanstack/react-query";
import { productsApi } from '@/core/productManagement/services/productsApi';
import { useUser } from "@/hooks/useUser";
import { useState } from "react";

export function ProductList({ searchTerm, selectedCategory, onAddToOrder }) {
  const { businessId } = useUser();
  const [selectedProduct, setSelectedProduct] = useState(null);

  const { data: productsResponse, isLoading } = useQuery({
    queryKey: ['products', businessId, selectedCategory],
    queryFn: async () => {
      const response = await productsApi.getAllProducts(businessId, { 
        categoryId: selectedCategory 
      });
      return response;
    },
    enabled: !!businessId
  });

  const products = productsResponse?.data || [];

  const filteredProducts = products.filter((product) =>
    product.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleProductClick = (product) => {
    if (product.variations?.length > 0) {
      setSelectedProduct(product);
    } else {
      onAddToOrder(product);
    }
  };

  if (isLoading) {
    return <div className="flex justify-center items-center h-full">Loading products...</div>;
  }

  return (
    <>
      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 mt-4 overflow-y-auto">
        {filteredProducts.map((product) => (
          <Button
            key={product.productId}
            variant="outline"
            className="h-24 flex flex-col items-center justify-center text-center p-2"
            onClick={() => handleProductClick(product)}
          >
            <span className="font-bold">{product.name}</span>
            <span>${product.basePrice.toFixed(2)}</span>
            {product.variations?.length > 0 && (
              <span className="text-xs text-muted-foreground">
                {product.variations.length} variations
              </span>
            )}
          </Button>
        ))}
      </div>
      {selectedProduct && (
        <ProductVariationModal
          product={selectedProduct}
          onClose={() => setSelectedProduct(null)}
          onAddToOrder={(variation) => {
            onAddToOrder(selectedProduct, variation);
            setSelectedProduct(null);
          }}
        />
      )}
    </>
  );
}