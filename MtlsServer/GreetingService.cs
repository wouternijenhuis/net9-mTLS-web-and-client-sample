using CoreWCF;
using System.Security.Cryptography.X509Certificates;

namespace MtlsServer;

public class GreetingService : IGreetingService
{
    public string GetGreeting(string name)
    {
        var clientCertificate = GetClientCertificate();
        
        return clientCertificate != null 
            ? $"Hello {name}! Your client certificate subject is: {clientCertificate.Subject}"
            : $"Hello {name}! No client certificate provided.";
    }

    public string GetSecureInfo()
    {
        var clientCertificate = GetClientCertificate();
        
        if (clientCertificate == null)
        {
            throw new FaultException("Client certificate is required for this operation.");
        }

        return $"Secure operation completed. Client certificate thumbprint: {clientCertificate.Thumbprint}";
    }

    private X509Certificate2? GetClientCertificate()
    {
        // Try to get certificate from operation context
        var operationContext = OperationContext.Current;
        
        // For now, we'll simulate certificate presence based on connection security
        // In a real implementation, this would extract the actual certificate from the context
        // This is a simplified version for demonstration purposes
        
        return null; // Will be updated when we get certificate extraction working properly
    }
}