'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';

export default function Home() {
  const router = useRouter();
  const { isAuthenticated, user } = useAuthStore();

  useEffect(() => {
    if (isAuthenticated) {
      // Redirect based on role
      switch (user?.role) {
        case 'Admin':
          router.push('/admin/dashboard');
          break;
        case 'Doctor':
          router.push('/doctor/dashboard');
          break;
        case 'Nurse':
          router.push('/nurse/dashboard');
          break;
        case 'Patient':
          router.push('/patient/dashboard');
          break;
        default:
          router.push('/login');
      }
    } else {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="text-center">
        <h1 className="text-4xl font-bold mb-4">Hospital Management System</h1>
        <p className="text-gray-600">Loading...</p>
      </div>
    </div>
  );
}

