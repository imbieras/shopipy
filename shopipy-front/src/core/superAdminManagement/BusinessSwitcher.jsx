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
        <div className="p-4">
            <h1 className="text-2xl font-bold">Switch Business</h1>
            {businesses.length > 0 ? (
                <ul className="mt-4">
                    {businesses.map((business) => (
                        <li key={business.businessId} className="mb-2">
                            <button
                                className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-700"
                                onClick={() => handleSelectBusiness(business.businessId)}
                            >
                                {business.name}
                            </button>
                        </li>
                    ))}
                </ul>
            ) : (
                <p>Loading businesses...</p>
            )}
        </div>
    );
};

export default BusinessSwitcher;
