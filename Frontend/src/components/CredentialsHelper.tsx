'use client';

import { useState } from 'react';

export default function CredentialsHelper() {
  const [showCredentials, setShowCredentials] = useState(false);

  if (process.env.NODE_ENV === 'production') {
    return null; // Don't show in production
  }

  return (
    <div className="mt-4 p-4 bg-blue-50 border border-blue-200 rounded-md">
      <button
        onClick={() => setShowCredentials(!showCredentials)}
        className="text-sm text-blue-600 hover:text-blue-800 font-medium"
      >
        {showCredentials ? 'Hide' : 'Show'} Default Test Credentials
      </button>
      
      {showCredentials && (
        <div className="mt-3 space-y-2 text-sm">
          <div>
            <strong>Admin:</strong> admin@hms.com / Admin@123
          </div>
          <div>
            <strong>Doctor:</strong> doctor@hms.com / Doctor@123
          </div>
          <div>
            <strong>Nurse:</strong> nurse@hms.com / Nurse@123
          </div>
          <div>
            <strong>Patient:</strong> patient@hms.com / Patient@123
          </div>
        </div>
      )}
    </div>
  );
}

