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
  pricePerDay: number;
  isAvailable: boolean;
}

export default function AdminRoomsPage() {
  const router = useRouter();
  const { user, isAuthenticated, logout } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: rooms, isLoading } = useQuery<Room[]>({
    queryKey: ['rooms'],
    queryFn: async () => {
      const response = await api.get('/rooms');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Admin',
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
              <Link href="/admin/dashboard" className="text-primary-600 hover:text-primary-700">
                ← Back to Dashboard
              </Link>
              <h1 className="text-xl font-semibold">Manage Rooms & Beds</h1>
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
          <div className="mb-4 flex justify-between items-center">
            <h2 className="text-2xl font-bold">All Rooms</h2>
            <Link
              href="/admin/rooms/add"
              className="px-4 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700"
            >
              Add New Room
            </Link>
          </div>

          {isLoading ? (
            <div className="text-center py-12">Loading rooms...</div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {rooms && rooms.length > 0 ? (
                rooms.map((room) => (
                  <div key={room.id} className="bg-white shadow rounded-lg p-6">
                    <div className="flex justify-between items-start mb-4">
                      <div>
                        <h3 className="text-lg font-semibold text-gray-900">
                          Room {room.roomNumber}
                        </h3>
                        <p className="text-sm text-gray-500">{room.roomType}</p>
                      </div>
                      <span className={`px-2 py-1 text-xs font-semibold rounded-full ${
                        room.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                      }`}>
                        {room.isAvailable ? 'Available' : 'Occupied'}
                      </span>
                    </div>
                    <div className="space-y-2 text-sm">
                      {room.floor && (
                        <p className="text-gray-600">
                          <span className="font-medium">Floor:</span> {room.floor}
                        </p>
                      )}
                      <p className="text-gray-600">
                        <span className="font-medium">Price:</span> ${room.pricePerDay}/day
                      </p>
                    </div>
                    <div className="mt-4 flex space-x-2">
                      <Link
                        href={`/admin/rooms/${room.id}`}
                        className="flex-1 text-center px-4 py-2 text-sm text-primary-600 border border-primary-600 rounded-md hover:bg-primary-50"
                      >
                        View Details
                      </Link>
                    </div>
                  </div>
                ))
              ) : (
                <div className="col-span-full text-center py-12 text-gray-500">
                  No rooms found
                </div>
              )}
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

