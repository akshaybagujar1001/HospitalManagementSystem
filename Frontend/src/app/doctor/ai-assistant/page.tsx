'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuthStore } from '@/store/authStore';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import api from '@/lib/api';
import Link from 'next/link';

interface Patient {
  id: number;
  firstName: string;
  lastName: string;
}

interface AIDiagnosisResponse {
  id: number;
  suspectedDiagnosis: string;
  recommendedTests: string;
  riskLevel: string;
  confidence: number;
  createdAt: string;
}

export default function AIAssistantPage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { user, isAuthenticated, logout } = useAuthStore();
  const [formData, setFormData] = useState({
    patientId: '',
    symptoms: '',
    vitalSigns: '',
  });

  useEffect(() => {
    if (!isAuthenticated || user?.role !== 'Doctor') {
      router.push('/login');
    }
  }, [isAuthenticated, user, router]);

  const { data: patients } = useQuery<Patient[]>({
    queryKey: ['patients'],
    queryFn: async () => {
      const response = await api.get('/patients');
      return response.data;
    },
    enabled: isAuthenticated && user?.role === 'Doctor',
  });

  const analyzeMutation = useMutation({
    mutationFn: async (data: any) => {
      const response = await api.post('/ai-diagnosis/analyze', {
        patientId: parseInt(data.patientId),
        doctorId: user?.id,
        symptoms: data.symptoms,
        vitalSigns: data.vitalSigns || null,
      });
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ai-diagnoses'] });
      alert('Analysis completed successfully!');
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.patientId || !formData.symptoms) {
      alert('Please fill in all required fields');
      return;
    }
    analyzeMutation.mutate(formData);
  };

  if (!isAuthenticated || user?.role !== 'Doctor') {
    return null;
  }

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center space-x-4">
              <Link href="/doctor/dashboard" className="text-primary-600 hover:text-primary-700">
                ← Back to Dashboard
              </Link>
              <h1 className="text-xl font-semibold">AI Diagnostic Assistant</h1>
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
                <label className="block text-sm font-medium text-gray-700">Select Patient *</label>
                <select
                  required
                  value={formData.patientId}
                  onChange={(e) => setFormData({ ...formData, patientId: e.target.value })}
                  className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                >
                  <option value="">Choose a patient</option>
                  {patients?.map((patient) => (
                    <option key={patient.id} value={patient.id}>
                      {patient.firstName} {patient.lastName}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Symptoms *</label>
                <textarea
                  required
                  rows={6}
                  value={formData.symptoms}
                  onChange={(e) => setFormData({ ...formData, symptoms: e.target.value })}
                  placeholder="Describe patient symptoms in detail..."
                  className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Vital Signs (Optional)</label>
                <textarea
                  rows={3}
                  value={formData.vitalSigns}
                  onChange={(e) => setFormData({ ...formData, vitalSigns: e.target.value })}
                  placeholder='JSON format: {"temperature": "98.6", "bloodPressure": "120/80", "heartRate": "72"}'
                  className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500"
                />
              </div>

              <div className="flex justify-end">
                <button
                  type="submit"
                  disabled={analyzeMutation.isPending}
                  className="px-6 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50"
                >
                  {analyzeMutation.isPending ? 'Analyzing...' : 'Analyze Symptoms'}
                </button>
              </div>
            </form>

            {analyzeMutation.data && (
              <div className="mt-6 p-4 bg-blue-50 border border-blue-200 rounded-lg">
                <h3 className="text-lg font-semibold mb-2">AI Analysis Results</h3>
                <div className="space-y-2">
                  <p><strong>Suspected Diagnosis:</strong> {analyzeMutation.data.suspectedDiagnosis}</p>
                  <p><strong>Recommended Tests:</strong> {analyzeMutation.data.recommendedTests}</p>
                  <p><strong>Risk Level:</strong> 
                    <span className={`ml-2 px-2 py-1 text-xs font-semibold rounded-full ${
                      analyzeMutation.data.riskLevel === 'High' ? 'bg-red-100 text-red-800' :
                      analyzeMutation.data.riskLevel === 'Medium' ? 'bg-yellow-100 text-yellow-800' :
                      'bg-green-100 text-green-800'
                    }`}>
                      {analyzeMutation.data.riskLevel}
                    </span>
                  </p>
                  <p><strong>Confidence:</strong> {(analyzeMutation.data.confidence * 100).toFixed(1)}%</p>
                </div>
              </div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}

