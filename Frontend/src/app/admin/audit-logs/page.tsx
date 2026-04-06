'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface AuditLog {
  id: number;
  userId?: number;
  action: string;
  entityName: string;
  entityId?: number;
  timestamp: string;
  description?: string;
}

export default function AuditLogsPage() {
  const router = useRouter();
  const { user, isAuthenticated, logout } = useAuthStore();
  const [page, setPage] = useState(1);
  const [entityName, setEntityName] = useState('');

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Admin') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: logsData, isLoading } = useQuery({
    queryKey: ['audit-logs', page, entityName],
    queryFn: async () => {
      const params = new URLSearchParams({
        page: page.toString(),
        pageSize: '50',
      });
      if (entityName) params.append('entityName', entityName);
      
      const response = await api.get(`/audit-logs?${params}`);
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Admin',
  });

  if (!isAuthenticated || user?.role !== 'Admin') {
    return null;
  }

  const getActionColor = (action: string) => {
    switch (action) {
      case 'Added': return 'bg-green-100 text-green-800';
      case 'Modified': return 'bg-blue-100 text-blue-800';
      case 'Deleted': return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/admin/dashboard" className="text-primary-600 hover:text-primary-700">
                ← Back to Dashboard
              </Link>
              <h1 className="text-xl font-semibold">Audit Logs</h1>
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
          <div className="mb-4 flex justify-between items-center">
            <h2 className="text-2xl font-bold">Activity Log</h2>
            <div className="flex space-x-4">
              <input
                type="text"
                placeholder="Filter by entity..."
                value={entityName}
                onChange={(e) => setEntityName(e.target.value)}
                className="px-3 py-2 border border-gray-300 rounded-md"
              />
            </div>
          </div>

          {isLoading ? (
            <div className="text-center py-12">Loading...</div>
          ) : logsData?.data && logsData.data.length > 0 ? (
            <>
              <div className="bg-white shadow overflow-hidden sm:rounded-md">
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Timestamp</th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Action</th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Entity</th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Entity ID</th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">User ID</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {logsData.data.map((log: AuditLog) => (
                      <tr key={log.id}>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                          {new Date(log.timestamp).toLocaleString()}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${getActionColor(log.action)}`}>
                            {log.action}
                          </span>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {log.entityName}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                          {log.entityId || '-'}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                          {log.userId || 'System'}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              <div className="mt-4 flex justify-between items-center">
                <p className="text-sm text-gray-500">
                  Showing page {page} of {logsData.totalPages || 1}
                </p>
                <div className="flex space-x-2">
                  <button
                    onClick={() => setPage(p => Math.max(1, p - 1))}
                    disabled={page === 1}
                    className="px-4 py-2 border border-gray-300 rounded-md disabled:opacity-50"
                  >
                    Previous
                  </button>
                  <button
                    onClick={() => setPage(p => p + 1)}
                    disabled={page >= (logsData.totalPages || 1)}
                    className="px-4 py-2 border border-gray-300 rounded-md disabled:opacity-50"
                  >
                    Next
                  </button>
                </div>
              </div>
            </>
          ) : (
            <div className="bg-white shadow rounded-lg p-12 text-center text-gray-500">
              No audit logs found.
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

