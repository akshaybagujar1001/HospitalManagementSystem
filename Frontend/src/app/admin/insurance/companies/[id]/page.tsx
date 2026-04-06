'use client';

import { useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface InsuranceCompany {
  id: number;
  name: string;
  contactInfo?: string;
  phoneNumber?: string;
  email?: string;
  address?: string;
  policyRules?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export default function InsuranceCompanyDetailPage() {
  const router = useRouter();
  const params = useParams();
  const { user, isAuthenticated, logout } = useAuthStore();
  const companyId = params?.id as string;

  const { data: company, isLoading, error } = useQuery<InsuranceCompany>({
    queryKey: ['insurance-company', companyId],
    queryFn: async () => {
      const response = await api.get(`/insurance-companies/${companyId}`);
      return response.data;
    },
    enabled: !!companyId && isAuthenticated,
  });

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  if (!isAuthenticated || user?.role !== 'Admin') {
    return null;
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div>Loading insurance company details...</div>
      </div>
    );
  }

  if (error || !company) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Insurance Company Not Found</h2>
          <p className="text-gray-600 mb-4">The insurance company you're looking for doesn't exist.</p>
          <Link
            href="/admin/insurance"
            className="px-4 py-2 text-sm text-white bg-primary-600 rounded-md hover:bg-primary-700"
          >
            Back to Insurance Companies
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/admin/insurance" className="text-primary-600 hover:text-primary-700">
                ← Back to Insurance Companies
              </Link>
              <h1 className="text-xl font-semibold">Insurance Company Details</h1>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-sm text-gray-700">{user?.firstName} {user?.lastName}</span>
              <button
                onClick={() => {
                  logout();
                  router.push('/login');
                }}
                className="px-4 py-2 text-sm text-white bg-red-600 rounded-md hover:bg-red-700"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-4xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="bg-white shadow rounded-lg p-6">
            <div className="mb-6 flex justify-between items-start">
              <div>
                <h2 className="text-2xl font-bold text-gray-900">{company.name}</h2>
                <p className="text-sm text-gray-500 mt-1">
                  Created: {new Date(company.createdAt).toLocaleDateString()}
                </p>
              </div>
              <span className={`px-3 py-1 text-xs font-semibold rounded-full ${
                company.isActive 
                  ? 'bg-green-100 text-green-800' 
                  : 'bg-gray-100 text-gray-800'
              }`}>
                {company.isActive ? 'Active' : 'Inactive'}
              </span>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {company.phoneNumber && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Phone Number</label>
                  <p className="mt-1 text-sm text-gray-900">{company.phoneNumber}</p>
                </div>
              )}

              {company.email && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Email</label>
                  <p className="mt-1 text-sm text-gray-900">
                    <a href={`mailto:${company.email}`} className="text-primary-600 hover:text-primary-700">
                      {company.email}
                    </a>
                  </p>
                </div>
              )}

              {company.address && (
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-gray-700">Address</label>
                  <p className="mt-1 text-sm text-gray-900">{company.address}</p>
                </div>
              )}

              {company.contactInfo && (
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-gray-700">Contact Information</label>
                  <p className="mt-1 text-sm text-gray-900">{company.contactInfo}</p>
                </div>
              )}

              {company.policyRules && (
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-gray-700">Policy Rules</label>
                  <div className="mt-1 p-3 bg-gray-50 rounded-md">
                    <p className="text-sm text-gray-900 whitespace-pre-wrap">{company.policyRules}</p>
                  </div>
                </div>
              )}

              {company.updatedAt && (
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-gray-700">Last Updated</label>
                  <p className="mt-1 text-sm text-gray-500">
                    {new Date(company.updatedAt).toLocaleString()}
                  </p>
                </div>
              )}
            </div>

            <div className="mt-8 pt-6 border-t border-gray-200">
              <h3 className="text-lg font-semibold mb-4">Quick Actions</h3>
              <div className="flex space-x-4">
                <Link
                  href="/admin/insurance"
                  className="px-4 py-2 text-sm text-primary-600 border border-primary-600 rounded-md hover:bg-primary-50"
                >
                  View All Companies
                </Link>
                <Link
                  href="/admin/insurance/companies/new"
                  className="px-4 py-2 text-sm text-white bg-primary-600 rounded-md hover:bg-primary-700"
                >
                  Add New Company
                </Link>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}

