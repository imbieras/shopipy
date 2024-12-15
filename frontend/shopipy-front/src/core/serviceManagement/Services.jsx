import { useEffect } from 'react';
import { useUser } from '../../hooks/useUser';

const Services = ({ onLogout }) => {
    const { id, username, email, name } = useUser();

    useEffect(() => {
        console.log("ID: ", id);
    }, []);

    return (
        <div className="p-4">
            <div className="mb-4 flex justify-between items-center">
                <h1 className="text-2xl font-bold">Appointments</h1>
            </div>
            
            <div className="bg-white shadow rounded-lg p-6">
                <h2 className="text-xl font-semibold mb-4">User Information</h2>
                <div className="space-y-2">
                    <p><span className="font-medium">ID:</span> {id}</p>
                    <p><span className="font-medium">Username:</span> {username}</p>
                    <p><span className="font-medium">Email:</span> {email}</p>
                    <p><span className="font-medium">Name:</span> {name}</p>
                </div>
            </div>
        </div>
    );
};

export default Services;
