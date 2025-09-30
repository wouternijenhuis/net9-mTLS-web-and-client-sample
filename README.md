# .NET 9 ASP.NET Core mTLS WCF Service Demo

This project demonstrates a complete implementation of mutual TLS (mTLS) authentication using .NET 9 ASP.NET Core hosting a CoreWCF service, containerized with Docker.

## Architecture

### Components
- **Server**: ASP.NET Core 9.0 application hosting CoreWCF services
- **Client**: Console application that validates mTLS functionality
- **Certificates**: Self-signed CA and certificates for server and client authentication
- **Docker**: Containerized deployment with docker-compose

### Features
- **Dual endpoints**: HTTP (8080) and HTTPS (8443) with mTLS
- **Certificate-based authentication**: Client certificates required for secure operations
- **Transport security**: TLS 1.2+ with mutual authentication
- **Comprehensive testing**: Automated client that validates both secure and insecure endpoints

## Quick Start

### Prerequisites
- .NET 9.0 SDK
- Docker and Docker Compose (optional)
- OpenSSL (for certificate generation)

### 1. Local Testing

```bash
# Build the solution
dotnet build MtlsDemo.sln

# Start the server (in one terminal)
cd MtlsServer
dotnet run

# Run the client (in another terminal)
cd MtlsClient
dotnet run
```

### 2. Using Test Script

**Linux/macOS:**
```bash
# Run the automated test script
./test-local.sh
```

**Windows (PowerShell):**
```powershell
# Run the automated test script
.\test-local.ps1
```

## Expected Output

### Server
```
CoreWCF Service is running...
HTTP endpoint: http://localhost:8080/GreetingService
HTTPS endpoint: https://localhost:8443/GreetingService
info: Now listening on: http://0.0.0.0:8080
info: Now listening on: https://0.0.0.0:8443
```

### Client
```
=== Mutual TLS WCF Client ===
Loaded client certificate: CN=client, O=MtlsDemo, S=CA, C=US

=== Testing HTTP Endpoint (without mTLS) ===
1. Testing GetGreeting via HTTP:
Response: Hello HTTP Client! Connected via HTTP. No client certificate provided.
2. Testing GetSecureInfo via HTTP:
Expected error for secure operation without certificate: Client certificate is required for this operation. Please use HTTPS endpoint.

=== Testing HTTPS Endpoint (with mTLS) ===
1. Testing GetGreeting via HTTPS with mTLS:
Response: Hello HTTPS mTLS Client! Connected via HTTPS with mutual TLS. Certificate validation successful at transport level.
2. Testing GetSecureInfo via HTTPS with mTLS:
Response: Secure operation completed successfully. Connection authenticated via mutual TLS on HTTPS endpoint.

=== All tests completed successfully! ===
```

## How It Demonstrates mTLS

### 1. Certificate Generation
- Self-signed CA certificate created
- Server certificate signed by CA for localhost
- Client certificate signed by CA
- All certificates in PKCS#12 format for .NET compatibility

### 2. Server Configuration
- **HTTP endpoint (8080)**: No certificate required, basic operations allowed
- **HTTPS endpoint (8443)**: Client certificate mandatory, full mTLS enforcement
- Kestrel configured with `ClientCertificateMode.RequireCertificate`
- Different responses based on connection security level

### 3. Client Implementation
- Loads client certificate from PFX file
- Tests both HTTP and HTTPS endpoints
- Demonstrates successful mTLS handshake
- Shows certificate validation working at transport layer

### 4. Validation Proof Points
- **HTTP calls work**: Proves basic connectivity
- **HTTPS calls require certificate**: Demonstrates mTLS enforcement
- **Different responses**: Server distinguishes between HTTP/HTTPS calls
- **Secure operations protected**: Only accessible via mTLS endpoint

## Certificate Details

### Generated Certificates
```
CA Certificate: CN=MtlsDemo-CA, O=MtlsDemo, S=CA, C=US
Server Certificate: CN=localhost, O=MtlsDemo, S=CA, C=US
Client Certificate: CN=client, O=MtlsDemo, S=CA, C=US
```

### Files Created
- `ca.crt` / `ca.key`: Certificate Authority
- `server.crt` / `server.key` / `server.pfx`: Server certificate
- `client.crt` / `client.key` / `client.pfx`: Client certificate

## Service Contract

### IGreetingService
- `GetGreeting(string name)`: Basic operation, works on both HTTP/HTTPS
- `GetSecureInfo()`: Secure operation, requires mTLS (HTTPS endpoint)

### Security Behavior
- HTTP endpoint: Returns message indicating no client certificate
- HTTPS endpoint: Returns message confirming mTLS connection
- Secure operations: Only accessible via authenticated HTTPS connection

## Docker Support

### Building Images
```bash
docker compose build
```

### Running Containers
```bash
docker compose up
```

**Note**: Docker networking may require additional configuration for certificate validation.

## Project Structure

```
├── MtlsServer/              # ASP.NET Core WCF service
│   ├── Program.cs           # Kestrel and CoreWCF configuration
│   ├── IGreetingService.cs  # Service contract
│   ├── GreetingService.cs   # Service implementation
│   └── Dockerfile           # Server container
├── MtlsClient/              # Console test client
│   ├── Program.cs           # Client implementation and tests
│   ├── IGreetingService.cs  # Service contract (client-side)
│   └── Dockerfile           # Client container
├── certificates/            # Generated certificates
│   ├── ca.crt, ca.key       # Certificate Authority
│   ├── server.pfx           # Server certificate
│   └── client.pfx           # Client certificate
├── docker-compose.yml       # Container orchestration
├── test-local.sh           # Automated test script (Linux/macOS)
├── test-local.ps1          # Automated test script (Windows/PowerShell)
└── README.md               # This file
```

## Technical Implementation Details

### Kestrel Configuration
- Dual HTTP/HTTPS listeners
- TLS certificate from PFX file
- Client certificate validation enforced
- AllowAnyClientCertificate for demo purposes

### CoreWCF Integration
- BasicHttpBinding for both HTTP and HTTPS
- Transport security mode for HTTPS endpoint
- Service detection of connection security level

### Client Certificate Handling
- X509CertificateLoader for modern certificate loading
- Disabled server certificate validation (demo only)
- Certificate attached to WCF client credentials

## Security Considerations

**This is a demonstration project. For production use:**
- Use proper certificate validation
- Implement certificate revocation checking
- Use certificates from trusted CA
- Enable proper logging and monitoring
- Implement proper error handling
- Use secure certificate storage

## Troubleshooting

### Common Issues
1. **Certificate errors**: Ensure certificates are in the correct location
2. **Port conflicts**: Check that ports 8080 and 8443 are available
3. **Permission errors**: Verify certificate file permissions
4. **Network issues**: Check firewall settings for Docker containers

### Logs
Server logs show certificate validation and connection details. Client logs show connection status and certificate loading.

## Validation Checklist

✅ **mTLS Working**: Client connects successfully to HTTPS endpoint with certificate  
✅ **Certificate Required**: HTTPS endpoint rejects connections without certificate  
✅ **HTTP Fallback**: HTTP endpoint works without certificate  
✅ **Service Differentiation**: Different responses based on connection security  
✅ **Transport Security**: TLS handshake completes with mutual authentication  
✅ **Certificate Loading**: Both server and client certificates load successfully  

## Next Steps

To extend this demo:
1. Add certificate revocation checking
2. Implement proper certificate validation
3. Add certificate-based authorization
4. Create certificate management tools
5. Add comprehensive logging and monitoring