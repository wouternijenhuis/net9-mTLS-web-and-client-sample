# mTLS Certificates

This directory contains SSL/TLS certificates for mutual TLS (mTLS) demonstration.

## Files

The following certificate files should be generated in this directory:

- **ca.crt, ca.key** - Certificate Authority (CA)
- **server.crt, server.key, server.pfx** - Server certificate (CN=localhost)
- **client.crt, client.key, client.pfx** - Client certificate (CN=client)

## Generating Certificates

If the `.pfx` files are missing, you can generate them using the provided scripts:

### On Windows (PowerShell):
```powershell
.\generate-certificates.ps1
```

### On Linux/macOS:
```bash
./generate-certificates.sh
```

## Prerequisites

- OpenSSL must be installed and available in your PATH
- Windows: Install from [https://slproweb.com/products/Win32OpenSSL.html](https://slproweb.com/products/Win32OpenSSL.html)
- Linux: `sudo apt-get install openssl` or `sudo yum install openssl`
- macOS: `brew install openssl`

## Certificate Details

- **Certificate Authority**: CN=MtlsDemo-CA, O=MtlsDemo, ST=CA, C=US
- **Server Certificate**: CN=localhost, O=MtlsDemo, ST=CA, C=US
- **Client Certificate**: CN=client, O=MtlsDemo, ST=CA, C=US
- **Password**: `password` (for all .pfx files)
- **Validity**: 365 days (certificates), 3650 days (CA)

## Security Note

⚠️ **Important**: These certificates are for demonstration and testing purposes only. 

- Do NOT use these certificates in production
- The password is hardcoded as "password" for simplicity
- The private keys are stored unencrypted on disk
- For production use, generate new certificates with:
  - Strong passwords
  - Secure key storage
  - Proper certificate management practices
  - Certificates signed by a trusted CA

## Troubleshooting

### OpenSSL Not Found
If you get an error that OpenSSL is not found:
1. Verify OpenSSL is installed: `openssl version`
2. On Windows, ensure OpenSSL is in your PATH environment variable
3. On Linux/macOS, install OpenSSL using your package manager

### Permission Denied (Linux/macOS)
If you get a permission denied error when running the script:
```bash
chmod +x generate-certificates.sh
```

### Certificate Files Already Exist
The generation scripts will:
- Skip generating .crt and .key files if they already exist
- Always regenerate .pfx files from existing .crt and .key files

To regenerate all certificates from scratch:
```bash
# Back up existing certificates if needed
rm -rf certificates/*.crt certificates/*.key certificates/*.pfx certificates/*.csr certificates/*.srl

# Run the generation script
./generate-certificates.sh  # or generate-certificates.ps1
```
