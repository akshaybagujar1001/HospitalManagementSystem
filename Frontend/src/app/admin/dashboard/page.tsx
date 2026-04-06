'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface DashboardStats {
  totalPatients: number;
  totalDoctors: number;
  totalNurses: number;
  monthlyNewPatients: number;
  activeInpatients: number;
  todayAppointments: number;
  availableRooms: number;
}

export default function AdminDashboard() {
  const router = useRouter();
  const { user, isAuthenticated, logout } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: stats, isLoading } = useQuery<DashboardStats>({
    queryKey: ['admin-dashboard-overview'],
    queryFn: async () => {
      try {
        const response = await api.get('/dashboard/overview');
        return response.data;
      } catch (error) {
        console.error('Error fetching stats:', error);
        return {
          totalPatients: 0,
          totalDoctors: 0,
          totalNurses: 0,
          monthlyNewPatients: 0,
          activeInpatients: 0,
          todayAppointments: 0,
          availableRooms: 0,
        };
      }
    },
    enabled: isAuthenticated && user?.role === 'Admin',
    refetchInterval: 30000, // Refetch every 30 seconds
  });

  const { data: financial } = useQuery({
    queryKey: ['admin-dashboard-financial'],
    queryFn: async () => {
      const response = await api.get('/dashboard/financial');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Admin',
  });

  const { data: operational } = useQuery({
    queryKey: ['admin-dashboard-operational'],
    queryFn: async () => {
      const response = await api.get('/dashboard/operational');
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
            <div className="flex items-center">
              <h1 className="text-xl font-semibold">HMS Admin Dashboard</h1>
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
          <h2 className="text-2xl font-bold mb-6">Dashboard Overview</h2>
          
          {isLoading ? (
            <div className="text-center py-12">Loading...</div>
          ) : (
            <>
              <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
                <div className="bg-white overflow-hidden shadow rounded-lg">
                  <div className="p-5">
                    <div className="flex items-center">
                      <div className="flex-shrink-0">
                        <div className="w-8 h-8 bg-blue-500 rounded-full flex items-center justify-center">
                          <span className="text-white text-sm font-bold">P</span>
                        </div>
                      </div>
                      <div className="ml-5 w-0 flex-1">
                        <dl>
                          <dt className="text-sm font-medium text-gray-500 truncate">
                            Total Patients
                          </dt>
                          <dd className="text-lg font-medium text-gray-900">
                            {stats?.totalPatients || 0}
                          </dd>
                          <dd className="text-xs text-gray-500">
                            {stats?.monthlyNewPatients || 0} new this month
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
                          <span className="text-white text-sm font-bold">D</span>
                        </div>
                      </div>
                      <div className="ml-5 w-0 flex-1">
                        <dl>
                          <dt className="text-sm font-medium text-gray-500 truncate">
                            Total Doctors
                          </dt>
                          <dd className="text-lg font-medium text-gray-900">
                            {stats?.totalDoctors || 0}
                          </dd>
                          <dd className="text-xs text-gray-500">
                            {stats?.totalNurses || 0} nurses
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
                          <span className="text-white text-sm font-bold">A</span>
                        </div>
                      </div>
                      <div className="ml-5 w-0 flex-1">
                        <dl>
                          <dt className="text-sm font-medium text-gray-500 truncate">
                            Today's Appointments
                          </dt>
                          <dd className="text-lg font-medium text-gray-900">
                            {stats?.todayAppointments || 0}
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
                        <div className="w-8 h-8 bg-purple-500 rounded-full flex items-center justify-center">
                          <span className="text-white text-sm font-bold">R</span>
                        </div>
                      </div>
                      <div className="ml-5 w-0 flex-1">
                        <dl>
                          <dt className="text-sm font-medium text-gray-500 truncate">
                            Available Rooms
                          </dt>
                          <dd className="text-lg font-medium text-gray-900">
                            {stats?.availableRooms || 0}
                          </dd>
                          <dd className="text-xs text-gray-500">
                            {stats?.activeInpatients || 0} inpatients
                          </dd>
                        </dl>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              {/* Financial KPIs */}
              {financial && (
                <div className="mt-8">
                  <h3 className="text-lg font-semibold mb-4">Financial Overview</h3>
                  <div className="grid grid-cols-1 gap-5 sm:grid-cols-3">
                    <div className="bg-white overflow-hidden shadow rounded-lg p-5">
                      <dt className="text-sm font-medium text-gray-500">Total Revenue</dt>
                      <dd className="mt-1 text-2xl font-semibold text-green-600">
                        ${financial.totalRevenue.toLocaleString()}
                      </dd>
                    </div>
                    <div className="bg-white overflow-hidden shadow rounded-lg p-5">
                      <dt className="text-sm font-medium text-gray-500">Total Expenses</dt>
                      <dd className="mt-1 text-2xl font-semibold text-red-600">
                        ${financial.totalExpenses.toLocaleString()}
                      </dd>
                    </div>
                    <div className="bg-white overflow-hidden shadow rounded-lg p-5">
                      <dt className="text-sm font-medium text-gray-500">Net Profit</dt>
                      <dd className={`mt-1 text-2xl font-semibold ${
                        financial.netProfit >= 0 ? 'text-green-600' : 'text-red-600'
                      }`}>
                        ${financial.netProfit.toLocaleString()}
                      </dd>
                    </div>
                  </div>
                </div>
              )}

              {/* Operational KPIs */}
              {operational && (
                <div className="mt-8">
                  <h3 className="text-lg font-semibold mb-4">Operational Metrics</h3>
                  <div className="grid grid-cols-1 gap-5 sm:grid-cols-3">
                    <div className="bg-white overflow-hidden shadow rounded-lg p-5">
                      <dt className="text-sm font-medium text-gray-500">Room Occupancy Rate</dt>
                      <dd className="mt-1 text-2xl font-semibold text-gray-900">
                        {operational.roomOccupancyRate.toFixed(1)}%
                      </dd>
                    </div>
                    <div className="bg-white overflow-hidden shadow rounded-lg p-5">
                      <dt className="text-sm font-medium text-gray-500">Avg Waiting Time</dt>
                      <dd className="mt-1 text-2xl font-semibold text-gray-900">
                        {operational.averageWaitingTimeHours.toFixed(1)} hrs
                      </dd>
                    </div>
                    <div className="bg-white overflow-hidden shadow rounded-lg p-5">
                      <dt className="text-sm font-medium text-gray-500">Lab Tests (Period)</dt>
                      <dd className="mt-1 text-2xl font-semibold text-gray-900">
                        {operational.totalLabTests}
                      </dd>
                    </div>
                  </div>
                </div>
              )}
            </>
          )}

          <div className="mt-8">
            <h3 className="text-lg font-semibold mb-4">Quick Actions</h3>
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
              <Link
                href="/admin/patients"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Manage Patients</h4>
                <p className="text-sm text-gray-600">View and manage patient records</p>
              </Link>
              <Link
                href="/admin/doctors"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Manage Doctors</h4>
                <p className="text-sm text-gray-600">View and manage doctor profiles</p>
              </Link>
              <Link
                href="/admin/appointments"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Manage Appointments</h4>
                <p className="text-sm text-gray-600">View and manage all appointments</p>
              </Link>
              <Link
                href="/admin/rooms"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Manage Rooms & Beds</h4>
                <p className="text-sm text-gray-600">View and manage hospital rooms and beds</p>
              </Link>
              <Link
                href="/admin/invoices"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Manage Invoices</h4>
                <p className="text-sm text-gray-600">View and manage billing invoices</p>
              </Link>
              <Link
                href="/admin/lab-tests"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Manage Lab Tests</h4>
                <p className="text-sm text-gray-600">View and manage laboratory tests</p>
              </Link>
              <Link
                href="/admin/notifications"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Notifications</h4>
                <p className="text-sm text-gray-600">View system notifications</p>
              </Link>
              <Link
                href="/admin/insurance"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Insurance Management</h4>
                <p className="text-sm text-gray-600">Manage insurance companies and policies</p>
              </Link>
              <Link
                href="/admin/pharmacy"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Pharmacy</h4>
                <p className="text-sm text-gray-600">Manage medications and inventory</p>
              </Link>
              <Link
                href="/admin/radiology"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Radiology</h4>
                <p className="text-sm text-gray-600">Manage imaging orders and reports</p>
              </Link>
              <Link
                href="/admin/emergency"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Emergency Department</h4>
                <p className="text-sm text-gray-600">Monitor emergency cases</p>
              </Link>
              <Link
                href="/admin/assets"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Asset Management</h4>
                <p className="text-sm text-gray-600">Track hospital assets</p>
              </Link>
              <Link
                href="/admin/finance"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Financial Management</h4>
                <p className="text-sm text-gray-600">View financial reports and accounting</p>
              </Link>
              <Link
                href="/admin/shifts"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Staff Shifts</h4>
                <p className="text-sm text-gray-600">Manage staff scheduling</p>
              </Link>
              <Link
                href="/admin/audit-logs"
                className="block p-6 bg-white rounded-lg shadow hover:shadow-md transition cursor-pointer"
              >
                <h4 className="text-lg font-medium mb-2">Audit Logs</h4>
                <p className="text-sm text-gray-600">View system activity logs</p>
              </Link>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}

