'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface FinancialSummary {
  totalRevenue: number;
  totalExpenses: number;
  netProfit: number;
  totalInvoices: number;
  paidInvoices: number;
}

export default function FinancePage() {
  const router = useRouter();
  const { user, isAuthenticated, logout } = useAuthStore();
  const [startDate, setStartDate] = useState(new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]);
  const [endDate, setEndDate] = useState(new Date().toISOString().split('T')[0]);

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: financial, isLoading } = useQuery<FinancialSummary>({
    queryKey: ['financial-summary', startDate, endDate],
    queryFn: async () => {
      const response = await api.get(`/financial-reports/profit-loss?startDate=${startDate}&endDate=${endDate}`);
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
              <Link href="/admin/dashboard" className="text-primary-600 hover:text-primary-700">
                ← Back to Dashboard
              </Link>
              <h1 className="text-xl font-semibold">Financial Management</h1>
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
          <div className="mb-6 flex justify-between items-center">
            <h2 className="text-2xl font-bold">Financial Overview</h2>
            <div className="flex space-x-4">
              <input
                type="date"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
                className="px-3 py-2 border border-gray-300 rounded-md"
              />
              <input
                type="date"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
                className="px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
          </div>

          {isLoading ? (
            <div className="text-center py-12">Loading...</div>
          ) : financial ? (
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
              <div className="bg-white shadow rounded-lg p-6">
                <h3 className="text-sm font-medium text-gray-500">Total Revenue</h3>
                <p className="mt-2 text-3xl font-semibold text-green-600">
                  ${financial.totalRevenue.toLocaleString()}
                </p>
              </div>
              <div className="bg-white shadow rounded-lg p-6">
                <h3 className="text-sm font-medium text-gray-500">Total Expenses</h3>
                <p className="mt-2 text-3xl font-semibold text-red-600">
                  ${financial.totalExpenses.toLocaleString()}
                </p>
              </div>
              <div className="bg-white shadow rounded-lg p-6">
                <h3 className="text-sm font-medium text-gray-500">Net Profit</h3>
                <p className={`mt-2 text-3xl font-semibold ${
                  financial.netProfit >= 0 ? 'text-green-600' : 'text-red-600'
                }`}>
                  ${financial.netProfit.toLocaleString()}
                </p>
              </div>
            </div>
          ) : null}

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <Link href="/admin/finance/expenses" className="bg-white shadow rounded-lg p-6 hover:shadow-lg transition">
              <h3 className="text-lg font-semibold mb-2">Expenses</h3>
              <p className="text-sm text-gray-500">Manage and track hospital expenses</p>
            </Link>
            <Link href="/admin/finance/revenues" className="bg-white shadow rounded-lg p-6 hover:shadow-lg transition">
              <h3 className="text-lg font-semibold mb-2">Revenues</h3>
              <p className="text-sm text-gray-500">View revenue records and reports</p>
            </Link>
          </div>
        </div>
      </main>
    </div>
  );
}

