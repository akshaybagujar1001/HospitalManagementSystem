'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Appointment {
  id: number;
  patientName: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
  reason?: string;
}

interface Prescription {
  id: number;
  patientName: string;
  medicationName: string;
  dosage?: string;
  prescribedDate: string;
  isActive: boolean;
}

export default function DoctorDashboard() {
  const router = useRouter();
  const { user, isAuthenticated, logout } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Doctor') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: appointments, isLoading: appointmentsLoading } = useQuery<Appointment[]>({
    queryKey: ['doctor-appointments'],
    queryFn: async () => {
      const response = await api.get('/appointments');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Doctor',
  });

  const { data: prescriptions, isLoading: prescriptionsLoading } = useQuery<Prescription[]>({
    queryKey: ['doctor-prescriptions'],
    queryFn: async () => {
      const response = await api.get('/prescriptions?isActive=true');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Doctor',
  });

  if (!isAuthenticated || user?.role !== 'Doctor') {
    return null;
  }

  const upcomingAppointments = appointments?.filter(a => 
    a.status === 'Scheduled' && 
    new Date(a.appointmentDate) >= new Date()
  ) || [];

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <h1 className="text-xl font-semibold">Doctor Dashboard</h1>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-sm text-gray-700">
                Welcome, Dr. {user?.firstName} {user?.lastName}
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
          <div className="grid grid-cols-1 gap-5 sm:grid-cols-3 mb-8">
            <div className="bg-white overflow-hidden shadow rounded-lg">
              <div className="p-5">
                <div className="flex items-center">
                  <div className="flex-shrink-0">
                    <div className="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center">
                      <span className="text-white text-sm font-bold">A</span>
                    </div>
                  </div>
                  <div className="ml-5 w-0 flex-1">
                    <dl>
                      <dt className="text-sm font-medium text-gray-500 truncate">
                        Upcoming Appointments
                      </dt>
                      <dd className="text-lg font-medium text-gray-900">
                        {upcomingAppointments.length}
                      </dd>
                    </dl>
                  </div>
                </div>
              </div>
            </div>

            <div className="bg-white overflow-hidden shadow rounded-lg">
              <div className="p-5">
                <div className="flex items-center">
                  <div className="flex-shrink-0">
                    <div className="w-8 h-8 bg-green-500 rounded-full flex items-center justify-center">
                      <span className="text-white text-sm font-bold">P</span>
                    </div>
                  </div>
                  <div className="ml-5 w-0 flex-1">
                    <dl>
                      <dt className="text-sm font-medium text-gray-500 truncate">
                        Active Prescriptions
                      </dt>
                      <dd className="text-lg font-medium text-gray-900">
                        {prescriptions?.length || 0}
                      </dd>
                    </dl>
                  </div>
                </div>
              </div>
            </div>

            <div className="bg-white overflow-hidden shadow rounded-lg">
              <div className="p-5">
                <div className="flex items-center">
                  <div className="flex-shrink-0">
                    <div className="w-8 h-8 bg-yellow-500 rounded-full flex items-center justify-center">
                      <span className="text-white text-sm font-bold">T</span>
                    </div>
                  </div>
                  <div className="ml-5 w-0 flex-1">
                    <dl>
                      <dt className="text-sm font-medium text-gray-500 truncate">
                        Today's Appointments
                      </dt>
                      <dd className="text-lg font-medium text-gray-900">
                        {appointments?.filter(a => 
                          new Date(a.appointmentDate).toDateString() === new Date().toDateString()
                        ).length || 0}
                      </dd>
                    </dl>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div>
              <h2 className="text-xl font-bold mb-4">Upcoming Appointments</h2>
              {appointmentsLoading ? (
                <div className="text-center py-12">Loading...</div>
              ) : (
                <div className="bg-white shadow overflow-hidden sm:rounded-md">
                  <ul className="divide-y divide-gray-200">
                    {upcomingAppointments.length > 0 ? (
                      upcomingAppointments.slice(0, 5).map((appointment) => (
                        <li key={appointment.id} className="px-6 py-4">
                          <div className="flex items-center justify-between">
                            <div>
                              <p className="text-sm font-medium text-gray-900">
                                {appointment.patientName}
                              </p>
                              <p className="text-sm text-gray-500">
                                {new Date(appointment.appointmentDate).toLocaleDateString()} at {appointment.appointmentTime}
                              </p>
                              {appointment.reason && (
                                <p className="text-xs text-gray-400 mt-1">{appointment.reason}</p>
                              )}
                            </div>
                            <span className="px-2 py-1 text-xs font-semibold rounded-full bg-green-100 text-green-800">
                              {appointment.status}
                            </span>
                          </div>
                        </li>
                      ))
                    ) : (
                      <li className="px-6 py-12 text-center text-gray-500">
                        No upcoming appointments
                      </li>
                    )}
                  </ul>
                </div>
              )}
            </div>

            <div>
              <h2 className="text-xl font-bold mb-4">Recent Prescriptions</h2>
              {prescriptionsLoading ? (
                <div className="text-center py-12">Loading...</div>
              ) : (
                <div className="bg-white shadow overflow-hidden sm:rounded-md">
                  <ul className="divide-y divide-gray-200">
                    {prescriptions && prescriptions.length > 0 ? (
                      prescriptions.slice(0, 5).map((prescription) => (
                        <li key={prescription.id} className="px-6 py-4">
                          <div>
                            <p className="text-sm font-medium text-gray-900">
                              {prescription.medicationName}
                            </p>
                            <p className="text-sm text-gray-500">
                              {prescription.patientName}
                            </p>
                            {prescription.dosage && (
                              <p className="text-xs text-gray-400 mt-1">
                                Dosage: {prescription.dosage}
                              </p>
                            )}
                            <p className="text-xs text-gray-400">
                              {new Date(prescription.prescribedDate).toLocaleDateString()}
                            </p>
                          </div>
                        </li>
                      ))
                    ) : (
                      <li className="px-6 py-12 text-center text-gray-500">
                        No active prescriptions
                      </li>
                    )}
                  </ul>
                </div>
              )}
            </div>
          </div>

          <div className="mt-8">
            <h3 className="text-lg font-semibold mb-4">Quick Actions</h3>
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
              <Link
                href="/doctor/appointments"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">View All Appointments</h4>
                <p className="text-sm text-gray-600">Manage all your appointments</p>
              </Link>
              <Link
                href="/doctor/prescriptions"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Manage Prescriptions</h4>
                <p className="text-sm text-gray-600">Create and manage prescriptions</p>
              </Link>
              <Link
                href="/doctor/patients"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">View Patients</h4>
                <p className="text-sm text-gray-600">Access patient records</p>
              </Link>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}

