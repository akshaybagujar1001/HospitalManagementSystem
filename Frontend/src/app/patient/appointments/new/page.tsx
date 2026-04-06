'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface PatientProfile {
  id: number;
  firstName: string;
  lastName: string;
  patientNumber: string;
}

interface Doctor {
  id: number;
  doctorNumber: string;
  firstName: string;
  lastName: string;
  specialization: string;
}

export default function PatientNewAppointmentPage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { user, isAuthenticated, logout } = useAuthStore();
  const [formData, setFormData] = useState({
    doctorId: '',
    appointmentDate: '',
    appointmentTime: '',
    reason: '',
  });
  const [error, setError] = useState('');

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Patient') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: profile, isLoading: profileLoading, error: profileError } = useQuery<PatientProfile>({
    queryKey: ['patient-profile'],
    queryFn: async () => {
      const response = await api.get('/patients/profile');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Patient',
    retry: 1,
  });

  const { data: doctors, isLoading: doctorsLoading } = useQuery<Doctor[]>({
    queryKey: ['available-doctors'],
    queryFn: async () => {
      const response = await api.get('/doctors');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Patient',
  });

  const createMutation = useMutation({
    mutationFn: async (payload: any) => {
      return await api.post('/appointments', payload);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['patient-appointments'] });
      router.push('/patient/dashboard');
    },
    onError: (err: any) => {
      setError(err.response?.data?.message || 'Failed to book appointment. Please try again.');
    },
  });

  if (!isAuthenticated || user?.role !== 'Patient') {
    return null;
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    setError('');
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!profile) {
      setError('Unable to load patient profile.');
      return;
    }

    createMutation.mutate({
      patientId: profile.id,
      doctorId: parseInt(formData.doctorId, 10),
      appointmentDate: formData.appointmentDate,
      appointmentTime: formData.appointmentTime,
      reason: formData.reason || null,
    });
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/patient/dashboard" className="text-primary-600 hover:text-primary-700">
                ← Back to Dashboard
              </Link>
              <h1 className="text-xl font-semibold">Book New Appointment</h1>
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

      <main className="max-w-4xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="bg-white shadow rounded-lg p-6">
            {(error || profileError) && (
              <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
                {error || (profileError as any)?.response?.data?.message || 'Unable to load patient profile.'}
              </div>
            )}

            {profileLoading || doctorsLoading ? (
              <div className="text-center py-12">Loading...</div>
            ) : !profile ? (
              <div className="text-center py-12">
                <p className="text-red-600 mb-4">Unable to load patient profile. Please try refreshing the page.</p>
                <Link
                  href="/patient/dashboard"
                  className="text-primary-600 hover:text-primary-700"
                >
                  ← Back to Dashboard
                </Link>
              </div>
            ) : (
              <form onSubmit={handleSubmit} className="space-y-6">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Select Doctor *</label>
                  <select
                    name="doctorId"
                    required
                    value={formData.doctorId}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  >
                    <option value="">Choose a doctor</option>
                    {doctors?.map((doctor) => (
                      <option key={doctor.id} value={doctor.id}>
                        Dr. {doctor.firstName} {doctor.lastName} ({doctor.specialization})
                      </option>
                    ))}
                  </select>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Date *</label>
                    <input
                      type="date"
                      name="appointmentDate"
                      required
                      value={formData.appointmentDate}
                      onChange={handleChange}
                      className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Time *</label>
                    <input
                      type="time"
                      name="appointmentTime"
                      required
                      value={formData.appointmentTime}
                      onChange={handleChange}
                      className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Reason / Symptoms</label>
                  <textarea
                    name="reason"
                    rows={4}
                    value={formData.reason}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div className="flex justify-end space-x-4">
                  <Link
                    href="/patient/dashboard"
                    className="px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
                  >
                    Cancel
                  </Link>
                  <button
                    type="submit"
                    disabled={createMutation.isPending}
                    className="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50"
                  >
                    {createMutation.isPending ? 'Booking...' : 'Book Appointment'}
                  </button>
                </div>
              </form>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}

