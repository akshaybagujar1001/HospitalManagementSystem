'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Appointment {
  id: number;
  doctorName: string;
  appointmentDate: string;
  appointmentTime: string;
  status: string;
}

interface MedicalRecord {
  id: number;
  diagnosis: string;
  recordDate: string;
}

interface Prescription {
  id: number;
  medicationName: string;
  prescribedDate: string;
  isActive: boolean;
}

export default function PatientDashboard() {
  const router = useRouter();
  const { user, isAuthenticated, logout } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Patient') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: appointments, isLoading: appointmentsLoading } = useQuery<Appointment[]>({
    queryKey: ['patient-appointments'],
    queryFn: async () => {
      const response = await api.get('/appointments');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Patient',
  });

  const { data: medicalRecords, isLoading: recordsLoading } = useQuery<MedicalRecord[]>({
    queryKey: ['patient-medical-records'],
    queryFn: async () => {
      const response = await api.get('/medical-records');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Patient',
  });

  const { data: prescriptions, isLoading: prescriptionsLoading } = useQuery<Prescription[]>({
    queryKey: ['patient-prescriptions'],
    queryFn: async () => {
      const response = await api.get('/prescriptions');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Patient',
  });

  if (!isAuthenticated || user?.role !== 'Patient') {
    return null;
  }

  const recentRecords = medicalRecords?.slice(0, 3) || [];
  const recentPrescriptions = prescriptions?.filter(p => p.isActive).slice(0, 3) || [];

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <h1 className="text-xl font-semibold">Patient Dashboard</h1>
            </div>
            <div className="flex items-center space-x-4">
              <span className="text-sm text-gray-700">
                Welcome, {user?.firstName} {user?.lastName}
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
          <h2 className="text-2xl font-bold mb-6">My Appointments</h2>
          
          {appointmentsLoading ? (
            <div className="text-center py-12">Loading appointments...</div>
          ) : (
            <div className="bg-white shadow overflow-hidden sm:rounded-md">
              <ul className="divide-y divide-gray-200">
                {appointments && appointments.length > 0 ? (
                  appointments.map((appointment) => (
                    <li key={appointment.id} className="px-6 py-4">
                      <div className="flex items-center justify-between">
                        <div>
                          <p className="text-sm font-medium text-gray-900">
                            Dr. {appointment.doctorName}
                          </p>
                          <p className="text-sm text-gray-500">
                            {new Date(appointment.appointmentDate).toLocaleDateString()} at {appointment.appointmentTime}
                          </p>
                        </div>
                        <span className={`px-3 py-1 text-xs font-semibold rounded-full ${
                          appointment.status === 'Scheduled' ? 'bg-green-100 text-green-800' :
                          appointment.status === 'Completed' ? 'bg-blue-100 text-blue-800' :
                          'bg-gray-100 text-gray-800'
                        }`}>
                          {appointment.status}
                        </span>
                      </div>
                    </li>
                  ))
                ) : (
                  <li className="px-6 py-12 text-center text-gray-500">
                    No appointments found
                  </li>
                )}
              </ul>
            </div>
          )}

          <div className="mt-8 grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div>
              <h2 className="text-xl font-bold mb-4">Medical Records</h2>
              <div className="bg-white shadow overflow-hidden sm:rounded-md">
                {recordsLoading ? (
                  <div className="px-6 py-12 text-center text-gray-500">Loading records...</div>
                ) : recentRecords.length > 0 ? (
                  <>
                    <ul className="divide-y divide-gray-200">
                      {recentRecords.map((record) => (
                        <li key={record.id} className="px-6 py-4">
                          <div>
                            <p className="text-sm font-medium text-gray-900">{record.diagnosis}</p>
                            <p className="text-xs text-gray-500">
                              {new Date(record.recordDate).toLocaleDateString()}
                            </p>
                          </div>
                        </li>
                      ))}
                    </ul>
                    <div className="px-6 py-4 border-t border-gray-200">
                      <Link href="/patient/medical-records" className="text-primary-600 hover:text-primary-500 text-sm font-medium">
                        View All Records →
                      </Link>
                    </div>
                  </>
                ) : (
                  <div className="px-6 py-12 text-center text-gray-500">
                    <p>No medical records available</p>
                    <Link href="/patient/medical-records" className="text-primary-600 hover:text-primary-500 mt-2 inline-block">
                      View All Records
                    </Link>
                  </div>
                )}
              </div>
            </div>

            <div>
              <h2 className="text-xl font-bold mb-4">Recent Prescriptions</h2>
              <div className="bg-white shadow overflow-hidden sm:rounded-md">
                {prescriptionsLoading ? (
                  <div className="px-6 py-12 text-center text-gray-500">Loading prescriptions...</div>
                ) : recentPrescriptions.length > 0 ? (
                  <>
                    <ul className="divide-y divide-gray-200">
                      {recentPrescriptions.map((prescription) => (
                        <li key={prescription.id} className="px-6 py-4">
                          <div>
                            <p className="text-sm font-medium text-gray-900">{prescription.medicationName}</p>
                            <p className="text-xs text-gray-500">
                              {new Date(prescription.prescribedDate).toLocaleDateString()}
                            </p>
                          </div>
                        </li>
                      ))}
                    </ul>
                    <div className="px-6 py-4 border-t border-gray-200">
                      <Link href="/patient/prescriptions" className="text-primary-600 hover:text-primary-500 text-sm font-medium">
                        View All Prescriptions →
                      </Link>
                    </div>
                  </>
                ) : (
                  <div className="px-6 py-12 text-center text-gray-500">
                    <p>No prescriptions available</p>
                    <Link href="/patient/prescriptions" className="text-primary-600 hover:text-primary-500 mt-2 inline-block">
                      View All Prescriptions
                    </Link>
                  </div>
                )}
              </div>
            </div>
          </div>

          <div className="mt-8">
            <Link
              href="/patient/appointments/new"
              className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-primary-600 hover:bg-primary-700"
            >
              Book New Appointment
            </Link>
          </div>
        </div>
      </main>
    </div>
  );
}

