import React, { useState, useEffect } from 'react';
import { businessApi } from '../businessManagement/services/BusinessApi';
import { useUser } from '@/hooks/useUser';

const BusinessSwitcher = () => {
    const [businesses, setBusinesses] = useState([]);
    const [selectedBusiness, setSelectedBusiness] = useState(null);
    const { setBusinessId } = useUser();

    useEffect(() => {
        const fetchBusinesses = async () => {
            try {
                const data = await businessApi.getBusinesses();
                setBusinesses(data);
                console.log(`Fetched buisinesses: ${JSON.stringify(data, null, 2)}`);
            } catch (error) {
                console.error('Failed to fetch businesses:', error);
            }
        };

        fetchBusinesses();
    }, []);

    const handleSelectBusiness = (businessId) => {
        setSelectedBusiness(businessId);
        setBusinessId(businessId);
        console.log(`Selected Business ID: ${businessId}`);
        // all vodoo magic happens here. 
    };

    return (
        <div className="container mx-auto p-8">
            <h1 className="text-3xl font-bold mb-6 text-gray-800">Switch Business</h1>
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
                {businesses.map((business) => (
                    <div
                        key={business.businessId}
                        className={`p-4 border rounded-lg shadow-md cursor-pointer transition-transform transform hover:scale-105 ${
                            selectedBusiness === business.businessId
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
