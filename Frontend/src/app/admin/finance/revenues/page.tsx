'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface RevenueRecord {
  id: number;
  sourceType: string;
  amount: number;
  description?: string;
  revenueDate: string;
  patientId?: number;
}

export default function RevenuesPage() {
  const router = useRouter();
  const { user, isAuthenticated, logout } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: revenues, isLoading } = useQuery<RevenueRecord[]>({
    queryKey: ['revenues'],
    queryFn: async () => {
      const response = await api.get('/revenues');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Admin',
  });

  if (!isAuthenticated || user?.role !== 'Admin') {
    return null;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/admin/finance" className="text-primary-600 hover:text-primary-700">
                ← Back to Finance
              </Link>
              <h1 className="text-xl font-semibold">Revenues</h1>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-sm text-gray-700">{user?.firstName} {user?.lastName}</span>
              <button
                onClick={logout}
                className="px-4 py-2 text-sm text-white bg-red-600 rounded-md hover:bg-red-700"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          {isLoading ? (
            <div className="text-center py-12">Loading...</div>
          ) : revenues && revenues.length > 0 ? (
            <div className="bg-white shadow overflow-hidden sm:rounded-md">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Source</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Description</th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Amount</th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {revenues.map((revenue) => (
                    <tr key={revenue.id}>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {new Date(revenue.revenueDate).toLocaleDateString()}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {revenue.sourceType}
                      </td>
                      <td className="px-6 py-4 text-sm text-gray-500">
                        {revenue.description || '-'}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-green-600">
                        ${revenue.amount.toLocaleString()}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <div className="bg-white shadow rounded-lg p-12 text-center text-gray-500">
              No revenue records found.
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

