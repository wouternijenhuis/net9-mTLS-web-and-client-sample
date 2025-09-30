using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using MtlsClient;

Console.WriteLine("=== Mutual TLS WCF Client ===");

try
{
    // Load client certificate using modern API
    var clientCert = X509CertificateLoader.LoadPkcs12FromFile("../certificates/client.pfx", "password");
    Console.WriteLine($"Loaded client certificate: {clientCert.Subject}");

    // Determine server host (localhost for local testing, mtls-server for Docker)
    var serverHost = Environment.GetEnvironmentVariable("SERVER_HOST") ?? "localhost";
    Console.WriteLine($"Connecting to server: {serverHost}");

    Console.WriteLine("\n=== Testing HTTP Endpoint (without mTLS) ===");
    await TestHttpEndpoint(serverHost);

    Console.WriteLine("\n=== Testing HTTPS Endpoint (with mTLS) ===");
    await TestHttpsEndpoint(serverHost, clientCert);

    Console.WriteLine("\n=== All tests completed successfully! ===");
}
catch (Exception ex)
{
    Console.WriteLine($"\nError: {ex.Message}");
    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    Environment.Exit(1);
}

static async Task TestHttpEndpoint(string serverHost)
{
    try
    {
        // Test basic HTTP endpoint (no certificate required)
        var httpBinding = new BasicHttpBinding();
        var httpEndpoint = new EndpointAddress($"http://{serverHost}:8080/GreetingService");
        var httpFactory = new ChannelFactory<IGreetingService>(httpBinding, httpEndpoint);
        var httpProxy = httpFactory.CreateChannel();

        Console.WriteLine("1. Testing GetGreeting via HTTP:");
        var greeting = httpProxy.GetGreeting("HTTP Client");
        Console.WriteLine($"Response: {greeting}");

        Console.WriteLine("2. Testing GetSecureInfo via HTTP:");
        try
        {
            var secureInfo = httpProxy.GetSecureInfo();
            Console.WriteLine($"Response: {secureInfo}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Expected error for secure operation without certificate: {ex.Message}");
        }

        ((IClientChannel)httpProxy).Close();
        httpFactory.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"HTTP Test failed: {ex.Message}");
    }
}

static async Task TestHttpsEndpoint(string serverHost, X509Certificate2 clientCert)
{
    try
    {
        // Create HTTPS binding with client certificate authentication
        var httpsBinding = new BasicHttpsBinding();
        httpsBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
        httpsBinding.Security.Mode = BasicHttpsSecurityMode.Transport;

        // Create endpoint address
        var httpsEndpoint = new EndpointAddress($"https://{serverHost}:8443/GreetingService");

        // Create channel factory
        var httpsFactory = new ChannelFactory<IGreetingService>(httpsBinding, httpsEndpoint);
        
        // Set client certificate
        httpsFactory.Credentials.ClientCertificate.Certificate = clientCert;
        
        // For demo purposes, disable server certificate validation
        httpsFactory.Credentials.ServiceCertificate.SslCertificateAuthentication = 
            new System.ServiceModel.Security.X509ServiceCertificateAuthentication()
            {
                CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None
            };

        // Create service proxy
        var httpsProxy = httpsFactory.CreateChannel();

        Console.WriteLine("1. Testing GetGreeting via HTTPS with mTLS:");
        var greeting = httpsProxy.GetGreeting("HTTPS mTLS Client");
        Console.WriteLine($"Response: {greeting}");

        Console.WriteLine("2. Testing GetSecureInfo via HTTPS with mTLS:");
        var secureInfo = httpsProxy.GetSecureInfo();
        Console.WriteLine($"Response: {secureInfo}");

        // Close the channel
        ((IClientChannel)httpsProxy).Close();
        httpsFactory.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"HTTPS mTLS Test failed: {ex.Message}");
        Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
    }
}
