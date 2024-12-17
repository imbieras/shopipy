import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { categoryApi } from './services/CategoryApi';
import { useUser } from '@/hooks/useUser';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Pencil, Trash2, Plus } from "lucide-react";

export default function Categories() {
  const queryClient = useQueryClient();
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [isEditOpen, setIsEditOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [name, setName] = useState('');
  const { businessId } = useUser();

  // Queries
  const { data: categories, isLoading } = useQuery({
    queryKey: ['categories', businessId],
    queryFn: () => categoryApi.getCategories(businessId)
  });

  // Mutations
  const createMutation = useMutation({
    mutationFn: (newCategory) => categoryApi.createCategory(businessId, newCategory),
    onSuccess: () => {
      queryClient.invalidateQueries(['categories', businessId]);
      setIsCreateOpen(false);
      setName('');
      //alert('Category created successfully');
    },
    onError: () => console.error('Failed to create category')
  });

  const updateMutation = useMutation({
    mutationFn: ({ categoryId, data }) => categoryApi.updateCategory(businessId, categoryId, data),
    onSuccess: () => {
      queryClient.invalidateQueries(['categories', businessId]);
      setIsEditOpen(false);
      setSelectedCategory(null);
      setName('');
      //alert('Category updated successfully');
    },
    onError: () => alert('Failed to update category')
  });

  const deleteMutation = useMutation({
    mutationFn: (categoryId) => categoryApi.deleteCategory(businessId, categoryId),
    onSuccess: () => {
      queryClient.invalidateQueries(['categories', businessId]);
    //   alert('Category deleted successfully');
    },
    onError: () => console.error('Failed to delete category')
  });

  const handleSubmit = (e) => {
    e.preventDefault();
    if (selectedCategory) {
      updateMutation.mutate({
        categoryId: selectedCategory.categoryId,
        data: { name }
      });
    } else {
      createMutation.mutate({ name });
    }
  };

  const handleEdit = (category) => {
    setSelectedCategory(category);
    setName(category.name);
    setIsEditOpen(true);
  };

  const handleDelete = (categoryId) => {
    if (window.confirm('Are you sure you want to delete this category?')) {
      deleteMutation.mutate(categoryId);
    }
  };

  if (isLoading) {
    return <div className="flex justify-center items-center h-96">Loading...</div>;
  }

  return (
    <Card className="max-w-4xl mx-auto my-8">
      <CardHeader>
        <div className="flex justify-between items-center">
          <div>
            <CardTitle>Categories</CardTitle>
            <CardDescription>Manage your business categories</CardDescription>
          </div>
          <Dialog open={isCreateOpen} onOpenChange={setIsCreateOpen}>
            <DialogTrigger asChild>
              <Button className="flex items-center gap-2">
                <Plus className="h-4 w-4" /> Add Category
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Create New Category</DialogTitle>
              </DialogHeader>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="name">Name</Label>
                  <Input
                    id="name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required
                  />
                </div>
                <Button type="submit" className="w-full">
                  Create Category
                </Button>
              </form>
            </DialogContent>
          </Dialog>
        </div>
      </CardHeader>
      <CardContent>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead className="w-[100px] text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {categories?.map((category) => (
              <TableRow key={category.categoryId}>
                <TableCell className="font-medium">{category.name}</TableCell>
                <TableCell className="text-right">
                  <div className="flex items-center justify-end gap-2">
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => handleEdit(category)}
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => handleDelete(category.categoryId)}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </CardContent>

      {/* Edit Dialog */}
      <Dialog open={isEditOpen} onOpenChange={setIsEditOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Edit Category</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="edit-name">Name</Label>
              <Input
                id="edit-name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
              />
            </div>
            <Button type="submit" className="w-full">
              Update Category
            </Button>
          </form>
        </DialogContent>
      </Dialog>
    </Card>
  );
}