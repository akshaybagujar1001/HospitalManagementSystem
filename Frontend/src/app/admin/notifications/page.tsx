'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '@/lib/api';
import * as signalR from '@microsoft/signalr';

interface Notification {
  id: number;
  title: string;
  message: string;
  type: string;
  isRead: boolean;
  createdAt: string;
  relatedEntityType?: string;
  relatedEntityId?: number;
}

export default function NotificationsPage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { user, isAuthenticated, logout } = useAuthStore();
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  useEffect(() => {
    if (!isAuthenticated) return;

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000'}/notificationHub`, {
        accessTokenFactory: () => {
          const token = document.cookie
            .split('; ')
            .find(row => row.startsWith('token='))
            ?.split('=')[1] || '';
          return token;
        }
      })
      .build();

    newConnection.on('ReceiveNotification', () => {
      queryClient.invalidateQueries({ queryKey: ['notifications'] });
      queryClient.invalidateQueries({ queryKey: ['unread-count'] });
    });

    newConnection.start().catch(err => console.error('SignalR Connection Error: ', err));
    setConnection(newConnection);

    return () => {
      newConnection.stop();
    };
  }, [isAuthenticated, queryClient]);

  const { data: notifications, isLoading } = useQuery<Notification[]>({
    queryKey: ['notifications'],
    queryFn: async () => {
      const response = await api.get('/notifications');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Admin',
  });

  const { data: unreadCount } = useQuery({
    queryKey: ['unread-count'],
    queryFn: async () => {
      const response = await api.get('/notifications/unread-count');
      return response.data.count;
    },
    enabled: isAuthenticated && user?.role === 'Admin',
  });

  const markAsReadMutation = useMutation({
    mutationFn: async (id: number) => {
      return await api.put(`/notifications/${id}/read`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['notifications'] });
      queryClient.invalidateQueries({ queryKey: ['unread-count'] });
    },
  });

  const markAllAsReadMutation = useMutation({
    mutationFn: async () => {
      return await api.put('/notifications/read-all');
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['notifications'] });
      queryClient.invalidateQueries({ queryKey: ['unread-count'] });
    },
  });

  if (!isAuthenticated || user?.role !== 'Admin') {
    return null;
  }

  const getTypeColor = (type: string) => {
    switch (type) {
      case 'Appointment': return 'bg-blue-100 text-blue-800';
      case 'LabResult': return 'bg-green-100 text-green-800';
      case 'Prescription': return 'bg-purple-100 text-purple-800';
      case 'Billing': return 'bg-yellow-100 text-yellow-800';
      case 'Emergency': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <h1 className="text-xl font-semibold">Notifications</h1>
              {unreadCount > 0 && (
                <span className="ml-2 px-2 py-1 text-xs font-semibold rounded-full bg-red-600 text-white">
                  {unreadCount} unread
                </span>
              )}
            </div>
            <div className="flex items-center space-x-4">
              <button
                onClick={() => markAllAsReadMutation.mutate()}
                className="px-4 py-2 text-sm text-primary-600 hover:text-primary-700"
                disabled={markAllAsReadMutation.isPending}
              >
                Mark All Read
              </button>
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
            <div className="text-center py-12">Loading notifications...</div>
          ) : notifications && notifications.length > 0 ? (
            <div className="bg-white shadow overflow-hidden sm:rounded-md">
              <ul className="divide-y divide-gray-200">
                {notifications.map((notification) => (
                  <li
                    key={notification.id}
                    className={`px-6 py-4 hover:bg-gray-50 cursor-pointer ${!notification.isRead ? 'bg-blue-50' : ''}`}
                    onClick={() => !notification.isRead && markAsReadMutation.mutate(notification.id)}
                  >
                    <div className="flex items-center justify-between">
                      <div className="flex-1">
                        <div className="flex items-center">
                          <span className={`px-2 py-1 text-xs font-semibold rounded-full ${getTypeColor(notification.type)}`}>
                            {notification.type}
                          </span>
                          {!notification.isRead && (
                            <span className="ml-2 w-2 h-2 bg-blue-600 rounded-full"></span>
                          )}
                        </div>
                        <p className="mt-1 text-sm font-medium text-gray-900">{notification.title}</p>
                        <p className="mt-1 text-sm text-gray-500">{notification.message}</p>
                        <p className="mt-1 text-xs text-gray-400">
                          {new Date(notification.createdAt).toLocaleString()}
                        </p>
                      </div>
                    </div>
                  </li>
                ))}
              </ul>
            </div>
          ) : (
            <div className="bg-white shadow rounded-lg p-12 text-center text-gray-500">
              No notifications found.
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

