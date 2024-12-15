import React, { useState } from 'react';
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
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
      console.error('Error fetching categories: ', err);
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
      console.log("Service: ", service);
      console.log("Service base price: ", service.serviceBasePrice);
      if(service) {
        setSelectedService(service);
        setEditedService({
          id: service.serviceId,
          name: service.serviceName || '',
          duration: service.serviceDuration || 0,
          price: service.serviceBasePrice || 0,
          category: service.categoryId || '',
          description: service.serviceDescription || '',
        })
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
        await serviceApi.updateService(businessId, editedService.id, editedService);
        await fetchServices(); // Refresh the services list
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
  if (error) return <div>Error: {error}</div>;

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
        {selectedService && editedService && (
          <div className="space-y-4">
            <div>
              <Label htmlFor="name">Name</Label>
              <Input
                id="name"
                value={editedService.name}
                onChange={(e) => setEditedService({ ...editedService, name: e.target.value })}
              />
            </div>
            <div>
              <Label htmlFor="duration">Duration (minutes)</Label>
              <Input
                id="duration"
                type="number"
                value={editedService.duration}
                onChange={(e) => setEditedService({ 
                  ...editedService, 
                  duration: parseInt(e.target.value, 10) 
                })}
              />
            </div>
            <div>
              <Label htmlFor="price">Price ($)</Label>
              <Input
                id="price"
                type="number"
                value={editedService.price}
                onChange={(e) => setEditedService({ 
                  ...editedService, 
                  price: parseFloat(e.target.value) 
                })}
              />
            </div>
            <div>
              <Label htmlFor="category">Category</Label>
              <Select
                value={editedService.category}
                onValueChange={(value) => setEditedService({ 
                  ...editedService, 
                  category: value 
                })}
              >
                <SelectTrigger id="category">
                  <SelectValue placeholder="Select a category" />
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
                value={editedService.description}
                onChange={(e) => setEditedService({ 
                  ...editedService, 
                  description: e.target.value 
                })}
              />
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