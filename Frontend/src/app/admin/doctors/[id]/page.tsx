'use client';

import { useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Doctor {
  id: number;
  doctorNumber: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  specialization: string;
  licenseNumber?: string;
  bio?: string;
  consultationFee: number;
  isAvailable: boolean;
}

export default function DoctorDetailPage() {
  const router = useRouter();
  const params = useParams();
  const { user, isAuthenticated } = useAuthStore();
  const doctorId = params?.id as string;

  const { data: doctor, isLoading } = useQuery<Doctor>({
    queryKey: ['doctor', doctorId],
    queryFn: async () => {
      const response = await api.get(`/doctors/${doctorId}`);
      return response.data;
    },
    enabled: !!doctorId && isAuthenticated,
  });

  if (!isAuthenticated || user?.role !== 'Admin') {
    router.push('/login');
    return null;
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div>Loading doctor details...</div>
      </div>
    );
  }

  if (!doctor) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div>Doctor not found</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/admin/doctors" className="text-primary-600 hover:text-primary-700">
                ← Back to Doctors
              </Link>
              <h1 className="text-xl font-semibold">Doctor Details</h1>
            </div>
            <div className="flex items-center space-x-4">
              <Link
                href={`/admin/doctors/${doctorId}/edit`}
                className="px-4 py-2 text-sm text-white bg-primary-600 rounded-md hover:bg-primary-700"
              >
                Edit Doctor
              </Link>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-4xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="bg-white shadow rounded-lg p-6">
            <div className="mb-6">
              <h2 className="text-2xl font-bold text-gray-900">
                Dr. {doctor.firstName} {doctor.lastName}
              </h2>
              <p className="text-sm text-gray-500">Doctor Number: {doctor.doctorNumber}</p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700">Email</label>
                <p className="mt-1 text-sm text-gray-900">{doctor.email}</p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Phone Number</label>
                <p className="mt-1 text-sm text-gray-900">{doctor.phoneNumber || '-'}</p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Specialization</label>
                <p className="mt-1 text-sm text-gray-900">{doctor.specialization}</p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Consultation Fee</label>
                <p className="mt-1 text-sm text-gray-900">${doctor.consultationFee.toFixed(2)}</p>
              </div>

              {doctor.licenseNumber && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">License Number</label>
                  <p className="mt-1 text-sm text-gray-900">{doctor.licenseNumber}</p>
                </div>
              )}

              <div>
                <label className="block text-sm font-medium text-gray-700">Status</label>
                <p className="mt-1">
                  <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                    doctor.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                  }`}>
                    {doctor.isAvailable ? 'Available' : 'Unavailable'}
                  </span>
                </p>
              </div>

              {doctor.bio && (
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-gray-700">Bio</label>
                  <p className="mt-1 text-sm text-gray-900">{doctor.bio}</p>
                </div>
              )}
            </div>

            <div className="mt-8 pt-6 border-t border-gray-200">
              <h3 className="text-lg font-semibold mb-4">Quick Actions</h3>
              <div className="flex space-x-4">
                <Link
                  href={`/admin/doctors/${doctorId}/appointments`}
                  className="px-4 py-2 text-sm text-primary-600 border border-primary-600 rounded-md hover:bg-primary-50"
                >
                  View Appointments
                </Link>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}

