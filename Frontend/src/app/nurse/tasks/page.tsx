'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface NurseTask {
  id: number;
  description: string;
  priority: string;
  status: string;
  dueTime?: string;
  patientId?: number;
  createdAt: string;
}

export default function NurseTasksPage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { user, isAuthenticated, logout } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Nurse') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: tasks, isLoading } = useQuery<NurseTask[]>({
    queryKey: ['nurse-tasks'],
    queryFn: async () => {
      const response = await api.get('/nurse-tasks');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Nurse',
  });

  const updateStatusMutation = useMutation({
    mutationFn: async ({ id, status }: { id: number; status: string }) => {
      return await api.put(`/nurse-tasks/${id}/status`, { status });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['nurse-tasks'] });
    },
  });

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'Critical': return 'bg-red-100 text-red-800';
      case 'High': return 'bg-orange-100 text-orange-800';
      case 'Medium': return 'bg-yellow-100 text-yellow-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Completed': return 'bg-green-100 text-green-800';
      case 'InProgress': return 'bg-blue-100 text-blue-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  if (!isAuthenticated || user?.role !== 'Nurse') {
    return null;
  }

  const handleStatusChange = (taskId: number, newStatus: string) => {
    updateStatusMutation.mutate({ id: taskId, status: newStatus });
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/nurse/dashboard" className="text-primary-600 hover:text-primary-700">
                ← Back to Dashboard
              </Link>
              <h1 className="text-xl font-semibold">My Tasks</h1>
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
          {isLoading ? (
            <div className="text-center py-12">Loading tasks...</div>
          ) : tasks && tasks.length > 0 ? (
            <div className="grid grid-cols-1 gap-4">
              {tasks.map((task) => (
                <div key={task.id} className="bg-white shadow rounded-lg p-6">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center space-x-2 mb-2">
                        <span className={`px-2 py-1 text-xs font-semibold rounded-full ${getPriorityColor(task.priority)}`}>
                          {task.priority}
                        </span>
                        <span className={`px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(task.status)}`}>
                          {task.status}
                        </span>
                      </div>
                      <p className="text-sm text-gray-900 mb-2">{task.description}</p>
                      {task.dueTime && (
                        <p className="text-xs text-gray-500">
                          Due: {new Date(task.dueTime).toLocaleString()}
                        </p>
                      )}
                    </div>
                    <div className="flex space-x-2">
                      {task.status === 'Pending' && (
                        <button
                          onClick={() => handleStatusChange(task.id, 'InProgress')}
                          className="px-3 py-1 text-xs bg-blue-600 text-white rounded hover:bg-blue-700"
                        >
                          Start
                        </button>
                      )}
                      {task.status === 'InProgress' && (
                        <button
                          onClick={() => handleStatusChange(task.id, 'Completed')}
                          className="px-3 py-1 text-xs bg-green-600 text-white rounded hover:bg-green-700"
                        >
                          Complete
                        </button>
                      )}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="bg-white shadow rounded-lg p-12 text-center text-gray-500">
              No tasks assigned.
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

