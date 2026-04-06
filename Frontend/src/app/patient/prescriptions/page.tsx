'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Prescription {
  id: number;
  doctorId: number;
  medicationName: string;
  dosage?: string;
  instructions?: string;
  frequency?: string;
  quantity?: number;
  prescribedDate: string;
  isActive: boolean;
}

export default function PatientPrescriptionsPage() {
  const router = useRouter();
  const { user, isAuthenticated, logout } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Patient') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: prescriptions, isLoading } = useQuery<Prescription[]>({
    queryKey: ['patient-prescriptions'],
    queryFn: async () => {
      const response = await api.get('/prescriptions');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Patient',
  });

  if (!isAuthenticated || user?.role !== 'Patient') {
    return null;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/patient/dashboard" className="text-primary-600 hover:text-primary-700">
                ← Back to Dashboard
              </Link>
              <h1 className="text-xl font-semibold">My Prescriptions</h1>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-sm text-gray-700">
                {user?.firstName} {user?.lastName}
              </span>
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

      <main className="max-w-6xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          {isLoading ? (
            <div className="text-center py-12">Loading prescriptions...</div>
          ) : prescriptions && prescriptions.length > 0 ? (
            <div className="bg-white shadow overflow-hidden sm:rounded-md">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                      Medication
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                      Dosage & Frequency
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                      Instructions
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                      Prescribed Date
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                      Status
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {prescriptions.map((prescription) => (
                    <tr key={prescription.id}>
                      <td className="px-6 py-4 text-sm font-medium text-gray-900">
                        {prescription.medicationName}
                      </td>
                      <td className="px-6 py-4 text-sm text-gray-500">
                        {prescription.dosage || '-'}
                        {prescription.frequency && (
                          <span className="block text-xs text-gray-400">{prescription.frequency}</span>
                        )}
                      </td>
                      <td className="px-6 py-4 text-sm text-gray-500">{prescription.instructions || '-'}</td>
                      <td className="px-6 py-4 text-sm text-gray-500">
                        {new Date(prescription.prescribedDate).toLocaleDateString()}
                      </td>
                      <td className="px-6 py-4">
                        <span
                          className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                            prescription.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                          }`}
                        >
                          {prescription.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <div className="bg-white shadow rounded-lg p-12 text-center text-gray-500">
              No prescriptions found.
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

