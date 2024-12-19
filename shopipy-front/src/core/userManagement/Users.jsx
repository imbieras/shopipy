'use client';

import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { userApi } from './services/UsersApi.jsx';
import { useUser } from '@/hooks/useUser';

const AddUserForm = ({ onClose }) => {
    const { businessId } = useUser(); // Get the business ID
    const queryClient = useQueryClient(); // To manage query cache
    const [role, setRole] = useState(''); // For storing the selected role
    
    // Mutation for creating a new user
    const createUserMutation = useMutation({
      mutationFn: (userData) => userApi.createUser(businessId, userData), // Pass the businessId to the API call
      onSuccess: () => {
        // Invalidate the users query to fetch updated data after user creation
        queryClient.invalidateQueries(['users', businessId]);
        onClose(); // Close the form on success
      },
      onError: (error) => {
        // Handle error
        alert(`Failed to add user: ${error.message}`);
      },
    });
  
    const handleSubmit = async (e) => {
      e.preventDefault();
      const formData = new FormData(e.target);
      const data = {
        email: formData.get('email'),
        name: formData.get('name'),
        role, // The role selected from the dropdown
        phone: formData.get('phone'),
        businessId, // Include the businessId in the request body
        password: formData.get('password'),
      };
      await createUserMutation.mutateAsync(data); // Trigger the mutation
    };
  
    return (
      <form onSubmit={handleSubmit} className="space-y-4">
        {/* Email input */}
        <div>
          <Label htmlFor="email">Email</Label>
          <Input id="email" name="email" type="email" required />
        </div>
  
        {/* Name input */}
        <div>
          <Label htmlFor="name">Name</Label>
          <Input id="name" name="name" required />
        </div>
  
        {/* Role selection */}
        <div>
          <Label htmlFor="role">Role</Label>
          <Select onValueChange={(value) => setRole(value)} required>
            <SelectTrigger>
              <SelectValue placeholder="Select a role" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="BusinessOwner">Business owner</SelectItem>
              <SelectItem value="Employee">Employee</SelectItem> {/* Added the 'Employee' role */}
            </SelectContent>
          </Select>
        </div>
  
        {/* Phone input */}
        <div>
          <Label htmlFor="phone">Phone</Label>
          <Input id="phone" name="phone" type="tel" />
        </div>
  
        {/* Password input */}
        <div>
          <Label htmlFor="password">Password</Label>
          <Input id="password" name="password" type="password" required />
        </div>
  
        {/* Buttons */}
        <div className="flex gap-2">
          <Button type="submit" isLoading={createUserMutation.isLoading}>Add User</Button>
          <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
        </div>
      </form>
    );
  };



const UpdateUserForm = ({ user, onClose }) => {
    const queryClient = useQueryClient();
    const { businessId } = useUser();
  
    const [role, setRole] = useState(user.role);
    const [userState, setUserState] = useState(user.userState);
  
    const updateUserMutation = useMutation({
      mutationFn: (updatedUserData) =>
        userApi.updateUser(businessId, user.userId, {
          ...updatedUserData,
          role,
          userState,
        }),
      onSuccess: () => {
        queryClient.invalidateQueries(['users', businessId]);
        onClose();
      },
      onError: (error) => {
        alert(`Failed to update user: ${error.message}`);
      },
    });
  
    const handleSubmit = async (e) => {
      e.preventDefault();
      const formData = new FormData(e.target);
      const data = {
        email: formData.get('email'),
        name: formData.get('name'),
        phone: formData.get('phone'),
        businessId: formData.get('businessId'),
      };
      await updateUserMutation.mutateAsync(data);
    };
  
    return (
      <form onSubmit={handleSubmit} className="space-y-4 mb-4">
        <div>
          <Label htmlFor="email">Email</Label>
          <Input id="email" name="email" type="email" defaultValue={user.email} required />
        </div>
        <div>
          <Label htmlFor="name">Name</Label>
          <Input id="name" name="name" defaultValue={user.name} required />
        </div>
        <div>
          <Label htmlFor="role">Role</Label>
          <Select name="role" value={role} onValueChange={setRole} required>
            <SelectTrigger>
              <SelectValue placeholder="Select a role" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="BusinessOwner">Business owner</SelectItem>
              <SelectItem value="Employee">Employee</SelectItem>
            </SelectContent>
          </Select>
        </div>
        <div>
          <Label htmlFor="phone">Phone</Label>
          <Input id="phone" name="phone" type="tel" defaultValue={user.phone} />
        </div>
        <div>
          <Label htmlFor="businessId">Business ID</Label>
          <Input id="businessId" name="businessId" type="number" defaultValue={user.businessId} />
        </div>
        <div>
          <Label htmlFor="userState">User State</Label>
          <Select name="userState" value={userState} onValueChange={setUserState} required>
            <SelectTrigger>
              <SelectValue placeholder="Select a state" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="Active">Active</SelectItem>
              <SelectItem value="Inactive">Inactive</SelectItem>
            </SelectContent>
          </Select>
        </div>
        <Button type="submit">Update User</Button>
        <Button type="button" variant="outline" onClick={onClose} className="ml-2">
          Cancel
        </Button>
      </form>
    );
  };
  
  
  // User List Component with Edit Button
  const UserList = ({ users, onEdit }) => {
    const { businessId } = useUser();
    const queryClient = useQueryClient();
  
    const deleteUserMutation = useMutation({
      mutationFn: (userId) => userApi.deleteUser(businessId, userId),
      onSuccess: () => {
        queryClient.invalidateQueries(['users', businessId]);
      },
    });
  
    const handleDelete = (userId) => {
      if (window.confirm('Are you sure you want to delete this user?')) {
        deleteUserMutation.mutate(userId);
      }
    };
  
    return (
      <table className="min-w-full divide-y divide-gray-200">
        <thead className="bg-gray-50">
          <tr>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Name</th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Email</th>
            <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Role</th>
            <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-200">
          {users.map((user) => (
            <tr key={user.userId}>
              <td className="px-6 py-4 whitespace-nowrap">{user.name}</td>
              <td className="px-6 py-4 whitespace-nowrap">{user.email}</td>
              <td className="px-6 py-4 whitespace-nowrap">{user.role}</td>
              <td className="px-6 py-4 whitespace-nowrap text-right">
                <Button className="mr-2" size="sm" onClick={() => onEdit(user)}>
                  Edit
                </Button>
                <Button variant="destructive" size="sm" onClick={() => handleDelete(user.userId)}>
                  Delete
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    );
  };
  
  // Users Page Component
  const UsersPage = () => {
    const [isAdding, setIsAdding] = useState(false);
    const [editingUser, setEditingUser] = useState(null); // Track the user being edited
    const { businessId } = useUser();
  
    const { data: users = [], isLoading, error } = useQuery({
      queryKey: ['users', businessId],
      queryFn: () => userApi.getUsers(businessId),
    });
  
    if (isLoading) return <div>Loading...</div>;
    if (error) return <div>Error loading users</div>;
  
    return (
      <div className="space-y-6">
        <h1 className="text-3xl font-bold">Users</h1>
  
        {isAdding ? (
          <AddUserForm onClose={() => setIsAdding(false)} />
        ) : editingUser ? (
          <UpdateUserForm user={editingUser} onClose={() => setEditingUser(null)} />
        ) : (
          <Button onClick={() => setIsAdding(true)}>Add User</Button>
        )}
  
        <UserList users={users} onEdit={setEditingUser} />
      </div>
    );
  };
  
  export default UsersPage;
  