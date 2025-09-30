#!/bin/bash

echo "=== Testing Mutual TLS Setup Locally ==="

# Build the solution
echo "Building solution..."
dotnet build MtlsDemo.sln

if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

# Start the server in background
echo "Starting server..."
cd MtlsServer
dotnet run &
SERVER_PID=$!
cd ..

# Wait for server to start
echo "Waiting for server to start..."
sleep 5

# Run the client
echo "Running client..."
cd MtlsClient
dotnet run
CLIENT_EXIT_CODE=$?
cd ..

# Stop the server
echo "Stopping server..."
kill $SERVER_PID 2>/dev/null

if [ $CLIENT_EXIT_CODE -eq 0 ]; then
    echo "=== Test completed successfully! ==="
else
    echo "=== Test failed! ==="
fi

exit $CLIENT_EXIT_CODE