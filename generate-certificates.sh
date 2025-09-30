#!/bin/bash

################################################################################
# Generate SSL/TLS certificates for mutual TLS (mTLS) demonstration
#
# This script generates a Certificate Authority (CA), server certificate, and
# client certificate using OpenSSL. The certificates are created in PKCS#12
# (.pfx) format for use with .NET applications.
#
# Prerequisites:
#   - OpenSSL must be installed
#
# Usage:
#   ./generate-certificates.sh
#
# The script generates the following files in the 'certificates' directory:
#   - ca.crt, ca.key: Certificate Authority
#   - server.crt, server.key, server.pfx: Server certificate
#   - client.crt, client.key, client.pfx: Client certificate
#
# All certificates use password: "password"
################################################################################

set -e  # Exit on error

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;90m'
NC='\033[0m' # No Color

echo -e "${CYAN}=== mTLS Certificate Generation Script ===${NC}"
echo ""

# Check if OpenSSL is available
if ! command -v openssl &> /dev/null; then
    echo -e "${RED}✗ Error: OpenSSL is not installed or not in PATH${NC}"
    echo -e "${YELLOW}  Please install OpenSSL using your package manager:${NC}"
    echo -e "${YELLOW}    - Ubuntu/Debian: sudo apt-get install openssl${NC}"
    echo -e "${YELLOW}    - macOS: brew install openssl${NC}"
    exit 1
fi
echo -e "${GREEN}✓ OpenSSL is available${NC}"

# Create certificates directory if it doesn't exist
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CERT_DIR="$SCRIPT_DIR/certificates"

if [ ! -d "$CERT_DIR" ]; then
    mkdir -p "$CERT_DIR"
    echo -e "${GREEN}✓ Created certificates directory${NC}"
else
    echo -e "${GREEN}✓ Certificates directory exists${NC}"
fi

cd "$CERT_DIR"

echo ""
echo -e "${CYAN}Generating certificates...${NC}"

# Certificate configuration
PASSWORD="password"

# 1. Generate CA private key and certificate
echo ""
echo -e "${YELLOW}1. Generating Certificate Authority (CA)...${NC}"
if [ ! -f "ca.key" ] || [ ! -f "ca.crt" ]; then
    echo -e "${GRAY}  → Generate CA private key${NC}"
    openssl genrsa -out ca.key 4096
    
    echo -e "${GRAY}  → Generate CA certificate${NC}"
    openssl req -new -x509 -days 3650 -key ca.key -out ca.crt \
        -subj "/C=US/ST=CA/O=MtlsDemo/CN=MtlsDemo-CA"
    
    echo -e "${GREEN}  ✓ CA certificate created${NC}"
else
    echo -e "${GRAY}  ℹ CA certificate already exists, skipping...${NC}"
fi

# 2. Generate Server certificate
echo ""
echo -e "${YELLOW}2. Generating Server certificate...${NC}"
if [ ! -f "server.key" ] || [ ! -f "server.crt" ]; then
    echo -e "${GRAY}  → Generate server private key${NC}"
    openssl genrsa -out server.key 4096
    
    echo -e "${GRAY}  → Generate server CSR${NC}"
    openssl req -new -key server.key -out server.csr \
        -subj "/C=US/ST=CA/O=MtlsDemo/CN=localhost"
    
    echo -e "${GRAY}  → Sign server certificate${NC}"
    openssl x509 -req -in server.csr -CA ca.crt -CAkey ca.key \
        -CAcreateserial -out server.crt -days 365 -sha256
    
    echo -e "${GREEN}  ✓ Server certificate created${NC}"
else
    echo -e "${GRAY}  ℹ Server certificate already exists, skipping...${NC}"
fi

# Generate server.pfx
echo -e "${GRAY}  → Generating server.pfx...${NC}"
openssl pkcs12 -export -out server.pfx \
    -inkey server.key -in server.crt -certfile ca.crt \
    -passout pass:$PASSWORD
echo -e "${GREEN}  ✓ server.pfx created${NC}"

# 3. Generate Client certificate
echo ""
echo -e "${YELLOW}3. Generating Client certificate...${NC}"
if [ ! -f "client.key" ] || [ ! -f "client.crt" ]; then
    echo -e "${GRAY}  → Generate client private key${NC}"
    openssl genrsa -out client.key 4096
    
    echo -e "${GRAY}  → Generate client CSR${NC}"
    openssl req -new -key client.key -out client.csr \
        -subj "/C=US/ST=CA/O=MtlsDemo/CN=client"
    
    echo -e "${GRAY}  → Sign client certificate${NC}"
    openssl x509 -req -in client.csr -CA ca.crt -CAkey ca.key \
        -CAcreateserial -out client.crt -days 365 -sha256
    
    echo -e "${GREEN}  ✓ Client certificate created${NC}"
else
    echo -e "${GRAY}  ℹ Client certificate already exists, skipping...${NC}"
fi

# Generate client.pfx
echo -e "${GRAY}  → Generating client.pfx...${NC}"
openssl pkcs12 -export -out client.pfx \
    -inkey client.key -in client.crt -certfile ca.crt \
    -passout pass:$PASSWORD
echo -e "${GREEN}  ✓ client.pfx created${NC}"

echo ""
echo -e "${CYAN}=== Certificate Generation Complete ===${NC}"
echo ""
echo -e "${GREEN}Generated files in 'certificates' directory:${NC}"
echo -e "${NC}  - ca.crt, ca.key          (Certificate Authority)${NC}"
echo -e "${NC}  - server.crt, server.key, server.pfx  (Server certificate)${NC}"
echo -e "${NC}  - client.crt, client.key, client.pfx  (Client certificate)${NC}"
echo ""
echo -e "${YELLOW}Certificate password: $PASSWORD${NC}"
echo ""
echo -e "${CYAN}You can now run the application:${NC}"
echo -e "${NC}  dotnet build MtlsDemo.sln${NC}"
echo -e "${NC}  ./test-local.sh${NC}"
echo ""
