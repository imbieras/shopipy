import { useState } from 'react';
import { Clock, User, ChevronRight } from 'lucide-react';
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { useNavigate } from 'react-router-dom';
import { useUser } from '@/hooks/useUser';

export function OrderListItem({ order, onCancel, isExpanded, onToggle }) {
  const { role } = useUser();
  const navigate = useNavigate();

  const getStatusColor = (orderStatus) => {
    if (!orderStatus) return 'bg-gray-500';
    
    switch (orderStatus.toLowerCase()) {
      case 'pending': return 'bg-yellow-500';
      case 'inprogress': return 'bg-blue-500';
      case 'completed': return 'bg-green-500';
      case 'cancelled': return 'bg-red-500';
      default: return 'bg-gray-500';
    }
  };

  const formatTime = (dateString) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffInMinutes = Math.floor((now - date) / (1000 * 60));
   
    if (diffInMinutes < 60) {
      return `${diffInMinutes} min ago`;
    } else if (diffInMinutes < 1440) {
      return `${Math.floor(diffInMinutes / 60)} hours ago`;
    }
    return date.toLocaleDateString();
  };

  return (
    <div
      className={`mb-4 p-4 border rounded-lg cursor-pointer transition-colors ${
        isExpanded ? 'bg-blue-50 border-blue-500' : 'bg-white hover:bg-gray-50'
      }`}
      onClick={onToggle}
    >
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-lg font-semibold">{order.customerName}</h2>
          <p className="text-sm text-gray-500">Order #{order.orderId}</p>
        </div>
        <ChevronRight className={`transition-transform ${isExpanded ? 'rotate-90' : ''}`} />
      </div>
      <div className="mt-2 flex justify-between items-center">
        <div className="flex items-center space-x-4">
          <Badge variant="secondary" className="flex items-center">
            <User className="w-4 h-4 mr-1" />
            {order.orderItems?.length || 0} items
          </Badge>
          <Badge variant="secondary" className="flex items-center">
            <Clock className="w-4 h-4 mr-1" />
            {formatTime(order.createdAt)}
          </Badge>
        </div>
        <Badge className={`${getStatusColor(order.orderStatus)} text-white`}>
          {order.orderStatus}
        </Badge>
      </div>
      {isExpanded && (
        <div className="mt-4 flex justify-end space-x-2">
          {(role === 'BusinessOwner' || role === 'Employee') && order.orderStatus.toLowerCase() !== 'cancelled' && (
            <Button
              variant="destructive"
              onClick={(e) => {
                e.stopPropagation();
                onCancel(order.orderId);
              }}
            >
              Cancel Order
            </Button>
          )}
          <Button
            variant="secondary"
            onClick={(e) => {
              e.stopPropagation();
              navigate(`/orders/${order.orderId}`);
            }}
          >
            View Details
          </Button>
        </div>
      )}
    </div>
  );
}