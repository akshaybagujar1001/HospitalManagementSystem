'use client';

import { useEffect, useState } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Room {
  id: number;
  roomNumber: string;
  roomType: string;
  floor?: string;
  description?: string;
  pricePerDay: number;
}

export default function EditRoomPage() {
  const router = useRouter();
  const params = useParams();
  const queryClient = useQueryClient();
  const { user, isAuthenticated } = useAuthStore();
  const roomId = params?.id as string;

  const { data: room, isLoading } = useQuery<Room>({
    queryKey: ['room', roomId],
    queryFn: async () => {
      const response = await api.get(`/rooms/${roomId}`);
      return response.data;
    },
    enabled: !!roomId && isAuthenticated && user?.role === 'Admin',
  });

  const [formData, setFormData] = useState({
    roomNumber: '',
    roomType: '',
    floor: '',
    description: '',
    pricePerDay: '',
  });

  const [error, setError] = useState('');

  useEffect(() => {
    if (room) {
      setFormData({
        roomNumber: room.roomNumber || '',
        roomType: room.roomType || '',
        floor: room.floor || '',
        description: room.description || '',
        pricePerDay: room.pricePerDay.toString(),
      });
    }
  }, [room]);

  const updateMutation = useMutation({
    mutationFn: async (data: any) => {
      return await api.put(`/rooms/${roomId}`, data);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['rooms'] });
      queryClient.invalidateQueries({ queryKey: ['room', roomId] });
      router.push(`/admin/rooms/${roomId}`);
    },
    onError: (err: any) => {
      setError(err.response?.data?.message || 'Failed to update room. Please try again.');
    },
  });

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  if (!isAuthenticated || user?.role !== 'Admin') {
    return null;
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div>Loading room...</div>
      </div>
    );
  }

  if (!room) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div>Room not found</div>
      </div>
    );
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
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
      roomNumber: formData.roomNumber,
      roomType: formData.roomType,
      floor: formData.floor || null,
      description: formData.description || null,
      pricePerDay: parseFloat(formData.pricePerDay),
    });
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href={`/admin/rooms/${roomId}`} className="text-primary-600 hover:text-primary-700">
                ← Back to Room
              </Link>
              <h1 className="text-xl font-semibold">Edit Room</h1>
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
                  <label className="block text-sm font-medium text-gray-700">Room Number *</label>
                  <input
                    type="text"
                    name="roomNumber"
                    required
                    value={formData.roomNumber}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Room Type *</label>
                  <input
                    type="text"
                    name="roomType"
                    required
                    value={formData.roomType}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Floor</label>
                  <input
                    type="text"
                    name="floor"
                    value={formData.floor}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Price Per Day *</label>
                  <input
                    type="number"
                    name="pricePerDay"
                    required
                    min="0"
                    step="0.01"
                    value={formData.pricePerDay}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>

                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-gray-700">Description</label>
                  <textarea
                    name="description"
                    rows={4}
                    value={formData.description}
                    onChange={handleChange}
                    className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                  />
                </div>
              </div>

              <div className="flex justify-end space-x-4">
                <Link
                  href={`/admin/rooms/${roomId}`}
                  className="px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
                >
                  Cancel
                </Link>
                <button
                  type="submit"
                  disabled={updateMutation.isPending}
                  className="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50"
                >
                  {updateMutation.isPending ? 'Updating...' : 'Update Room'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </main>
    </div>
  );
}

