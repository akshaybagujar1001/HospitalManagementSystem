'use client';

import { useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Appointment {
  id: number;
  patientId: number;
  doctorId: number;
  appointmentDate: string;
  status: string;
  notes?: string;
  createdAt: string;
}

export default function AppointmentDetailPage() {
  const router = useRouter();
  const params = useParams();
  const { user, isAuthenticated, logout } = useAuthStore();
  const appointmentId = params?.id as string;

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: appointment, isLoading } = useQuery<Appointment>({
    queryKey: ['appointment', appointmentId],
    queryFn: async () => {
      const response = await api.get(`/appointments/${appointmentId}`);
      return response.data;
    },
    enabled: !!appointmentId && isAuthenticated && user?.role === 'Admin',
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
              <Link href="/admin/appointments" className="text-primary-600 hover:text-primary-700">
                ← Back to Appointments
              </Link>
              <h1 className="text-xl font-semibold">Appointment Details</h1>
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

      <main className="max-w-4xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          {isLoading ? (
            <div className="text-center py-12">Loading...</div>
          ) : appointment ? (
            <div className="bg-white shadow rounded-lg p-6">
              <div className="space-y-4">
                <div>
                  <label className="text-sm font-medium text-gray-500">Appointment ID</label>
                  <p className="mt-1 text-lg text-gray-900">#{appointment.id}</p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-500">Date</label>
                  <p className="mt-1 text-lg text-gray-900">
                    {new Date(appointment.appointmentDate).toLocaleString()}
                  </p>
                </div>
                <div>
                  <label className="text-sm font-medium text-gray-500">Status</label>
                  <p className="mt-1">
                    <span className={`px-2 py-1 text-xs font-semibold rounded-full ${
                      appointment.status === 'Completed' ? 'bg-green-100 text-green-800' :
                      appointment.status === 'Cancelled' ? 'bg-red-100 text-red-800' :
                      'bg-yellow-100 text-yellow-800'
                    }`}>
                      {appointment.status}
                    </span>
                  </p>
                </div>
                {appointment.notes && (
                  <div>
                    <label className="text-sm font-medium text-gray-500">Notes</label>
                    <p className="mt-1 text-gray-900">{appointment.notes}</p>
                  </div>
                )}
                <div className="flex space-x-4 pt-4">
                  <Link
                    href={`/admin/patients/${appointment.patientId}`}
                    className="px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700"
                  >
                    View Patient
                  </Link>
                  <Link
                    href={`/admin/doctors/${appointment.doctorId}`}
                    className="px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700"
                  >
                    View Doctor
                  </Link>
                </div>
              </div>
            </div>
          ) : (
            <div className="bg-white shadow rounded-lg p-12 text-center text-gray-500">
              Appointment not found.
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

