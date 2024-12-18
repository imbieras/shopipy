import { getDiscounts } from './services/DiscountsApi';
import DiscountList from './components/DiscountList';
import AddDiscountForm from './components/AddDiscountForm';
import { useUser } from '@/hooks/useUser';

export default function DiscountsPage({ discounts }) {
  const { businessId } = useUser();

  if (!businessId) {
    return <div>Loading...</div>;
  }

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold">Discounts</h1>
      <AddDiscountForm />
      <DiscountList discounts={discounts} />
    </div>
  );
}

// This function will be called on the server side during the page request
export async function getServerSideProps(context) {
  // Get businessId from cookies or session or from an authenticated API
  // For example, you might be able to get the businessId from a user session:
  const { businessId } = await getBusinessIdFromSession(context); // You need to implement this logic

  // If you can't find the businessId, redirect or show an error
  if (!businessId) {
    return { redirect: { destination: '/login', permanent: false } };
  }

  // Fetch the discounts from the API using the businessId
  const discounts = await getDiscounts(businessId);

  // Pass the fetched data as props to the page component
  return {
    props: { discounts }, // This will be passed to your page as a prop
  };
}
