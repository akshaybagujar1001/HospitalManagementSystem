'use client';

import { useEffect, useState } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Appointment {
  id: number;
  patientId: number;
  patientName: string;
  doctorId: number;
  doctorName: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
  reason?: string;
  notes?: string;
  createdAt: string;
}

export default function AppointmentDetailPage() {
  const router = useRouter();
  const params = useParams();
  const { user, isAuthenticated, logout } = useAuthStore();
  const appointmentId = params?.id as string;
  const [error, setError] = useState<string>('');

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Doctor') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: appointment, isLoading } = useQuery<Appointment>({
    queryKey: ['appointment', appointmentId],
    queryFn: async () => {
      const response = await api.get(`/appointments/${appointmentId}`);
      return response.data;
    },
    enabled: !!appointmentId && isAuthenticated && user?.role === 'Doctor',
  });

  const handleComplete = async () => {
    try {
      setError('');
      const response = await api.put(`/appointments/${appointmentId}/complete`);
      // Invalidate queries to refresh the appointment list
      router.push('/doctor/appointments');
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || 
                          err.response?.data?.details || 
                          err.message || 
                          'Failed to complete appointment. Please check the backend logs for details.';
      setError(errorMessage);
      console.error('Error completing appointment:', err.response?.data || err);
    }
  };

  const handleCancel = async () => {
    try {
      setError('');
      const response = await api.put(`/appointments/${appointmentId}/cancel`);
      // Invalidate queries to refresh the appointment list
      router.push('/doctor/appointments');
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || 
                          err.response?.data?.details || 
                          err.message || 
                          'Failed to cancel appointment. Please check the backend logs for details.';
      setError(errorMessage);
      console.error('Error cancelling appointment:', err.response?.data || err);
    }
  };

  if (!isAuthenticated || user?.role !== 'Doctor') {
    return null;
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">Loading appointment details...</div>
      </div>
    );
  }

  if (!appointment) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <h1 className="text-2xl font-bold mb-4">Appointment Not Found</h1>
          <Link href="/doctor/appointments" className="text-primary-600 hover:text-primary-700">
            ← Back to Appointments
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
              <Link href="/doctor/appointments" className="text-primary-600 hover:text-primary-700">
                ← Back to Appointments
              </Link>
              <h1 className="text-xl font-semibold">Appointment Details</h1>
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
          {error && (
            <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
              {error}
            </div>
          )}

          <div className="bg-white shadow overflow-hidden sm:rounded-lg">
            <div className="px-4 py-5 sm:px-6">
              <div className="flex justify-between items-center">
                <h3 className="text-lg leading-6 font-medium text-gray-900">
                  Appointment #{appointment.id}
                </h3>
                <span className={`px-3 py-1 inline-flex text-sm leading-5 font-semibold rounded-full ${
                  appointment.status === 'Scheduled' ? 'bg-green-100 text-green-800' :
                  appointment.status === 'Completed' ? 'bg-blue-100 text-blue-800' :
                  appointment.status === 'Cancelled' ? 'bg-red-100 text-red-800' :
                  'bg-gray-100 text-gray-800'
                }`}>
                  {appointment.status}
                </span>
              </div>
            </div>
            <div className="border-t border-gray-200">
              <dl>
                <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Patient</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {appointment.patientName}
                  </dd>
                </div>
                <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Date</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {new Date(appointment.appointmentDate).toLocaleDateString()}
                  </dd>
                </div>
                <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Time</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {appointment.appointmentTime}
                  </dd>
                </div>
                {appointment.reason && (
                  <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                    <dt className="text-sm font-medium text-gray-500">Reason</dt>
                    <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                      {appointment.reason}
                    </dd>
                  </div>
                )}
                {appointment.notes && (
                  <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                    <dt className="text-sm font-medium text-gray-500">Notes</dt>
                    <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                      {appointment.notes}
                    </dd>
                  </div>
                )}
                <div className="bg-white px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4 sm:px-6">
                  <dt className="text-sm font-medium text-gray-500">Created At</dt>
                  <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                    {new Date(appointment.createdAt).toLocaleString()}
                  </dd>
                </div>
              </dl>
            </div>
            {appointment.status === 'Scheduled' && (
              <div className="px-4 py-4 sm:px-6 border-t border-gray-200 flex space-x-3">
                <button
                  onClick={handleComplete}
                  className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
                >
                  Mark as Completed
                </button>
                <button
                  onClick={handleCancel}
                  className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
                >
                  Cancel Appointment
                </button>
                <Link
                  href={`/doctor/prescriptions/add?appointmentId=${appointment.id}&patientId=${appointment.patientId}`}
                  className="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                >
                  Create Prescription
                </Link>
              </div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}

