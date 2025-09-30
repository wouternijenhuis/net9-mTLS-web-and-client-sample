<#
.SYNOPSIS
    Generate SSL/TLS certificates for mutual TLS (mTLS) demonstration.

.DESCRIPTION
    This script generates a Certificate Authority (CA), server certificate, and client certificate
    using OpenSSL. The certificates are created in PKCS#12 (.pfx) format for use with .NET applications.

.NOTES
    Prerequisites:
    - OpenSSL must be installed and available in PATH
    - Run this script from the repository root directory
    
    The script generates the following files in the 'certificates' directory:
    - ca.crt, ca.key: Certificate Authority
    - server.crt, server.key, server.pfx: Server certificate
    - client.crt, client.key, client.pfx: Client certificate
    
    All certificates use password: "password"
#>

Write-Host "=== mTLS Certificate Generation Script ===" -ForegroundColor Cyan
Write-Host ""

# Check if OpenSSL is available
try {
    $null = & openssl version 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "OpenSSL command failed"
    }
    Write-Host "OpenSSL is available" -ForegroundColor Green
} catch {
    Write-Host "Error: OpenSSL is not installed or not in PATH" -ForegroundColor Red
    Write-Host "  Please install OpenSSL from: https://www.openssl.org/" -ForegroundColor Yellow
    exit 1
}

# Create certificates directory if it doesn't exist
$certDir = Join-Path $PSScriptRoot "certificates"
if (-not (Test-Path $certDir)) {
    New-Item -ItemType Directory -Path $certDir | Out-Null
    Write-Host "Created certificates directory" -ForegroundColor Green
} else {
    Write-Host "Certificates directory exists" -ForegroundColor Green
}

Push-Location $certDir

Write-Host ""
Write-Host "Generating certificates..." -ForegroundColor Cyan

# Certificate configuration
$password = "password"

# Function to run OpenSSL commands
function Invoke-OpenSSL {
    param(
        [string]$Arguments,
        [string]$Description
    )
    
    Write-Host "  → $Description" -ForegroundColor Gray
    $cmd = "openssl $Arguments"
    Invoke-Expression $cmd
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed: $Description" -ForegroundColor Red
        Pop-Location
        exit 1
    }
}

# 1. Generate CA private key and certificate
Write-Host ""
Write-Host "1. Generating Certificate Authority (CA)..." -ForegroundColor Yellow
if (-not (Test-Path "ca.key") -or -not (Test-Path "ca.crt")) {
    Invoke-OpenSSL "genrsa -out ca.key 4096" "Generate CA private key"
    Invoke-OpenSSL "req -new -x509 -days 3650 -key ca.key -out ca.crt -subj '/C=US/ST=CA/O=MtlsDemo/CN=MtlsDemo-CA'" "Generate CA certificate"
    Write-Host "CA certificate created" -ForegroundColor Green
} else {
    Write-Host "CA certificate already exists, skipping..." -ForegroundColor Gray
}

# 2. Generate Server certificate
Write-Host ""
Write-Host "2. Generating Server certificate..." -ForegroundColor Yellow
if (-not (Test-Path "server.key") -or -not (Test-Path "server.crt")) {
    Invoke-OpenSSL "genrsa -out server.key 4096" "Generate server private key"
    Invoke-OpenSSL "req -new -key server.key -out server.csr -subj '/C=US/ST=CA/O=MtlsDemo/CN=localhost'" "Generate server CSR"
    Invoke-OpenSSL "x509 -req -in server.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out server.crt -days 365 -sha256" "Sign server certificate"
    Write-Host "Server certificate created" -ForegroundColor Green
} else {
    Write-Host "Server certificate already exists, skipping..." -ForegroundColor Gray
}

# Generate server.pfx
Write-Host "  → Generating server.pfx..." -ForegroundColor Gray
Invoke-OpenSSL "pkcs12 -export -out server.pfx -inkey server.key -in server.crt -certfile ca.crt -passout pass:$password" "Export server certificate to PFX"
Write-Host "  server.pfx created" -ForegroundColor Green

# 3. Generate Client certificate
Write-Host ""
Write-Host "3. Generating Client certificate..." -ForegroundColor Yellow
if (-not (Test-Path "client.key") -or -not (Test-Path "client.crt")) {
    Invoke-OpenSSL "genrsa -out client.key 4096" "Generate client private key"
    Invoke-OpenSSL "req -new -key client.key -out client.csr -subj '/C=US/ST=CA/O=MtlsDemo/CN=client'" "Generate client CSR"
    Invoke-OpenSSL "x509 -req -in client.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out client.crt -days 365 -sha256" "Sign client certificate"
    Write-Host "  Client certificate created" -ForegroundColor Green
} else {
    Write-Host "  Client certificate already exists, skipping..." -ForegroundColor Gray
}

# Generate client.pfx
Write-Host "  → Generating client.pfx..." -ForegroundColor Gray
Invoke-OpenSSL "pkcs12 -export -out client.pfx -inkey client.key -in client.crt -certfile ca.crt -passout pass:$password" "Export client certificate to PFX"
Write-Host "client.pfx created" -ForegroundColor Green

Pop-Location

Write-Host ""
Write-Host "=== Certificate Generation Complete ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Generated files in 'certificates' directory:" -ForegroundColor Green
Write-Host "  - ca.crt, ca.key          (Certificate Authority)" -ForegroundColor White
Write-Host "  - server.crt, server.key, server.pfx  (Server certificate)" -ForegroundColor White
Write-Host "  - client.crt, client.key, client.pfx  (Client certificate)" -ForegroundColor White
Write-Host ""
Write-Host "Certificate password: $password" -ForegroundColor Yellow
Write-Host ""
Write-Host "You can now run the application:" -ForegroundColor Cyan
Write-Host "  dotnet build MtlsDemo.sln" -ForegroundColor White
Write-Host "  ./test-local.ps1" -ForegroundColor White
Write-Host ""
