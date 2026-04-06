# Backend Server Information

## 🌐 Server URLs

### Development Environment

**HTTP (Default):**
```
http://localhost:5001
```

**HTTPS:**
```
https://localhost:7001
```

**API Base URL:**
```
http://localhost:5001/api
```
ose
```
https://localhost:7001/api
```

### Swagger UI

Pas startimit të backend, Swagger është i disponueshëm në:
```
http://localhost:5001
```
ose
```
https://localhost:7001
```

## 📋 Konfigurimi

### 1. Port Configuration
Konfigurimi i portit gjendet në:
- `Backend/HMS.API/Properties/launchSettings.json`

```json
{
  "http": {
    "applicationUrl": "http://localhost:5001"
  },
  "https": {
    "applicationUrl": "https://localhost:7001;http://localhost:5001"
  }
}
```

### 2. Frontend Configuration
Frontend përdor këtë URL për API calls:
- `Frontend/src/lib/api.ts`

```typescript
baseURL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5001/api'
```

Mund të ndryshosh URL-në duke krijuar një `.env.local` file në `Frontend/`:
```env
NEXT_PUBLIC_API_URL=http://localhost:5001/api
```

## 🗄️ Database Configuration

**Connection String:**
```
Server=localhost;Database=HMSDB;Trusted_Connection=True;TrustServerCertificate=True;
```

- **Server:** `localhost` (SQL Server lokal)
- **Database:** `HMSDB`
- **Authentication:** Windows Authentication (Trusted_Connection=True)

## 🚀 Si të startosh Backend

### Metoda 1: Visual Studio / Rider
1. Hap `Backend/HMS.API/HMS.API.csproj`
2. Run projektin (F5)

### Metoda 2: Command Line
```bash
cd Backend/HMS.API
dotnet run
```

### Metoda 3: Batch Script (Windows)
```bash
cd Backend
run-backend.bat
```

## ✅ Verifikimi

Pas startimit, verifiko që backend po funksionon:

1. **Swagger UI:**
   - Shko në: `http://localhost:5001`
   - Duhet të shohësh Swagger documentation

2. **Health Check:**
   - Shko në: `http://localhost:5001/api/auth/test` (nëse ekziston)
   - Ose: `http://localhost:5001/swagger`

3. **Frontend Connection:**
   - Starto frontend: `cd Frontend && npm run dev`
   - Frontend do të lidhet automatikisht me `http://localhost:5001/api`

## 🔧 Ndryshimi i Portit

Nëse porti 5001 ose 7001 janë të zënë, mund t'i ndryshosh:

1. **Ndrysho `launchSettings.json`:**
```json
"applicationUrl": "http://localhost:5002"
```

2. **Ndrysho frontend `api.ts` ose `.env.local`:**
```typescript
baseURL: 'http://localhost:5002/api'
```

3. **Restart backend dhe frontend**

## 📝 Shënime

- Backend duhet të jetë i startuar para frontend
- Nëse përdor HTTPS, mund të duhet të pranosh certificate warning në browser
- Për production, duhet të konfigurosh CORS dhe security settings në `Program.cs`

