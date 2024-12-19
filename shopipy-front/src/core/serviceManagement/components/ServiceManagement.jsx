import React, { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
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
import { useBusiness } from "@/hooks/useUser";
import { categoryApi } from "../../categoryManagement/services/CategoryApi";
import { serviceApi } from "../services/ServiceApi";
import { useMutation, useQueryClient } from "@tanstack/react-query";

const ServiceManagement = ({
  service,
  onClose,
  mode = "edit",
  onServiceSaved,
}) => {
  const initialState = {
    serviceName: "",
    serviceDuration: 30,
    servicePrice: 0,
    categoryId: "",
    serviceDescription: "",
    isServiceActive: true,
  };
  const queryClient = useQueryClient();

  const [serviceData, setServiceData] = useState(
    mode === "edit" ? null : initialState
  );
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const { businessId } = useBusiness();

  useEffect(() => {
    if (businessId) {
      fetchCategories();
    }
  }, [businessId]);

  useEffect(() => {
    if (mode === "edit" && service) {
      setServiceData({
        id: service.serviceId,
        serviceName: service.serviceName || "",
        serviceDuration: service.serviceDuration || 30,
        servicePrice: service.serviceBasePrice || 0,
        categoryId: service.categoryId?.toString() || "",
        serviceDescription: service.serviceDescription || "",
        isServiceActive: service.isServiceActive ?? true,
      });
    }
  }, [service, mode]);

  const fetchCategories = async () => {
    try {
      const response = await categoryApi.getCategories(businessId);
      setCategories(response);
    } catch (error) {
      console.error("Error fetching categories: ", error);
      setError("Failed to fetch categories");
    }
  };

  const { mutate: updateService, isPending: isUpdating } = useMutation({
    mutationFn: async (serviceData) => {
      return await serviceApi.updateService(
        businessId,
        serviceData.id,
        serviceData
      );
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["services", businessId] });
      onClose?.();
    },
    onError: (error) => {
      setError("Failed to save service: " + error.message);
    },
  });

  const handleSave = async () => {
    if (!serviceData) return;
    if (!serviceData.serviceName || !serviceData.categoryId) {
      setError("Please fill in all required fields");
      return;
    }

    const serviceRequestDto = {
      categoryId: parseInt(serviceData.categoryId),
      serviceName: serviceData.serviceName,
      serviceDescription: serviceData.serviceDescription || "",
      servicePrice: parseFloat(serviceData.servicePrice) || 0,
      serviceDuration: parseInt(serviceData.serviceDuration) || 30,
      isServiceActive: Boolean(serviceData.isServiceActive),
    };

    if (mode === "edit") {
      updateService({ ...serviceRequestDto, id: serviceData.id });
    } else {
      onServiceSaved(serviceRequestDto);
    }
  };

  if (loading) return <div>Loading...</div>;
  if (mode === "edit" && !service) return null;
  if (!serviceData) return null;

  return (
    <Card className="border-0 shadow-none">
      <CardHeader>
        <CardTitle>
          {mode === "edit" ? "Edit Service" : "Create Service"}
        </CardTitle>
        <CardDescription>
          {mode === "edit"
            ? `Make changes to ${service.serviceName}`
            : "Create a new service for your business"}
        </CardDescription>
      </CardHeader>
      <CardContent>
        {error && <div className="text-red-600 mb-4">Error: {error}</div>}
        <div className="space-y-4">
          <div>
            <Label htmlFor="name">Name</Label>
            <Input
              id="name"
              value={serviceData.serviceName}
              onChange={(e) =>
                setServiceData({ ...serviceData, serviceName: e.target.value })
              }
            />
          </div>
          <div>
            <Label htmlFor="duration">Duration (minutes)</Label>
            <Input
              id="duration"
              type="number"
              value={serviceData.serviceDuration}
              onChange={(e) =>
                setServiceData({
                  ...serviceData,
                  serviceDuration: parseInt(e.target.value, 10),
                })
              }
            />
          </div>
          <div>
            <Label htmlFor="price">Price ($)</Label>
            <Input
              id="price"
              type="number"
              value={serviceData.servicePrice}
              onChange={(e) =>
                setServiceData({
                  ...serviceData,
                  servicePrice: parseFloat(e.target.value),
                })
              }
            />
          </div>
          <div>
            <Label htmlFor="category">Category</Label>
            <Select
              value={serviceData.categoryId?.toString()}
              onValueChange={(value) =>
                setServiceData({
                  ...serviceData,
                  categoryId: value,
                })
              }
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
              value={serviceData.serviceDescription}
              onChange={(e) =>
                setServiceData({
                  ...serviceData,
                  serviceDescription: e.target.value,
                })
              }
            />
          </div>
          <div className="flex items-center space-x-2">
            <Switch
              id="active"
              checked={serviceData.isServiceActive}
              onCheckedChange={(checked) =>
                setServiceData((prev) => ({
                  ...prev,
                  isServiceActive: checked,
                }))
              }
            />
            <Label htmlFor="active">Service Active</Label>
          </div>
        </div>
      </CardContent>
      <CardFooter className="flex justify-end space-x-2">
        <Button variant="outline" onClick={onClose}>
          Cancel
        </Button>
        <Button onClick={handleSave}>
          {mode === "edit" ? "Save Changes" : "Create Service"}
        </Button>
      </CardFooter>
    </Card>
  );
};

export default ServiceManagement;
