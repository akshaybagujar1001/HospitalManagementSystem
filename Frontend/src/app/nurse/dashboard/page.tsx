'use client';

import { useMemo } from 'react';
import { useEffect } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';

interface Patient {
  id: number;
  firstName: string;
  lastName: string;
  patientNumber: string;
  gender: string;
}

interface Room {
  id: number;
  roomNumber: string;
  roomType: string;
  isAvailable: boolean;
}

interface Appointment {
  id: number;
  patientName: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
}

export default function NurseDashboardPage() {
  const router = useRouter();
  const { user, isAuthenticated, logout } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Nurse') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const patientsQuery = useQuery<Patient[]>({
    queryKey: ['nurse-patients'],
    queryFn: async () => {
      const response = await api.get('/patients');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Nurse',
  });

  const roomsQuery = useQuery<Room[]>({
    queryKey: ['nurse-rooms'],
    queryFn: async () => {
      const response = await api.get('/rooms');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Nurse',
  });

  const appointmentsQuery = useQuery<Appointment[]>({
    queryKey: ['nurse-appointments'],
    queryFn: async () => {
      const response = await api.get('/appointments');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Nurse',
  });

  const totalPatients = patientsQuery.data?.length ?? 0;
  const totalRooms = roomsQuery.data?.length ?? 0;
  const availableRooms = roomsQuery.data?.filter((room) => room.isAvailable).length ?? 0;
  const upcomingAppointments =
    appointmentsQuery.data?.filter((appt) => appt.status === 'Scheduled').length ?? 0;

  const nextAppointments = useMemo(() => {
    if (!appointmentsQuery.data) return [];
    return appointmentsQuery.data
      .filter((appt) => appt.status === 'Scheduled')
      .sort(
        (a, b) =>
          new Date(a.appointmentDate + 'T' + a.appointmentTime).getTime() -
          new Date(b.appointmentDate + 'T' + b.appointmentTime).getTime(),
      )
      .slice(0, 5);
  }, [appointmentsQuery.data]);

  const recentPatients = useMemo(() => {
    if (!patientsQuery.data) return [];
    return patientsQuery.data.slice(0, 5);
  }, [patientsQuery.data]);

  if (!isAuthenticated || user?.role !== 'Nurse') {
    return null;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <h1 className="text-xl font-semibold text-gray-900">Nurse Dashboard</h1>
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
        <div className="px-4 py-6 sm:px-0 space-y-6">
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
            <div className="bg-white shadow rounded-lg p-5">
              <p className="text-sm text-gray-500">Total Patients</p>
              <p className="text-3xl font-bold text-gray-900">{totalPatients}</p>
            </div>
            <div className="bg-white shadow rounded-lg p-5">
              <p className="text-sm text-gray-500">Available Rooms</p>
              <p className="text-3xl font-bold text-gray-900">
                {availableRooms} / {totalRooms}
              </p>
            </div>
            <div className="bg-white shadow rounded-lg p-5">
              <p className="text-sm text-gray-500">Upcoming Appointments</p>
              <p className="text-3xl font-bold text-gray-900">{upcomingAppointments}</p>
            </div>
            <div className="bg-white shadow rounded-lg p-5">
              <p className="text-sm text-gray-500">Role</p>
              <p className="text-3xl font-bold text-gray-900">Nurse</p>
            </div>
          </div>

          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div className="bg-white shadow rounded-lg">
              <div className="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
                <div>
                  <h2 className="text-lg font-semibold text-gray-900">Upcoming Appointments</h2>
                  <p className="text-sm text-gray-500">Next 5 scheduled appointments</p>
                </div>
              </div>
              <div className="p-6">
                {appointmentsQuery.isLoading ? (
                  <div className="text-center text-gray-500">Loading appointments...</div>
                ) : nextAppointments.length > 0 ? (
                  <ul className="divide-y divide-gray-200">
                    {nextAppointments.map((appointment) => (
                      <li key={appointment.id} className="py-4 flex items-center justify-between">
                        <div>
                          <p className="text-sm font-medium text-gray-900">{appointment.patientName}</p>
                          <p className="text-xs text-gray-500">
                            {new Date(appointment.appointmentDate).toLocaleDateString()} at{' '}
                            {appointment.appointmentTime}
                          </p>
                        </div>
                        <span className="text-xs font-semibold text-green-700 bg-green-100 px-2 py-1 rounded-full">
                          {appointment.status}
                        </span>
                      </li>
                    ))}
                  </ul>
                ) : (
                  <div className="text-center text-gray-500">No upcoming appointments</div>
                )}
              </div>
            </div>

            <div className="bg-white shadow rounded-lg">
              <div className="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
                <div>
                  <h2 className="text-lg font-semibold text-gray-900">Recent Patients</h2>
                  <p className="text-sm text-gray-500">Most recently registered patients</p>
                </div>
              </div>
              <div className="p-6">
                {patientsQuery.isLoading ? (
                  <div className="text-center text-gray-500">Loading patients...</div>
                ) : recentPatients.length > 0 ? (
                  <ul className="divide-y divide-gray-200">
                    {recentPatients.map((patient) => (
                      <li key={patient.id} className="py-4 flex items-center justify-between">
                        <div>
                          <p className="text-sm font-medium text-gray-900">
                            {patient.firstName} {patient.lastName}
                          </p>
                          <p className="text-xs text-gray-500">#{patient.patientNumber}</p>
                        </div>
                        <span className="text-xs text-gray-500">{patient.gender}</span>
                      </li>
                    ))}
                  </ul>
                ) : (
                  <div className="text-center text-gray-500">No patients available</div>
                )}
              </div>
            </div>
          </div>

          <div className="bg-white shadow rounded-lg">
            <div className="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
              <div>
                <h2 className="text-lg font-semibold text-gray-900">Room Availability</h2>
                <p className="text-sm text-gray-500">Current room status overview</p>
              </div>
              <Link href="/nurse/rooms" className="text-sm text-primary-600 hover:text-primary-700">
                Manage Rooms
              </Link>
            </div>
            <div className="p-6">
              {roomsQuery.isLoading ? (
                <div className="text-center text-gray-500">Loading rooms...</div>
              ) : roomsQuery.data && roomsQuery.data.length > 0 ? (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  {roomsQuery.data.slice(0, 6).map((room) => (
                    <div
                      key={room.id}
                      className="border border-gray-200 rounded-lg p-4 flex items-center justify-between"
                    >
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
                  ))}
                </div>
              ) : (
                <div className="text-center text-gray-500">No room data available</div>
              )}
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}

