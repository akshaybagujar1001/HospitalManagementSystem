'use client';

import { useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Prescription {
  id: number;
  patientId: number;
  doctorId: number;
  appointmentId?: number;
  medicationName: string;
  dosage?: string;
  instructions?: string;
  frequency?: string;
  quantity?: number;
  durationDays?: number;
  prescribedDate: string;
  expiryDate?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export default function PrescriptionDetailPage() {
  const router = useRouter();
  const params = useParams();
  const { user, isAuthenticated, logout } = useAuthStore();
  const prescriptionId = params?.id as string;

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Doctor') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: prescription, isLoading } = useQuery<Prescription>({
    queryKey: ['prescription', prescriptionId],
    queryFn: async () => {
      const response = await api.get(`/prescriptions/${prescriptionId}`);
      return response.data;
    },
    enabled: !!prescriptionId && isAuthenticated && user?.role === 'Doctor',
  });

  if (!isAuthenticated || user?.role !== 'Doctor') {
    return null;
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">Loading prescription details...</div>
      </div>
    );
  }

  if (!prescription) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h1 className="text-2xl font-bold mb-4">Prescription Not Found</h1>
          <Link href="/doctor/prescriptions" className="text-primary-600 hover:text-primary-700">
            ← Back to Prescriptions
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
              <Link href="/doctor/prescriptions" className="text-primary-600 hover:text-primary-700">
                ← Back to Prescriptions
              </Link>
              <h1 className="text-xl font-semibold">Prescription Details</h1>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-sm text-gray-700">
                Dr. {user?.firstName} {user?.lastName}
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

      <main className="max-w-4xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="bg-white shadow overflow-hidden sm:rounded-lg">
            <div className="px-4 py-5 sm:px-6">
              <div className="flex justify-between items-center">
                <h3 className="text-lg leading-6 font-medium text-gray-900">
                  Prescription #{prescription.id}
                </h3>
                <span className={`px-3 py-1 inline-flex text-sm leading-5 font-semibold rounded-full ${
                  prescription.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                }`}>
                  {prescription.isActive ? 'Active' : 'Inactive'}
                </span>
              </div>
            </div>
            <div className="border-t border-gray-200">
              <dl>
                <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Medication Name</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {prescription.medicationName}
                  </dd>
                </div>
                {prescription.dosage && (
                  <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                    <dt className="text-sm font-medium text-gray-500">Dosage</dt>
                    <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                      {prescription.dosage}
                    </dd>
                  </div>
                )}
                {prescription.instructions && (
                  <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                    <dt className="text-sm font-medium text-gray-500">Instructions</dt>
                    <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                      {prescription.instructions}
                    </dd>
                  </div>
                )}
                {prescription.frequency && (
                  <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                    <dt className="text-sm font-medium text-gray-500">Frequency</dt>
                    <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                      {prescription.frequency}
                    </dd>
                  </div>
                )}
                {prescription.quantity && (
                  <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                    <dt className="text-sm font-medium text-gray-500">Quantity</dt>
                    <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                      {prescription.quantity}
                    </dd>
                  </div>
                )}
                {prescription.durationDays && (
                  <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                    <dt className="text-sm font-medium text-gray-500">Duration (days)</dt>
                    <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                      {prescription.durationDays}
                    </dd>
                  </div>
                )}
                <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Prescribed Date</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {new Date(prescription.prescribedDate).toLocaleString()}
                  </dd>
                </div>
                {prescription.expiryDate && (
                  <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                    <dt className="text-sm font-medium text-gray-500">Expiry Date</dt>
                    <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                      {new Date(prescription.expiryDate).toLocaleString()}
                    </dd>
                  </div>
                )}
                <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Created At</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {new Date(prescription.createdAt).toLocaleString()}
                  </dd>
                </div>
              </dl>
            </div>
            {prescription.isActive && (
              <div className="px-4 py-4 sm:px-6 border-t border-gray-200">
                <Link
                  href={`/doctor/prescriptions/${prescription.id}/edit`}
                  className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
                >
                  Edit Prescription
                </Link>
              </div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}

