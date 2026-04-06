'use client';

import { useEffect } from 'react';
import { useRouter, useParams } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Room {
  id: number;
  roomNumber: string;
  roomType: string;
  floor?: string;
  description?: string;
  pricePerDay: number;
  isAvailable: boolean;
}

interface Bed {
  id: number;
  bedNumber: string;
  description?: string;
  isAvailable: boolean;
  isOccupied: boolean;
}

export default function RoomDetailPage() {
  const router = useRouter();
  const params = useParams();
  const { user, isAuthenticated, logout } = useAuthStore();
  const roomId = params?.id as string;

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: room, isLoading } = useQuery<Room>({
    queryKey: ['room', roomId],
    queryFn: async () => {
      const response = await api.get(`/rooms/${roomId}`);
      return response.data;
    },
    enabled: !!roomId && isAuthenticated && user?.role === 'Admin',
  });

  const { data: beds, isLoading: bedsLoading } = useQuery<Bed[]>({
    queryKey: ['room-beds', roomId],
    queryFn: async () => {
      const response = await api.get(`/rooms/${roomId}/beds`);
      return response.data;
    },
    enabled: !!roomId && isAuthenticated && user?.role === 'Admin',
  });

  if (!isAuthenticated || user?.role !== 'Admin') {
    return null;
  }

  if (isLoading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div>Loading room details...</div>
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

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/admin/rooms" className="text-primary-600 hover:text-primary-700">
                ← Back to Rooms
              </Link>
              <h1 className="text-xl font-semibold">Room Details</h1>
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

      <main className="max-w-5xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="bg-white shadow rounded-lg p-6 mb-6">
            <div className="flex justify-between items-start">
              <div>
                <h2 className="text-2xl font-bold text-gray-900">Room {room.roomNumber}</h2>
                <p className="text-sm text-gray-500">{room.roomType}</p>
              </div>
              <span
                className={`px-2 py-1 text-xs font-semibold rounded-full ${
                  room.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                }`}
              >
                {room.isAvailable ? 'Available' : 'Occupied'}
              </span>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mt-6">
              <div>
                <label className="block text-sm font-medium text-gray-700">Room Number</label>
                <p className="mt-1 text-sm text-gray-900">{room.roomNumber}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Room Type</label>
                <p className="mt-1 text-sm text-gray-900">{room.roomType}</p>
              </div>
              {room.floor && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Floor</label>
                  <p className="mt-1 text-sm text-gray-900">{room.floor}</p>
                </div>
              )}
              <div>
                <label className="block text-sm font-medium text-gray-700">Price Per Day</label>
                <p className="mt-1 text-sm text-gray-900">${room.pricePerDay}</p>
              </div>
              {room.description && (
                <div className="md:col-span-2">
                  <label className="block text-sm font-medium text-gray-700">Description</label>
                  <p className="mt-1 text-sm text-gray-900">{room.description}</p>
                </div>
              )}
            </div>

            <div className="mt-8 flex space-x-4">
              <Link
                href={`/admin/rooms/${roomId}/edit`}
                className="px-4 py-2 text-sm text-white bg-primary-600 rounded-md hover:bg-primary-700"
              >
                Edit Room
              </Link>
            </div>
          </div>

          <div className="bg-white shadow rounded-lg p-6">
            <div className="flex items-center justify-between mb-4">
              <div>
                <h3 className="text-lg font-semibold text-gray-900">Beds</h3>
                <p className="text-sm text-gray-500">Beds assigned to this room</p>
              </div>
            </div>

            {bedsLoading ? (
              <div className="text-center py-6">Loading beds...</div>
            ) : beds && beds.length > 0 ? (
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Bed Number</th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Description</th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Availability</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {beds.map((bed) => (
                      <tr key={bed.id}>
                        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{bed.bedNumber}</td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                          {bed.description || '-'}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span
                            className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                              bed.isAvailable && !bed.isOccupied
                                ? 'bg-green-100 text-green-800'
                                : bed.isOccupied
                                ? 'bg-red-100 text-red-800'
                                : 'bg-yellow-100 text-yellow-800'
                            }`}
                          >
                            {bed.isOccupied ? 'Occupied' : bed.isAvailable ? 'Available' : 'Unavailable'}
                          </span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <div className="text-center py-8 text-gray-500">No beds assigned to this room yet.</div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}

