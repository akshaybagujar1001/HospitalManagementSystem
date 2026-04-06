'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface EmergencyCase {
  id: number;
  patientId: number;
  triageLevel: number;
  description: string;
  status: string;
  arrivalTime: string;
}

export default function EmergencyPage() {
  const router = useRouter();
  const { user, isAuthenticated, logout } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: activeCases } = useQuery<EmergencyCase[]>({
    queryKey: ['emergency-cases-active'],
    queryFn: async () => {
      const response = await api.get('/emergency-cases/active');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Admin',
    refetchInterval: 10000, // Refetch every 10 seconds
  });

  const getTriageColor = (level: number) => {
    switch (level) {
      case 1: return 'bg-red-600 text-white';
      case 2: return 'bg-orange-500 text-white';
      case 3: return 'bg-yellow-500 text-white';
      case 4: return 'bg-blue-500 text-white';
      default: return 'bg-gray-500 text-white';
    }
  };

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
              <h1 className="text-xl font-semibold">Emergency Department</h1>
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

      <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="mb-4">
            <h2 className="text-2xl font-bold">Active Emergency Cases</h2>
            <p className="text-sm text-gray-500">Real-time monitoring of emergency cases</p>
          </div>

          {activeCases && activeCases.length > 0 ? (
            <div className="grid grid-cols-1 gap-4">
              {activeCases.map((case_) => (
                <div key={case_.id} className="bg-white shadow rounded-lg p-6">
                  <div className="flex items-center justify-between">
                    <div className="flex-1">
                      <div className="flex items-center space-x-2 mb-2">
                        <span className={`px-3 py-1 text-sm font-semibold rounded ${getTriageColor(case_.triageLevel)}`}>
                          Triage Level {case_.triageLevel}
                        </span>
                        <span className="px-2 py-1 text-xs font-semibold rounded-full bg-gray-100 text-gray-800">
                          {case_.status}
                        </span>
                      </div>
                      <p className="text-sm text-gray-900 mb-2">{case_.description}</p>
                      <p className="text-xs text-gray-500">
                        Arrived: {new Date(case_.arrivalTime).toLocaleString()}
                      </p>
                    </div>
                    <Link
                      href={`/admin/emergency/${case_.id}`}
                      className="px-4 py-2 text-sm bg-primary-600 text-white rounded-md hover:bg-primary-700"
                    >
                      View Details
                    </Link>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="bg-white shadow rounded-lg p-12 text-center text-gray-500">
              No active emergency cases.
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

