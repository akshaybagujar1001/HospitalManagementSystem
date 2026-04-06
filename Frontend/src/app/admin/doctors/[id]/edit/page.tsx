'use client';

import { useEffect, useState } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
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

export default function EditDoctorPage() {
  const router = useRouter();
  const params = useParams();
  const queryClient = useQueryClient();
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

  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    specialization: '',
    licenseNumber: '',
    bio: '',
    consultationFee: '',
    isAvailable: true,
  });

  const [error, setError] = useState('');

  useEffect(() => {
    if (doctor) {
      setFormData({
        firstName: doctor.firstName || '',
        lastName: doctor.lastName || '',
        email: doctor.email || '',
        phoneNumber: doctor.phoneNumber || '',
        specialization: doctor.specialization || '',
        licenseNumber: doctor.licenseNumber || '',
        bio: doctor.bio || '',
        consultationFee: doctor.consultationFee.toString() || '',
        isAvailable: doctor.isAvailable,
      });
    }
  }, [doctor]);

  const updateMutation = useMutation({
    mutationFn: async (data: any) => {
      return await api.put(`/doctors/${doctorId}`, data);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['doctor', doctorId] });
      queryClient.invalidateQueries({ queryKey: ['doctors'] });
      router.push(`/admin/doctors/${doctorId}`);
    },
    onError: (err: any) => {
      setError(err.response?.data?.message || 'Failed to update doctor. Please try again.');
    },
  });

  if (!isAuthenticated || user?.role !== 'Admin') {
    router.push('/login');
    return null;
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div>Loading...</div>
      </div>
    );
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value, type } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? (e.target as HTMLInputElement).checked : value,
    }));
    setError('');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    updateMutation.mutate({
      firstName: formData.firstName,
      lastName: formData.lastName,
      email: formData.email,
      phoneNumber: formData.phoneNumber,
      specialization: formData.specialization,
      licenseNumber: formData.licenseNumber || null,
      bio: formData.bio || null,
      consultationFee: parseFloat(formData.consultationFee),
      isAvailable: formData.isAvailable,
    });
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href={`/admin/doctors/${doctorId}`} className="text-primary-600 hover:text-primary-700">
                ← Back to Doctor
              </Link>
              <h1 className="text-xl font-semibold">Edit Doctor</h1>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-4xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="bg-white shadow rounded-lg p-6">
            {error && (
              <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
                {error}
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <label className="block text-sm font-medium text-gray-700">First Name *</label>
                  <input
                    type="text"
                    name="firstName"
                    required
                    value={formData.firstName}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Last Name *</label>
                  <input
                    type="text"
                    name="lastName"
                    required
                    value={formData.lastName}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Email *</label>
                  <input
                    type="email"
                    name="email"
                    required
                    value={formData.email}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Phone Number</label>
                  <input
                    type="tel"
                    name="phoneNumber"
                    value={formData.phoneNumber}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Specialization *</label>
                  <input
                    type="text"
                    name="specialization"
                    required
                    value={formData.specialization}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">License Number</label>
                  <input
                    type="text"
                    name="licenseNumber"
                    value={formData.licenseNumber}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Consultation Fee *</label>
                  <input
                    type="number"
                    name="consultationFee"
                    required
                    step="0.01"
                    min="0"
                    value={formData.consultationFee}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Status</label>
                  <div className="mt-2">
                    <label className="inline-flex items-center">
                      <input
                        type="checkbox"
                        name="isAvailable"
                        checked={formData.isAvailable}
                        onChange={handleChange}
                        className="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                      />
                      <span className="ml-2 text-sm text-gray-700">Available</span>
                    </label>
                  </div>
                </div>

                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-gray-700">Bio</label>
                  <textarea
                    name="bio"
                    value={formData.bio}
                    onChange={handleChange}
                    rows={4}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>
              </div>

              <div className="flex justify-end space-x-4">
                <Link
                  href={`/admin/doctors/${doctorId}`}
                  className="px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
                >
                  Cancel
                </Link>
                <button
                  type="submit"
                  disabled={updateMutation.isPending}
                  className="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50"
                >
                  {updateMutation.isPending ? 'Updating...' : 'Update Doctor'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </main>
    </div>
  );
}

