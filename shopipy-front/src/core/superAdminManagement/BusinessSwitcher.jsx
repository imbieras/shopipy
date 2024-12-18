import React, { useState, useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { businessApi } from '../businessManagement/services/BusinessApi';
import { useUser } from '@/hooks/useUser';

const fetchBusinesses = async () => {
    const data = await businessApi.getBusinesses();
    return data;
};

const BusinessSwitcher = () => {
    const { setBusinessId, businessId } = useUser();

    // Fetch businesses using useQuery
    const { data: businesses = [], isLoading, error } = useQuery({
        queryKey: ['businesses'],
        queryFn: fetchBusinesses,
    });

    const handleSelectBusiness = (selectedBusinessId) => {
        setBusinessId(selectedBusinessId); 
        console.log(`Selected Business ID: ${selectedBusinessId}`);
    };

    if (isLoading) {
        return <div className="text-center text-gray-700">Loading businesses...</div>;
    }

    if (error) {
        return (
            <div className="text-center text-red-500">
                Failed to load businesses: {error.message}
            </div>
        );
    }

    return (
        <div className="container mx-auto p-8">
            <h1 className="text-3xl font-bold mb-6 text-gray-800">Switch Business</h1>
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
                {businesses.map((business) => (
                    <div
                        key={business.businessId}
                        className={`p-4 border rounded-lg shadow-md cursor-pointer transition-transform transform hover:scale-105 ${
                            businessId === business.businessId
                                ? 'bg-blue-500 text-white'
                                : 'bg-white text-gray-700'
                        }`}
                        onClick={() => handleSelectBusiness(business.businessId)}
                    >
                        <h2 className="text-lg font-semibold text-center">
                            {business.name}
                        </h2>
                    </div>
                ))}
            </div>
        </div>
    );
};


export default BusinessSwitcher;
