import React, { useState } from 'react';
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Switch } from '@/components/ui/switch';
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useUser } from '@/hooks/useUser';
import { useEffect } from 'react';
import { serviceApi } from './services/ServiceApi';
import { categoryApi } from '../categoryManagement/services/CategoryApi';

const ServiceManagement = () => {
  const [serviceId, setServiceId] = useState('');
  const [businessId, setBusinessId] = useState('');
  const [selectedService, setSelectedService] = useState(null);
  const [editedService, setEditedService] = useState(null);
  const [services, setServices] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const { role } = useUser();

  useEffect(() => {
    if (businessId) {
      fetchServices();
      fetchCategories();
    }
  }, [businessId]);

  const fetchCategories = async () => {
    try {
      const response = await categoryApi.getCategories(businessId);
      setCategories(response);
    } catch (error) {
      console.error('Error fetching categories: ', error);
    }
  }

  const fetchServices = async () => {
    try {
      setLoading(true);
      const response = await serviceApi.getServices(businessId);

      setServices(response);
      setError(null);
    } catch (err) {
      setError('Failed to fetch services');
      console.error('Error fetching services:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleFindService = async () => {
    try {
      setLoading(true);
      const service = await serviceApi.getServiceById(businessId, serviceId);
      console.log(service);
      if(service) {
        setSelectedService(service);
        setEditedService({
          id: service.serviceId,
          serviceName: service.serviceName || '',
          serviceDuration: service.serviceDuration || 0,
          servicePrice: service.serviceBasePrice || 0,
          categoryId: service.categoryId.toString() || '',
          serviceDescription: service.serviceDescription || '',
          isServiceActive: service.isServiceActive
        });
      }
      setError(null);
    } catch (err) {
      setError('Service not found');
      console.error('Error finding service:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateService = async () => {
    if (editedService) {
      try {
        setLoading(true);
        const serviceRequestDto = {
          categoryId: editedService.categoryId,
          serviceName: editedService.serviceName,
          serviceDescription: editedService.serviceDescription,
          servicePrice: editedService.servicePrice,
          serviceDuration: editedService.serviceDuration,
          isServiceActive: editedService.isServiceActive
        };
        await serviceApi.updateService(businessId, editedService.id, serviceRequestDto);
        await fetchServices();
        setSelectedService(null);
        setEditedService(null);
        setServiceId('');
        alert('Service updated successfully');
      } catch (err) {
        setError('Failed to update service');
        console.error('Error updating service:', err);
      } finally {
        setLoading(false);
      }
    }
  };

  const handleDeleteService = async () => {
    if (selectedService) {
      try {
        setLoading(true);
        await serviceApi.deleteService(businessId, selectedService.id);
        await fetchServices();
        setSelectedService(null);
        setEditedService(null);
        setServiceId('');
        alert('Service deleted successfully');
      } catch (err) {
        setError('Failed to delete service');
        console.error('Error deleting service:', err);
      } finally {
        setLoading(false);
      }
    }
  };

  if (loading) return <div>Loading...</div>;

  return (
    <Card>
      <CardHeader>
        <CardTitle>Manage Services</CardTitle>
        <CardDescription>
          Edit or delete services using the service ID from your SMS.
        </CardDescription>
      </CardHeader>
      <CardContent>
        <div className="space-y-4 w-full max-w-xl">
          <div className="flex gap-4">
            <Input
              placeholder="Enter Service ID"
              value={serviceId}
              onChange={(e) => setServiceId(e.target.value)}
              className="flex-grow"
            />
            <Button 
              onClick={handleFindService}
              className="whitespace-nowrap"
            >
              Find Service
            </Button>
          </div>
          
          {role === 'SuperAdmin' && (
            <div className="flex gap-4">
              <Input
                placeholder="Enter Business ID"
                value={businessId}
                onChange={(e) => setBusinessId(e.target.value)}
                className="flex-grow"
              />
            </div>
          )}
        </div>
        {error && (
          <div className="text-red-600">
            Error: {error}
          </div>
        )}
        {selectedService && editedService && (
          <div className="space-y-4">
            <div>
              <Label htmlFor="name">Name</Label>
              <Input
                id="name"
                value={editedService.serviceName}
                onChange={(e) => setEditedService({ ...editedService, serviceName: e.target.value })}
              />
            </div>
            <div>
              <Label htmlFor="duration">Duration (minutes)</Label>
              <Input
                id="duration"
                type="number"
                value={editedService.serviceDuration}
                onChange={(e) => setEditedService({ 
                  ...editedService, 
                  serviceDuration: parseInt(e.target.value, 10) 
                })}
              />
            </div>
            <div>
              <Label htmlFor="price">Price ($)</Label>
              <Input
                id="price"
                type="number"
                value={editedService.servicePrice}
                onChange={(e) => setEditedService({ 
                  ...editedService, 
                  servicePrice: parseFloat(e.target.value) 
                })}
              />
            </div>
            <div>
              <Label htmlFor="category">Category</Label>
              <Select
                defaultValue={editedService.categoryId?.toString()}
                value={editedService.categoryId?.toString()}
                onValueChange={(value) => setEditedService({ 
                  ...editedService, 
                  categoryId: value 
                })}
              >
              <SelectTrigger id="category">
                <SelectValue defaultValue={categories.find(cat => cat.categoryId === selectedService.categoryId)?.name}>
                  {categories.find(cat => cat.categoryId.toString() === editedService.categoryId?.toString())?.name}
                </SelectValue>
              </SelectTrigger>
                <SelectContent>
                  {categories.map((category) => (
                    <SelectItem 
                      key={category.categoryId} 
                      value={category.categoryId.toString()}
                    >
                      {category.name}
                    </SelectItem>
                  ))}
                </SelectContent>
            </Select>
            </div>
            <div>
              <Label htmlFor="description">Description</Label>
              <Input
                id="description"
                value={editedService.serviceDescription}
                onChange={(e) => setEditedService({ 
                  ...editedService, 
                  serviceDescription: e.target.value 
                })}
              />
            </div>
            <div className="flex items-center space-x-2">
            <Switch
                id="active"
                checked={editedService.isServiceActive}
                onCheckedChange={(checked) => setEditedService(prev => ({
                  ...prev,
                  isServiceActive: checked
                }))}
              />
              <Label htmlFor="active">Service Active</Label>
            </div>
          </div>
        )}
      </CardContent>
      {selectedService && (
        <CardFooter className="flex justify-between">
          <Button variant="outline" onClick={handleDeleteService}>
            Delete Service
          </Button>
          <Button onClick={handleUpdateService}>Update Service</Button>
        </CardFooter>
      )}
    </Card>
  );
};

export default ServiceManagement;