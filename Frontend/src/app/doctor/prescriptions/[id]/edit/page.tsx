'use client';

import { useEffect, useState } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
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

export default function EditPrescriptionPage() {
  const router = useRouter();
  const params = useParams();
  const queryClient = useQueryClient();
  const { user, isAuthenticated } = useAuthStore();
  const prescriptionId = params?.id as string;

  const [formData, setFormData] = useState({
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

  const { data: prescription, isLoading } = useQuery<Prescription>({
    queryKey: ['prescription', prescriptionId],
    queryFn: async () => {
      const response = await api.get(`/prescriptions/${prescriptionId}`);
      return response.data;
    },
    enabled: !!prescriptionId && isAuthenticated && user?.role === 'Doctor',
  });

  useEffect(() => {
    if (prescription) {
      setFormData({
        medicationName: prescription.medicationName || '',
        dosage: prescription.dosage || '',
        instructions: prescription.instructions || '',
        frequency: prescription.frequency || '',
        quantity: prescription.quantity?.toString() || '',
        durationDays: prescription.durationDays?.toString() || '',
      });
    }
  }, [prescription]);

  const updateMutation = useMutation({
    mutationFn: async (payload: any) => {
      return await api.put(`/prescriptions/${prescriptionId}`, payload);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['prescription', prescriptionId] });
      queryClient.invalidateQueries({ queryKey: ['doctor-prescriptions'] });
      router.push('/doctor/prescriptions');
    },
    onError: (err: any) => {
      setError(err.response?.data?.message || 'Failed to update prescription. Please try again.');
    },
  });

  if (!isAuthenticated || user?.role !== 'Doctor') {
    return null;
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">Loading prescription...</div>
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

    updateMutation.mutate({
      patientId: prescription.patientId,
      doctorId: prescription.doctorId,
      appointmentId: prescription.appointmentId || null,
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
              <h1 className="text-xl font-semibold">Edit Prescription</h1>
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

                <div className="bg-gray-50 p-4 rounded-md">
                  <p className="text-sm text-gray-600">
                    <strong>Prescribed Date:</strong>{' '}
                    {new Date(prescription.prescribedDate).toLocaleDateString()}
                  </p>
                  {prescription.expiryDate && (
                    <p className="text-sm text-gray-600 mt-1">
                      <strong>Expiry Date:</strong>{' '}
                      {new Date(prescription.expiryDate).toLocaleDateString()}
                    </p>
                  )}
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
                  disabled={updateMutation.isPending}
                  className="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50"
                >
                  {updateMutation.isPending ? 'Updating...' : 'Update Prescription'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </main>
    </div>
  );
}

