# Troubleshooting Guide - Registration and Login Issues

## Common Issues and Solutions

### 1. "Cannot connect to server" Error

**Problem**: Frontend cannot reach the backend API.

**Solutions**:
- ✅ Make sure the backend is running on `http://localhost:5001` or `https://localhost:7001`
- ✅ Check if the API URL in `.env.local` matches your backend URL:
  ```
  NEXT_PUBLIC_API_URL=http://localhost:5001/api
  ```
- ✅ Verify the backend is accessible by opening `http://localhost:5001` in your browser (should show Swagger UI)
- ✅ Check if there are any firewall or antivirus blocking the connection

### 2. "Registration failed" or "Login failed" Error

**Problem**: Backend is receiving requests but returning errors.

**Solutions**:
- ✅ Check browser console (F12) for detailed error messages
- ✅ Check backend logs for error details
- ✅ Verify database connection string in `appsettings.json`
- ✅ Make sure SQL Server is running
- ✅ Check if database exists and is accessible

### 3. CORS Errors

**Problem**: Browser blocks requests due to CORS policy.

**Solutions**:
- ✅ CORS is already configured in `Program.cs` with `AllowAll` policy
- ✅ If still having issues, check browser console for specific CORS errors
- ✅ Make sure backend is running before frontend

### 4. Validation Errors

**Problem**: Form validation fails.

**Solutions**:
- ✅ Make sure all required fields are filled
- ✅ Password must be at least 6 characters
- ✅ Email must be in valid format
- ✅ Passwords must match
- ✅ Phone number should be provided

### 5. Database Connection Issues

**Problem**: Cannot connect to SQL Server.

**Solutions**:
- ✅ Verify SQL Server is running
- ✅ Check connection string in `appsettings.json`:
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HMSDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
  ```
- ✅ For SQL Server Express, use: `Server=localhost\\SQLEXPRESS;...`
- ✅ Make sure you have permissions to create databases

### 6. "User with this email already exists"

**Problem**: Trying to register with an email that's already in use.

**Solutions**:
- ✅ Use a different email address
- ✅ Or login with existing credentials

## Step-by-Step Debugging

### Backend Debugging

1. **Check if backend is running**:
   ```bash
   cd Backend/HMS.API
   dotnet run
   ```
   Should see: "Now listening on: http://localhost:5001"

2. **Test API directly**:
   - Open `http://localhost:5001` (Swagger UI)
   - Try the `/api/auth/register` endpoint
   - Check the response

3. **Check logs**:
   - Backend logs will show detailed error messages
   - Look for exceptions or validation errors

### Frontend Debugging

1. **Check browser console** (F12):
   - Look for network errors
   - Check API request/response
   - Look for JavaScript errors

2. **Check API URL**:
   - Verify `.env.local` file exists
   - Check `NEXT_PUBLIC_API_URL` value
   - Make sure it matches backend URL

3. **Test API connection**:
   ```javascript
   // In browser console
   fetch('http://localhost:5001/api/auth/login', {
     method: 'POST',
     headers: { 'Content-Type': 'application/json' },
     body: JSON.stringify({ email: 'admin@hms.com', password: 'Admin@123' })
   }).then(r => r.json()).then(console.log)
   ```

## Quick Fixes

### Reset Database
```bash
# Delete database and recreate
# In SQL Server Management Studio, delete HMSDB database
# Then run backend again - it will recreate automatically
```

### Clear Browser Cache
- Clear browser cache and cookies
- Or use incognito/private mode

### Restart Services
1. Stop backend (Ctrl+C)
2. Stop frontend (Ctrl+C)
3. Restart backend
4. Restart frontend

## Default Test Credentials

After seeding, you can use:
- **Admin**: admin@hms.com / Admin@123
- **Doctor**: doctor@hms.com / Doctor@123
- **Nurse**: nurse@hms.com / Nurse@123
- **Patient**: patient@hms.com / Patient@123

## Still Having Issues?

1. Check all error messages in:
   - Browser console (F12)
   - Backend terminal/logs
   - Network tab in browser DevTools

2. Verify:
   - Backend is running
   - Frontend is running
   - Database is accessible
   - Ports are not blocked

3. Try:
   - Using different browser
   - Clearing all cookies
   - Restarting both services
   - Checking firewall settings

