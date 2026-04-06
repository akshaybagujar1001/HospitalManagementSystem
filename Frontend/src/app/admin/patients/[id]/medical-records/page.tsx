'use client';

import { useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface MedicalRecord {
  id: number;
  diagnosis: string;
  symptoms?: string;
  treatment?: string;
  notes?: string;
  recordDate: string;
  doctorId: number;
  appointmentId?: number;
}

export default function PatientMedicalRecordsPage() {
  const router = useRouter();
  const params = useParams();
  const { user, isAuthenticated, logout } = useAuthStore();
  const patientId = params?.id as string;

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: medicalRecords, isLoading } = useQuery<MedicalRecord[]>({
    queryKey: ['patient-medical-records', patientId],
    queryFn: async () => {
      const response = await api.get(`/patients/${patientId}/medical-records`);
      return response.data;
    },
    enabled: !!patientId && isAuthenticated && user?.role === 'Admin',
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
              <Link href={`/admin/patients/${patientId}`} className="text-primary-600 hover:text-primary-700">
                ← Back to Patient Details
              </Link>
              <h1 className="text-xl font-semibold">Medical Records</h1>
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

      <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <h2 className="text-2xl font-bold mb-6">Medical Records</h2>

          {isLoading ? (
            <div className="text-center py-12">Loading medical records...</div>
          ) : (
            <div className="space-y-4">
              {medicalRecords && medicalRecords.length > 0 ? (
                medicalRecords.map((record) => (
                  <div key={record.id} className="bg-white shadow rounded-lg p-6">
                    <div className="flex justify-between items-start mb-4">
                      <div>
                        <h3 className="text-lg font-semibold text-gray-900">{record.diagnosis}</h3>
                        <p className="text-sm text-gray-500">
                          {new Date(record.recordDate).toLocaleDateString()}
                        </p>
                      </div>
                    </div>
                    
                    <div className="space-y-3">
                      {record.symptoms && (
                        <div>
                          <label className="block text-sm font-medium text-gray-700">Symptoms</label>
                          <p className="mt-1 text-sm text-gray-900">{record.symptoms}</p>
                        </div>
                      )}
                      
                      {record.treatment && (
                        <div>
                          <label className="block text-sm font-medium text-gray-700">Treatment</label>
                          <p className="mt-1 text-sm text-gray-900">{record.treatment}</p>
                        </div>
                      )}
                      
                      {record.notes && (
                        <div>
                          <label className="block text-sm font-medium text-gray-700">Notes</label>
                          <p className="mt-1 text-sm text-gray-900">{record.notes}</p>
                        </div>
                      )}
                    </div>
                  </div>
                ))
              ) : (
                <div className="bg-white shadow rounded-lg p-12 text-center text-gray-500">
                  No medical records found
                </div>
              )}
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

