# Deployment Guide

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Backend Deployment](#backend-deployment)
3. [Frontend Deployment](#frontend-deployment)
4. [Database Setup](#database-setup)
5. [Environment Configuration](#environment-configuration)
6. [CI/CD Pipeline](#cicd-pipeline)
7. [Production Checklist](#production-checklist)

## Prerequisites

### Backend Requirements
- .NET 8 SDK
- SQL Server 2019 or later (or Azure SQL Database)
- IIS or Kestrel web server
- Windows Server or Linux server

### Frontend Requirements
- Node.js 18+ and npm/yarn
- Web server (Nginx, Apache, or hosting service like Vercel/Netlify)

### Database Requirements
- SQL Server 2019 or later
- Or Azure SQL Database
- Or SQL Server Express (for development)

## Backend Deployment

### Option 1: IIS (Windows Server)

1. **Publish the Application**
```bash
cd Backend/HMS.API
dotnet publish -c Release -o ./publish
```

2. **Create IIS Application Pool**
   - Open IIS Manager
   - Create a new Application Pool targeting .NET CLR Version "No Managed Code"
   - Set Identity to appropriate account

3. **Create IIS Website**
   - Right-click Sites → Add Website
   - Set physical path to published folder
   - Bind to appropriate port (e.g., 80, 443)
   - Select the application pool created above

4. **Configure appsettings.Production.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=HMSDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_PRODUCTION_SECRET_KEY_AT_LEAST_32_CHARACTERS",
    "Issuer": "HMS",
    "Audience": "HMS",
    "ExpiryMinutes": "1440"
  }
}
```

### Option 2: Linux with Kestrel

1. **Install .NET Runtime**
```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version 8.0.0
```

2. **Publish Application**
```bash
cd Backend/HMS.API
dotnet publish -c Release -o /var/www/hms-api
```

3. **Create systemd Service**
Create `/etc/systemd/system/hms-api.service`:
```ini
[Unit]
Description=HMS API Service
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet /var/www/hms-api/HMS.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=hms-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

4. **Start Service**
```bash
sudo systemctl enable hms-api
sudo systemctl start hms-api
sudo systemctl status hms-api
```

5. **Configure Nginx Reverse Proxy**
Create `/etc/nginx/sites-available/hms-api`:
```nginx
server {
    listen 80;
    server_name api.yourdomain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Enable and restart Nginx:
```bash
sudo ln -s /etc/nginx/sites-available/hms-api /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

### Option 3: Docker

1. **Create Dockerfile**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Backend/HMS.API/HMS.API.csproj", "Backend/HMS.API/"]
COPY ["Backend/HMS.Application/HMS.Application.csproj", "Backend/HMS.Application/"]
COPY ["Backend/HMS.Domain/HMS.Domain.csproj", "Backend/HMS.Domain/"]
COPY ["Backend/HMS.Infrastructure/HMS.Infrastructure.csproj", "Backend/HMS.Infrastructure/"]
RUN dotnet restore "Backend/HMS.API/HMS.API.csproj"
COPY . .
WORKDIR "/src/Backend/HMS.API"
RUN dotnet build "HMS.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HMS.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HMS.API.dll"]
```

2. **Create docker-compose.yml**
```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: Backend/HMS.API/Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=HMSDB;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;
    depends_on:
      - db

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123!
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql

volumes:
  sqlserver_data:
```

3. **Deploy**
```bash
docker-compose up -d
```

## Frontend Deployment

### Option 1: Vercel (Recommended)

1. **Install Vercel CLI**
```bash
npm i -g vercel
```

2. **Deploy**
```bash
cd Frontend
vercel
```

3. **Configure Environment Variables**
   - Go to Vercel Dashboard
   - Project Settings → Environment Variables
   - Add `NEXT_PUBLIC_API_URL`

### Option 2: Netlify

1. **Build Configuration**
Create `netlify.toml`:
```toml
[build]
  command = "npm run build"
  publish = ".next"

[[redirects]]
  from = "/*"
  to = "/index.html"
  status = 200
```

2. **Deploy via Netlify Dashboard**
   - Connect GitHub repository
   - Set build command: `npm run build`
   - Set publish directory: `.next`
   - Add environment variable: `NEXT_PUBLIC_API_URL`

### Option 3: Self-Hosted (Nginx)

1. **Build Application**
```bash
cd Frontend
npm install
npm run build
```

2. **Copy Files**
```bash
cp -r .next /var/www/hms-frontend
cp -r public /var/www/hms-frontend
```

3. **Configure Nginx**
Create `/etc/nginx/sites-available/hms-frontend`:
```nginx
server {
    listen 80;
    server_name yourdomain.com;

    root /var/www/hms-frontend;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /_next/static {
        alias /var/www/hms-frontend/.next/static;
        expires 365d;
        add_header Cache-Control "public, immutable";
    }
}
```

## Database Setup

### SQL Server Setup

1. **Create Database**
```sql
CREATE DATABASE HMSDB;
GO

USE HMSDB;
GO
```

2. **Run Migrations**
```bash
cd Backend/HMS.API
dotnet ef database update --project ../HMS.Infrastructure
```

3. **Seed Initial Data**
The seed data will run automatically on first startup.

### Azure SQL Database

1. **Create Azure SQL Database**
   - Go to Azure Portal
   - Create SQL Database
   - Note connection string

2. **Update Connection String**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=HMSDB;Persist Security Info=False;User ID=youruser;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

## Environment Configuration

### Backend Environment Variables

**Development (appsettings.Development.json)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HMSDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "DevelopmentSecretKey",
    "Issuer": "HMS",
    "Audience": "HMS",
    "ExpiryMinutes": "1440"
  }
}
```

**Production (appsettings.Production.json)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=PROD_SERVER;Database=HMSDB;User Id=PROD_USER;Password=PROD_PASSWORD;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "PRODUCTION_SECRET_KEY_MIN_32_CHARS",
    "Issuer": "HMS",
    "Audience": "HMS",
    "ExpiryMinutes": "1440"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "noreply@hms.com",
    "FromName": "Hospital Management System"
  }
}
```

### Frontend Environment Variables

**Development (.env.local)**
```env
NEXT_PUBLIC_API_URL=http://localhost:5001/api
```

**Production (.env.production)**
```env
NEXT_PUBLIC_API_URL=https://api.yourdomain.com/api
```

## CI/CD Pipeline

### GitHub Actions Example

Create `.github/workflows/deploy.yml`:
```yaml
name: Deploy HMS

on:
  push:
    branches: [ main ]

jobs:
  deploy-backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Publish
        run: dotnet publish -c Release -o ./publish
      
      - name: Deploy to server
        uses: appleboy/scp-action@master
        with:
          host: ${{ secrets.HOST }}
          username: ${{ secrets.USERNAME }}
          key: ${{ secrets.SSH_KEY }}
          source: "./publish"
          target: "/var/www/hms-api"

  deploy-frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
      
      - name: Install dependencies
        run: cd Frontend && npm install
      
      - name: Build
        run: cd Frontend && npm run build
      
      - name: Deploy to Vercel
        uses: amondnet/vercel-action@v20
        with:
          vercel-token: ${{ secrets.VERCEL_TOKEN }}
          vercel-org-id: ${{ secrets.VERCEL_ORG_ID }}
          vercel-project-id: ${{ secrets.VERCEL_PROJECT_ID }}
          working-directory: ./Frontend
```

## Production Checklist

### Security
- [ ] Change default JWT secret key
- [ ] Use strong database passwords
- [ ] Enable HTTPS/SSL
- [ ] Configure CORS properly
- [ ] Set up firewall rules
- [ ] Enable SQL Server encryption
- [ ] Regular security updates

### Performance
- [ ] Enable database indexing
- [ ] Configure connection pooling
- [ ] Set up CDN for frontend assets
- [ ] Enable response compression
- [ ] Configure caching strategies
- [ ] Monitor performance metrics

### Monitoring
- [ ] Set up application logging
- [ ] Configure error tracking (e.g., Sentry)
- [ ] Set up health checks
- [ ] Monitor database performance
- [ ] Set up alerts for critical issues

### Backup
- [ ] Configure database backups
- [ ] Set up automated backup schedule
- [ ] Test backup restoration
- [ ] Store backups off-site

### Documentation
- [ ] Update API documentation
- [ ] Document deployment procedures
- [ ] Create runbooks for common issues
- [ ] Document environment variables

## Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Check connection string
   - Verify SQL Server is running
   - Check firewall rules
   - Verify credentials

2. **JWT Token Invalid**
   - Verify secret key matches
   - Check token expiration
   - Ensure clock synchronization

3. **CORS Errors**
   - Update CORS policy in Program.cs
   - Verify allowed origins

4. **Build Failures**
   - Check .NET SDK version
   - Verify all dependencies installed
   - Check for compilation errors

## Support

For issues or questions, please refer to the main README.md or contact the development team.

