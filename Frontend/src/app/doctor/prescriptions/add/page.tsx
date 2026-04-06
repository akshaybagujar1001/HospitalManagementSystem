'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Patient {
  id: number;
  firstName: string;
  lastName: string;
  patientNumber: string;
}

interface Doctor {
  id: number;
  userId: number;
  firstName: string;
  lastName: string;
}

interface Appointment {
  id: number;
  patientName: string;
  appointmentDate: string;
}

export default function AddPrescriptionPage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { user, isAuthenticated } = useAuthStore();

  const [formData, setFormData] = useState({
    patientId: '',
    appointmentId: '',
    medicationName: '',
    dosage: '',
    instructions: '',
    frequency: '',
    quantity: '',
    durationDays: '',
  });

  const [error, setError] = useState('');

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Doctor') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: doctorRecord } = useQuery<Doctor | undefined>({
    queryKey: ['current-doctor', user?.userId],
    queryFn: async () => {
      const response = await api.get('/doctors');
      const doctors: Doctor[] = response.data;
      return doctors.find((doctor) => doctor.userId === user?.userId);
    },
    enabled: isAuthenticated && user?.role === 'Doctor' && !!user?.userId,
  });

  const { data: patients } = useQuery<Patient[]>({
    queryKey: ['patients-for-prescription'],
    queryFn: async () => {
      const response = await api.get('/patients');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Doctor',
  });

  const { data: appointments } = useQuery<Appointment[]>({
    queryKey: ['doctor-prescription-appointments'],
    queryFn: async () => {
      const response = await api.get('/appointments');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Doctor',
  });

  const createMutation = useMutation({
    mutationFn: async (payload: any) => {
      return await api.post('/prescriptions', payload);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['doctor-prescriptions'] });
      router.push('/doctor/prescriptions');
    },
    onError: (err: any) => {
      setError(err.response?.data?.message || 'Failed to create prescription. Please try again.');
    },
  });

  if (!isAuthenticated || user?.role !== 'Doctor') {
    return null;
  }

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>,
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    setError('');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!doctorRecord) {
      setError('Doctor profile not found. Please contact administrator.');
      return;
    }

    createMutation.mutate({
      patientId: parseInt(formData.patientId, 10),
      doctorId: doctorRecord.id,
      appointmentId: formData.appointmentId ? parseInt(formData.appointmentId, 10) : null,
      medicationName: formData.medicationName,
      dosage: formData.dosage || null,
      instructions: formData.instructions || null,
      frequency: formData.frequency || null,
      quantity: formData.quantity ? parseInt(formData.quantity, 10) : null,
      durationDays: formData.durationDays ? parseInt(formData.durationDays, 10) : null,
    });
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/doctor/prescriptions" className="text-primary-600 hover:text-primary-700">
                ← Back to Prescriptions
              </Link>
              <h1 className="text-xl font-semibold">Create Prescription</h1>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-3xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="bg-white shadow rounded-lg p-6">
            {error && (
              <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
                {error}
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="grid grid-cols-1 gap-6">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Patient *</label>
                  <select
                    name="patientId"
                    required
                    value={formData.patientId}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  >
                    <option value="">Select Patient</option>
                    {patients?.map((patient) => (
                      <option key={patient.id} value={patient.id}>
                        {patient.patientNumber} - {patient.firstName} {patient.lastName}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Related Appointment</label>
                  <select
                    name="appointmentId"
                    value={formData.appointmentId}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  >
                    <option value="">Optional</option>
                    {appointments?.map((appointment) => (
                      <option key={appointment.id} value={appointment.id}>
                        #{appointment.id} • {appointment.patientName} •{' '}
                        {new Date(appointment.appointmentDate).toLocaleDateString()}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Medication Name *</label>
                  <input
                    type="text"
                    name="medicationName"
                    required
                    value={formData.medicationName}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Dosage</label>
                  <input
                    type="text"
                    name="dosage"
                    value={formData.dosage}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Instructions</label>
                  <textarea
                    name="instructions"
                    rows={3}
                    value={formData.instructions}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Frequency</label>
                    <input
                      type="text"
                      name="frequency"
                      value={formData.frequency}
                      onChange={handleChange}
                      className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700">Quantity</label>
                    <input
                      type="number"
                      name="quantity"
                      min="0"
                      value={formData.quantity}
                      onChange={handleChange}
                      className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Duration (days)</label>
                  <input
                    type="number"
                    name="durationDays"
                    min="0"
                    value={formData.durationDays}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>
              </div>

              <div className="flex justify-end space-x-4">
                <Link
                  href="/doctor/prescriptions"
                  className="px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
                >
                  Cancel
                </Link>
                <button
                  type="submit"
                  disabled={createMutation.isPending}
                  className="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50"
                >
                  {createMutation.isPending ? 'Creating...' : 'Create Prescription'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </main>
    </div>
  );
}

