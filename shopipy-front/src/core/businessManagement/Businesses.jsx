import {useState} from 'react';
import {Button} from "@/components/ui/button";
import {Input} from "@/components/ui/input";
import {Label} from "@/components/ui/label";
import {Textarea} from "@/components/ui/textarea";
import {Select, SelectContent, SelectItem, SelectTrigger, SelectValue} from "@/components/ui/select";
import {Table, TableBody, TableCell, TableHead, TableHeader, TableRow} from "@/components/ui/table";
import {businessApi} from './services/BusinessApi.jsx';
import {useMutation, useQuery, useQueryClient} from '@tanstack/react-query';

const BusinessesPage = () => {
  const queryClient = useQueryClient();
  const [isAdding, setIsAdding] = useState(false);
  const [editingBusiness, setEditingBusiness] = useState(null);

  const {data: businesses = [], isLoading, error} = useQuery({
    queryKey: ['businesses'],
    queryFn: businessApi.getBusinesses,
  });

  const createBusinessMutation = useMutation({
    mutationFn: (businessData) => businessApi.createBusiness(businessData),
    onSuccess: () => {
      queryClient.invalidateQueries(['businesses']);
      setIsAdding(false);
    },
    onError: (error) => {
      alert(`Failed to create business: ${error.message}`);
    },
  });

  const updateBusinessMutation = useMutation({
    mutationFn: (updatedBusinessData) =>
      businessApi.updateBusiness(updatedBusinessData.businessId, updatedBusinessData),
    onSuccess: () => {
      queryClient.invalidateQueries(['businesses']);
      setEditingBusiness(null);
    },
    onError: (error) => {
      alert(`Failed to update business: ${error.message}`);
    },
  });

  const handleBusinessSubmit = async (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);

    const data = {
      businessId: editingBusiness?.businessId,
      name: formData.get('name'),
      address: formData.get('address'),
      phone: formData.get('phone') || null,
      email: formData.get('email'),
      vatNumber: formData.get('vatNumber') || null,
      businessType: formData.get('businessType'),
    };

    if (editingBusiness) {
      await updateBusinessMutation.mutateAsync(data);
    } else {
      await createBusinessMutation.mutateAsync(data);
    }
  };

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error loading businesses</div>;

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Businesses</h1>

      {isAdding || editingBusiness ? (
        <form onSubmit={handleBusinessSubmit} className="space-y-4 mb-4">
          <Label htmlFor="name">Name</Label>
          <Input
            id="name"
            name="name"
            maxLength={255}
            required
            defaultValue={editingBusiness?.name || ''}
          />

          <Label htmlFor="address">Address</Label>
          <Textarea
            id="address"
            name="address"
            maxLength={500}
            required
            defaultValue={editingBusiness?.address || ''}
          />

          <Label htmlFor="phone">Phone</Label>
          <Input
            id="phone"
            name="phone"
            type="tel"
            maxLength={20}
            defaultValue={editingBusiness?.phone || ''}
          />

          <Label htmlFor="email">Email</Label>
          <Input
            id="email"
            name="email"
            type="email"
            maxLength={255}
            required
            defaultValue={editingBusiness?.email || ''}
          />

          <Label htmlFor="vatNumber">VAT Number</Label>
          <Input
            id="vatNumber"
            name="vatNumber"
            maxLength={50}
            defaultValue={editingBusiness?.vatNumber || ''}
          />

          <Label htmlFor="businessType">Business Type</Label>
          <Select
            name="businessType"
            required
            defaultValue={editingBusiness?.businessType || ''}
          >
            <SelectTrigger>
              <SelectValue placeholder="Select a business type"/>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="Retail">Retail</SelectItem>
              <SelectItem value="Service">Service</SelectItem>
              <SelectItem value="Manufacturing">Manufacturing</SelectItem>
              <SelectItem value="Wholesale">Wholesale</SelectItem>
              <SelectItem value="Other">Other</SelectItem>
            </SelectContent>
          </Select>

          <Button type="submit">{isAdding ? 'Add Business' : 'Update Business'}</Button>
          <Button
            type="button"
            variant="outline"
            onClick={() => {
              setIsAdding(false);
              setEditingBusiness(null);
            }}
          >
            Cancel
          </Button>
        </form>
      ) : (
        <Button onClick={() => setIsAdding(true)}>Add Business</Button>
      )}


      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>ID</TableHead>
            <TableHead>Name</TableHead>
            <TableHead>Address</TableHead>
            <TableHead>Phone</TableHead>
            <TableHead>Email</TableHead>
            <TableHead>VAT Number</TableHead>
            <TableHead>Business Type</TableHead>
            <TableHead>Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {businesses.map((business) => (
            <TableRow key={business.businessId}>
              <TableCell>{business.businessId}</TableCell>
              <TableCell>{business.name}</TableCell>
              <TableCell>{business.address}</TableCell>
              <TableCell>{business.phone || 'N/A'}</TableCell>
              <TableCell>{business.email}</TableCell>
              <TableCell>{business.vatNumber || 'N/A'}</TableCell>
              <TableCell>{business.businessType}</TableCell>
              <TableCell>
                <Button onClick={() => setEditingBusiness(business)} className="mr-2">Edit</Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
};

export default BusinessesPage;
