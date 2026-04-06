#!/bin/bash

echo "========================================"
echo "  Hospital Management System - Backend"
echo "========================================"
echo ""

cd "$(dirname "$0")/HMS.API"

echo "[1/3] Restoring NuGet packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "ERROR: Failed to restore packages"
    exit 1
fi

echo ""
echo "[2/3] Building project..."
dotnet build
if [ $? -ne 0 ]; then
    echo "ERROR: Build failed"
    exit 1
fi

echo ""
echo "[3/3] Starting backend server..."
echo ""
echo "Backend will be available at:"
echo "  - HTTP:  http://localhost:5001"
echo "  - HTTPS: https://localhost:7001"
echo "  - Swagger: http://localhost:5001"
echo ""
echo "Press Ctrl+C to stop the server"
echo ""

dotnet run

