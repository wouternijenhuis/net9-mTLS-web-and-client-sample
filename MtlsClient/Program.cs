using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using MtlsClient;

Console.WriteLine("=== Mutual TLS WCF Client ===");

try
{
    // Load client certificate using modern API
    var clientCert = X509CertificateLoader.LoadPkcs12FromFile("../certificates/client.pfx", "password");
    Console.WriteLine($"Loaded client certificate: {clientCert.Subject}");

    // Create binding with client certificate authentication
    var binding = new BasicHttpsBinding();
    binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
    binding.Security.Mode = BasicHttpsSecurityMode.Transport;

    // Create endpoint address
    var endpoint = new EndpointAddress("https://localhost:8080/GreetingService");

    // Create channel factory
    var factory = new ChannelFactory<IGreetingService>(binding, endpoint);
    
    // Set client certificate
    factory.Credentials.ClientCertificate.Certificate = clientCert;
    
    // For demo purposes, disable server certificate validation
    factory.Credentials.ServiceCertificate.SslCertificateAuthentication = 
        new System.ServiceModel.Security.X509ServiceCertificateAuthentication()
        {
            CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None
        };

    // Create service proxy
    var proxy = factory.CreateChannel();

    Console.WriteLine("\n=== Testing WCF Service with Mutual TLS ===");

    // Test basic greeting
    Console.WriteLine("\n1. Testing GetGreeting:");
    var greeting = proxy.GetGreeting("Mutual TLS Client");
    Console.WriteLine($"Response: {greeting}");

    // Test secure operation
    Console.WriteLine("\n2. Testing GetSecureInfo:");
    var secureInfo = proxy.GetSecureInfo();
    Console.WriteLine($"Response: {secureInfo}");

    // Close the channel
    ((IClientChannel)proxy).Close();
    factory.Close();

    Console.WriteLine("\n=== Test completed successfully! ===");
}
catch (Exception ex)
{
    Console.WriteLine($"\nError: {ex.Message}");
    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    Environment.Exit(1);
}
