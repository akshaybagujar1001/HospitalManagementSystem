# Backend Setup Guide - Si të ekzekutosh Backend

## Hapat për të ekzekutuar Backend

### 1. Kontrollo Prerequisites

Para se të fillosh, sigurohu që ke:
- ✅ .NET 8 SDK i instaluar
- ✅ SQL Server i instaluar dhe running
- ✅ Visual Studio Code ose Visual Studio (opsionale)

**Kontrollo .NET version:**
```bash
dotnet --version
```
Duhet të shfaqë: `8.0.x` ose më lart

Nëse nuk ke .NET 8, shkarko nga: https://dotnet.microsoft.com/download/dotnet/8.0

### 2. Hapi 1: Hap Terminal/Command Prompt

- **Windows**: Hap PowerShell ose Command Prompt
- **Mac/Linux**: Hap Terminal

### 3. Hapi 2: Navigo te Backend folder

```bash
cd "C:\Users\Flori\Desktop\Hospital management system\Backend\HMS.API"
```

Ose nëse je tashmë në root folder:
```bash
cd Backend/HMS.API
```

### 4. Hapi 3: Restore Dependencies

```bash
dotnet restore
```

Kjo do të shkarkojë të gjitha pakot e nevojshme.

### 5. Hapi 4: Kontrollo Connection String

Hap file `appsettings.json` dhe kontrollo connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HMSDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Nëse ke SQL Server Express:**
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=HMSDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

**Nëse ke SQL Server me password:**
```json
"DefaultConnection": "Server=localhost;Database=HMSDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
```

### 6. Hapi 5: Build Project

```bash
dotnet build
```

Kjo do të kompilojë projektin. Nëse ka gabime, do t'i shohësh këtu.

### 7. Hapi 6: Run Backend

```bash
dotnet run
```

Ose për të ekzekutuar në background:
```bash
dotnet run &
```

### 8. Hapi 7: Verifiko që Backend është Running

Pas ekzekutimit, duhet të shohësh diçka si:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 9. Testo Backend

Hap browser dhe shko te:
- **Swagger UI**: http://localhost:5001 ose https://localhost:7001
- **API**: http://localhost:5001/api

Nëse shfaqet Swagger UI, backend është running saktë!

## Troubleshooting

### Problem: "dotnet command not found"

**Zgjidhje**: .NET SDK nuk është instaluar ose nuk është në PATH.
- Shkarko dhe instalo .NET 8 SDK
- Restart terminal/command prompt

### Problem: "Cannot connect to database"

**Zgjidhje**: 
1. Kontrollo që SQL Server është running
2. Verifiko connection string në `appsettings.json`
3. Testo connection me SQL Server Management Studio

### Problem: "Port 5001 is already in use"

**Zgjidhje**: 
1. Gjej procesin që përdor portin:
   ```bash
   # Windows
   netstat -ano | findstr :5001
   
   # Mac/Linux
   lsof -i :5001
   ```
2. Ndal procesin ose ndrysho port në `appsettings.json`:
   ```json
   "Urls": "http://localhost:5002"
   ```

### Problem: "Build failed"

**Zgjidhje**:
1. Kontrollo që të gjitha projektet janë restore:
   ```bash
   dotnet restore
   ```
2. Kontrollo që ke .NET 8 SDK:
   ```bash
   dotnet --version
   ```
3. Fshi `bin` dhe `obj` folders dhe provo përsëri:
   ```bash
   dotnet clean
   dotnet build
   ```

### Problem: "Database does not exist"

**Zgjidhje**: 
- Database do të krijohet automatikisht kur ekzekuton backend për herë të parë
- Nëse nuk krijohet, kontrollo që ke permissions për të krijuar database

## Quick Start Script

Krijo një file `run-backend.bat` (Windows) ose `run-backend.sh` (Mac/Linux):

**Windows (run-backend.bat):**
```batch
@echo off
cd Backend\HMS.API
echo Restoring packages...
dotnet restore
echo Building project...
dotnet build
echo Starting backend...
dotnet run
pause
```

**Mac/Linux (run-backend.sh):**
```bash
#!/bin/bash
cd Backend/HMS.API
echo "Restoring packages..."
dotnet restore
echo "Building project..."
dotnet build
echo "Starting backend..."
dotnet run
```

Pastaj thjesht ekzekuto:
- Windows: `run-backend.bat`
- Mac/Linux: `chmod +x run-backend.sh && ./run-backend.sh`

## Tips

1. **Keep Terminal Open**: Mos e mbyll terminalin kur backend është running
2. **Check Logs**: Logs do të shfaqen në terminal për debugging
3. **Ctrl+C**: Për të ndalur backend, shtyp Ctrl+C në terminal
4. **Hot Reload**: Ndryshimet në kod do të reload automatikisht në development mode

## Next Steps

Pas që backend është running:
1. ✅ Verifiko që Swagger UI është accessible
2. ✅ Testo login me credentials default:
   - Email: `admin@hms.com`
   - Password: `Admin@123`
3. ✅ Start frontend në terminal tjetër
4. ✅ Testo regjistrim dhe login nga frontend

