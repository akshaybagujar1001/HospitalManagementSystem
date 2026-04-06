'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Doctor {
  id: number;
  firstName: string;
  lastName: string;
  specialization: string;
}

interface Nurse {
  id: number;
  firstName: string;
  lastName: string;
}

export default function NewShiftPage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { user, isAuthenticated, logout } = useAuthStore();
  const [formData, setFormData] = useState({
    staffId: '',
    role: 'Doctor',
    startTime: '',
    endTime: '',
    department: '',
    notes: '',
  });

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: doctors } = useQuery<Doctor[]>({
    queryKey: ['doctors'],
    queryFn: async () => {
      const response = await api.get('/doctors');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Admin',
  });

  const { data: nurses } = useQuery<Nurse[]>({
    queryKey: ['nurses'],
    queryFn: async () => {
      const response = await api.get('/nurses');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Admin',
  });

  const createMutation = useMutation({
    mutationFn: async (data: any) => {
      const response = await api.post('/staff-shifts', {
        staffId: parseInt(data.staffId),
        role: data.role,
        startTime: new Date(data.startTime).toISOString(),
        endTime: new Date(data.endTime).toISOString(),
        department: data.department || null,
        notes: data.notes || null,
      });
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['staff-shifts'] });
      queryClient.invalidateQueries({ queryKey: ['staff-shifts-week'] });
      router.push('/admin/shifts');
    },
    onError: (error: any) => {
      console.error('Error creating shift:', error);
      const errorMessage = error?.response?.data?.message || 
                          error?.response?.data?.details || 
                          error?.message || 
                          'Failed to create shift';
      alert(`Error: ${errorMessage}\n\nPlease check the browser console for more details.`);
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.staffId) {
      alert('Please select a staff member');
      return;
    }

    if (!formData.startTime || !formData.endTime) {
      alert('Please select both start and end times');
      return;
    }

    const startDate = new Date(formData.startTime);
    const endDate = new Date(formData.endTime);

    if (endDate <= startDate) {
      alert('End time must be after start time');
      return;
    }

    createMutation.mutate(formData);
  };

  const staffOptions = formData.role === 'Doctor' 
    ? doctors?.map(d => ({ id: d.id, name: `${d.firstName} ${d.lastName} (${d.specialization})` }))
    : nurses?.map(n => ({ id: n.id, name: `${n.firstName} ${n.lastName}` }));

  if (!isAuthenticated || user?.role !== 'Admin') {
    return null;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/admin/shifts" className="text-primary-600 hover:text-primary-700">
                ← Back to Shifts
              </Link>
              <h1 className="text-xl font-semibold">Add Staff Shift</h1>
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

      <main className="max-w-4xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="bg-white shadow rounded-lg p-6">
            <form onSubmit={handleSubmit} className="space-y-6">
              <div>
                <label className="block text-sm font-medium text-gray-700">Role *</label>
                <select
                  required
                  value={formData.role}
                  onChange={(e) => {
                    setFormData({ ...formData, role: e.target.value, staffId: '' });
                  }}
                  className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="Doctor">Doctor</option>
                  <option value="Nurse">Nurse</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Staff Member *</label>
                <select
                  required
                  value={formData.staffId}
                  onChange={(e) => setFormData({ ...formData, staffId: e.target.value })}
                  className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">Select {formData.role}</option>
                  {staffOptions?.map((staff) => (
                    <option key={staff.id} value={staff.id}>
                      {staff.name}
                    </option>
                  ))}
                </select>
                {(!staffOptions || staffOptions.length === 0) && (
                  <p className="mt-1 text-sm text-gray-500">
                    No {formData.role.toLowerCase()}s available. Please add {formData.role.toLowerCase()}s first.
                  </p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Start Time *</label>
                <input
                  type="datetime-local"
                  required
                  value={formData.startTime}
                  onChange={(e) => setFormData({ ...formData, startTime: e.target.value })}
                  className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">End Time *</label>
                <input
                  type="datetime-local"
                  required
                  value={formData.endTime}
                  onChange={(e) => setFormData({ ...formData, endTime: e.target.value })}
                  className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Department</label>
                <input
                  type="text"
                  value={formData.department}
                  onChange={(e) => setFormData({ ...formData, department: e.target.value })}
                  placeholder="e.g., Emergency, Cardiology, ICU"
                  className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Notes</label>
                <textarea
                  rows={4}
                  value={formData.notes}
                  onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
                  placeholder="Additional notes about this shift..."
                  className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <div className="flex justify-end space-x-4">
                <Link
                  href="/admin/shifts"
                  className="px-6 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50"
                >
                  Cancel
                </Link>
                <button
                  type="submit"
                  disabled={createMutation.isPending || !staffOptions || staffOptions.length === 0}
                  className="px-6 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50"
                >
                  {createMutation.isPending ? 'Creating...' : 'Create Shift'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </main>
    </div>
  );
}

