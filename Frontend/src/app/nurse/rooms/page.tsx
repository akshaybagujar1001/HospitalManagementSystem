'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
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

export default function NurseRoomsPage() {
  const router = useRouter();
  const { user, isAuthenticated, logout } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Nurse') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: rooms, isLoading } = useQuery<Room[]>({
    queryKey: ['nurse-rooms-directory'],
    queryFn: async () => {
      const response = await api.get('/rooms');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Nurse',
  });

  if (!isAuthenticated || user?.role !== 'Nurse') {
    return null;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/nurse/dashboard" className="text-primary-600 hover:text-primary-700">
                ← Back to Dashboard
              </Link>
              <h1 className="text-xl font-semibold">Room Directory</h1>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-sm text-gray-700">
                Nurse {user?.firstName} {user?.lastName}
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
          <div className="bg-white shadow rounded-lg p-6">
            <div className="flex justify-between items-center mb-4">
              <div>
                <h2 className="text-2xl font-bold text-gray-900">All Rooms</h2>
                <p className="text-sm text-gray-500">View current availability and details.</p>
              </div>
            </div>

            {isLoading ? (
              <div className="text-center py-12">Loading rooms...</div>
            ) : rooms && rooms.length > 0 ? (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {rooms.map((room) => (
                  <div key={room.id} className="border border-gray-200 rounded-lg p-5 bg-white shadow-sm">
                    <div className="flex justify-between items-start mb-3">
                      <div>
                        <p className="text-sm font-semibold text-gray-900">Room {room.roomNumber}</p>
                        <p className="text-xs text-gray-500">{room.roomType}</p>
                      </div>
                      <span
                        className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                          room.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                        }`}
                      >
                        {room.isAvailable ? 'Available' : 'Occupied'}
                      </span>
                    </div>
                    {room.floor && (
                      <p className="text-xs text-gray-500 mb-1">
                        <span className="font-medium text-gray-600">Floor:</span> {room.floor}
                      </p>
                    )}
                    {room.description && (
                      <p className="text-xs text-gray-500 mb-3">
                        <span className="font-medium text-gray-600">Notes:</span> {room.description}
                      </p>
                    )}
                    <p className="text-xs text-gray-500">
                      <span className="font-medium text-gray-600">Rate:</span> ${room.pricePerDay}/day
                    </p>
                  </div>
                ))}
              </div>
            ) : (
              <div className="text-center py-12 text-gray-500">No rooms found</div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}

