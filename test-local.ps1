#!/usr/bin/env pwsh

Write-Host "=== Testing Mutual TLS Setup Locally ==="

# Build the solution
Write-Host "Building solution..."
dotnet build MtlsDemo.sln

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!"
    exit 1
}

# Start the server in background
Write-Host "Starting server..."
Push-Location MtlsServer
$serverProcess = Start-Process -FilePath "dotnet" -ArgumentList "run" -PassThru -NoNewWindow
Pop-Location

# Wait for server to start
Write-Host "Waiting for server to start..."
Start-Sleep -Seconds 5

# Run the client
Write-Host "Running client..."
Push-Location MtlsClient
dotnet run
$clientExitCode = $LASTEXITCODE
Pop-Location

# Stop the server
Write-Host "Stopping server..."
if ($serverProcess -and !$serverProcess.HasExited) {
    Stop-Process -Id $serverProcess.Id -Force -ErrorAction SilentlyContinue
}

if ($clientExitCode -eq 0) {
    Write-Host "=== Test completed successfully! ==="
} else {
    Write-Host "=== Test failed! ==="
}

exit $clientExitCode
