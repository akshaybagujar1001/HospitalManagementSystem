'use client';

import { useEffect, useState } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Patient {
  id: number;
  patientNumber: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  gender: string;
  address?: string;
  bloodGroup?: string;
  allergies?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
}

export default function PatientDetailPage() {
  const router = useRouter();
  const params = useParams();
  const { user, isAuthenticated } = useAuthStore();
  const patientId = params?.id as string;

  const { data: patient, isLoading } = useQuery<Patient>({
    queryKey: ['patient', patientId],
    queryFn: async () => {
      const response = await api.get(`/patients/${patientId}`);
      return response.data;
    },
    enabled: !!patientId && isAuthenticated,
  });

  if (!isAuthenticated || user?.role !== 'Admin') {
    router.push('/login');
    return null;
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div>Loading patient details...</div>
      </div>
    );
  }

  if (!patient) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div>Patient not found</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/admin/patients" className="text-primary-600 hover:text-primary-700">
                ← Back to Patients
              </Link>
              <h1 className="text-xl font-semibold">Patient Details</h1>
            </div>
            <div className="flex items-center space-x-4">
              <Link
                href={`/admin/patients/${patientId}/edit`}
                className="px-4 py-2 text-sm text-white bg-primary-600 rounded-md hover:bg-primary-700"
              >
                Edit Patient
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
                {patient.firstName} {patient.lastName}
              </h2>
              <p className="text-sm text-gray-500">Patient Number: {patient.patientNumber}</p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700">Email</label>
                <p className="mt-1 text-sm text-gray-900">{patient.email}</p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Phone Number</label>
                <p className="mt-1 text-sm text-gray-900">{patient.phoneNumber}</p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Date of Birth</label>
                <p className="mt-1 text-sm text-gray-900">
                  {new Date(patient.dateOfBirth).toLocaleDateString()}
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Gender</label>
                <p className="mt-1 text-sm text-gray-900">{patient.gender}</p>
              </div>

              {patient.bloodGroup && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Blood Group</label>
                  <p className="mt-1 text-sm text-gray-900">{patient.bloodGroup}</p>
                </div>
              )}

              {patient.address && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Address</label>
                  <p className="mt-1 text-sm text-gray-900">{patient.address}</p>
                </div>
              )}

              {patient.allergies && (
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-gray-700">Allergies</label>
                  <p className="mt-1 text-sm text-gray-900">{patient.allergies}</p>
                </div>
              )}

              {patient.emergencyContactName && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Emergency Contact Name</label>
                  <p className="mt-1 text-sm text-gray-900">{patient.emergencyContactName}</p>
                </div>
              )}

              {patient.emergencyContactPhone && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Emergency Contact Phone</label>
                  <p className="mt-1 text-sm text-gray-900">{patient.emergencyContactPhone}</p>
                </div>
              )}
            </div>

            <div className="mt-8 pt-6 border-t border-gray-200">
              <h3 className="text-lg font-semibold mb-4">Quick Actions</h3>
              <div className="flex space-x-4">
                <Link
                  href={`/admin/patients/${patientId}/appointments`}
                  className="px-4 py-2 text-sm text-primary-600 border border-primary-600 rounded-md hover:bg-primary-50"
                >
                  View Appointments
                </Link>
                <Link
                  href={`/admin/patients/${patientId}/medical-records`}
                  className="px-4 py-2 text-sm text-primary-600 border border-primary-600 rounded-md hover:bg-primary-50"
                >
                  View Medical Records
                </Link>
                <Link
                  href={`/admin/patients/${patientId}/invoices`}
                  className="px-4 py-2 text-sm text-primary-600 border border-primary-600 rounded-md hover:bg-primary-50"
                >
                  View Invoices
                </Link>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}







