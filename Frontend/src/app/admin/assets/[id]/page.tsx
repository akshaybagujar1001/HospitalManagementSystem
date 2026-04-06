'use client';

import { useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Asset {
  id: number;
  name: string;
  type: string;
  serialNumber?: string;
  manufacturer?: string;
  model?: string;
  location?: string;
  status: string;
  purchasePrice?: number;
  purchaseDate?: string;
  lastMaintenanceDate?: string;
  nextMaintenanceDate?: string;
  notes?: string;
}

export default function AssetDetailPage() {
  const router = useRouter();
  const params = useParams();
  const { user, isAuthenticated, logout } = useAuthStore();
  const assetId = params?.id as string;

  const { data: asset, isLoading, error } = useQuery<Asset>({
    queryKey: ['asset', assetId],
    queryFn: async () => {
      const response = await api.get(`/assets/${assetId}`);
      return response.data;
    },
    enabled: !!assetId && isAuthenticated,
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
        <div>Loading asset details...</div>
      </div>
    );
  }

  if (error || !asset) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Asset Not Found</h2>
          <p className="text-gray-600 mb-4">The asset you're looking for doesn't exist.</p>
          <Link
            href="/admin/assets"
            className="px-4 py-2 text-sm text-white bg-primary-600 rounded-md hover:bg-primary-700"
          >
            Back to Assets
          </Link>
        </div>
      </div>
    );
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Available':
        return 'bg-green-100 text-green-800';
      case 'In Use':
        return 'bg-blue-100 text-blue-800';
      case 'Maintenance':
        return 'bg-yellow-100 text-yellow-800';
      case 'Retired':
        return 'bg-gray-100 text-gray-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const isMaintenanceDue = asset.nextMaintenanceDate && 
    new Date(asset.nextMaintenanceDate) <= new Date(Date.now() + 7 * 24 * 60 * 60 * 1000);

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/admin/assets" className="text-primary-600 hover:text-primary-700">
                ← Back to Assets
              </Link>
              <h1 className="text-xl font-semibold">Asset Details</h1>
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
          {isMaintenanceDue && (
            <div className="mb-4 bg-yellow-50 border border-yellow-200 rounded-lg p-4">
              <h3 className="text-sm font-medium text-yellow-800">
                ⚠️ Maintenance Due Soon: {asset.nextMaintenanceDate && new Date(asset.nextMaintenanceDate).toLocaleDateString()}
              </h3>
            </div>
          )}

          <div className="bg-white shadow rounded-lg p-6">
            <div className="mb-6 flex justify-between items-start">
              <div>
                <h2 className="text-2xl font-bold text-gray-900">{asset.name}</h2>
                <p className="text-sm text-gray-500 mt-1">Type: {asset.type}</p>
              </div>
              <span className={`px-3 py-1 text-xs font-semibold rounded-full ${getStatusColor(asset.status)}`}>
                {asset.status}
              </span>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              {asset.serialNumber && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Serial Number</label>
                  <p className="mt-1 text-sm text-gray-900">{asset.serialNumber}</p>
                </div>
              )}

              {asset.manufacturer && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Manufacturer</label>
                  <p className="mt-1 text-sm text-gray-900">{asset.manufacturer}</p>
                </div>
              )}

              {asset.model && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Model</label>
                  <p className="mt-1 text-sm text-gray-900">{asset.model}</p>
                </div>
              )}

              {asset.location && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Location</label>
                  <p className="mt-1 text-sm text-gray-900">{asset.location}</p>
                </div>
              )}

              {asset.purchasePrice && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Purchase Price</label>
                  <p className="mt-1 text-sm text-gray-900">
                    ${asset.purchasePrice.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                  </p>
                </div>
              )}

              {asset.purchaseDate && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Purchase Date</label>
                  <p className="mt-1 text-sm text-gray-900">
                    {new Date(asset.purchaseDate).toLocaleDateString()}
                  </p>
                </div>
              )}

              {asset.lastMaintenanceDate && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Last Maintenance</label>
                  <p className="mt-1 text-sm text-gray-900">
                    {new Date(asset.lastMaintenanceDate).toLocaleDateString()}
                  </p>
                </div>
              )}

              {asset.nextMaintenanceDate && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Next Maintenance</label>
                  <p className={`mt-1 text-sm ${isMaintenanceDue ? 'text-yellow-600 font-semibold' : 'text-gray-900'}`}>
                    {new Date(asset.nextMaintenanceDate).toLocaleDateString()}
                    {isMaintenanceDue && ' (Due Soon)'}
                  </p>
                </div>
              )}

              {asset.notes && (
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-gray-700">Notes</label>
                  <div className="mt-1 p-3 bg-gray-50 rounded-md">
                    <p className="text-sm text-gray-900 whitespace-pre-wrap">{asset.notes}</p>
                  </div>
                </div>
              )}
            </div>

            <div className="mt-8 pt-6 border-t border-gray-200">
              <h3 className="text-lg font-semibold mb-4">Quick Actions</h3>
              <div className="flex space-x-4">
                <Link
                  href="/admin/assets"
                  className="px-4 py-2 text-sm text-primary-600 border border-primary-600 rounded-md hover:bg-primary-50"
                >
                  View All Assets
                </Link>
                <Link
                  href="/admin/assets/new"
                  className="px-4 py-2 text-sm text-white bg-primary-600 rounded-md hover:bg-primary-700"
                >
                  Add New Asset
                </Link>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}

