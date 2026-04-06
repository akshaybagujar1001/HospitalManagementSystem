# Si të kontrollosh nëse Backend është Running

## Hapi 1: Kontrollo Terminal Output

Kur ekzekuton `dotnet run`, duhet të shohësh:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

**Nëse nuk shohësh këto mesazhe:**
- Backend nuk ka startuar
- Shiko për gabime në terminal
- Kontrollo që database është accessible

## Hapi 2: Testo në Browser

### HTTP (Port 5001):
```
http://localhost:5001
```

### HTTPS (Port 7001):
```
https://localhost:7001
```

**Nëse shfaqet "ERR_CONNECTION_REFUSED":**
- Backend nuk është running
- Ose portet janë të bllokuara
- Ose ka një gabim që ndalon startup

## Hapi 3: Kontrollo nëse Portet janë në Përdorim

### Windows PowerShell:
```powershell
netstat -ano | findstr :5001
netstat -ano | findstr :7001
```

Nëse shfaqet diçka, porti është në përdorim.

## Hapi 4: Kontrollo Database Connection

Gabimet më të zakonshme:
1. SQL Server nuk është running
2. Connection string është gabim
3. Nuk ka permissions për të krijuar database

## Hapi 5: Testo me Swagger

Nëse backend është running, duhet të shohësh Swagger UI në:
```
http://localhost:5001
```

## Troubleshooting

### Problem: Backend starton por nuk hapet në browser

**Zgjidhje:**
1. Kontrollo firewall settings
2. Provo me `http://127.0.0.1:5001` në vend të `localhost:5001`
3. Kontrollo që nuk ke antivirus që bllokon portet

### Problem: "Cannot create database"

**Zgjidhje:**
1. Verifiko që SQL Server është running
2. Kontrollo connection string
3. Sigurohu që ke permissions

### Problem: Backend crash-on startup

**Zgjidhje:**
1. Shiko terminal për exception messages
2. Kontrollo logs
3. Verifiko që të gjitha packages janë restore

## Quick Test

Hap PowerShell dhe ekzekuto:
```powershell
# Test HTTP
Invoke-WebRequest -Uri http://localhost:5001 -UseBasicParsing

# Test HTTPS (do të kërkojë certificate)
Invoke-WebRequest -Uri https://localhost:7001 -UseBasicParsing -SkipCertificateCheck
```

Nëse kthen status 200, backend është running!

