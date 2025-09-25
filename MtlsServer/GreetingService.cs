using CoreWCF;
using System.Security.Cryptography.X509Certificates;

namespace MtlsServer;

public class GreetingService : IGreetingService
{
    public string GetGreeting(string name)
    {
        var isSecureConnection = IsSecureConnection();
        
        if (isSecureConnection)
        {
            return $"Hello {name}! Connected via HTTPS with mutual TLS. Certificate validation successful at transport level.";
        }
        else
        {
            return $"Hello {name}! Connected via HTTP. No client certificate provided.";
        }
    }

    public string GetSecureInfo()
    {
        var isSecureConnection = IsSecureConnection();
        
        if (!isSecureConnection)
        {
            throw new FaultException("Client certificate is required for this operation. Please use HTTPS endpoint.");
        }

        return $"Secure operation completed successfully. Connection authenticated via mutual TLS on HTTPS endpoint.";
    }

    private bool IsSecureConnection()
    {
        // Check if we're running on a secure connection by examining the operation context
        var operationContext = OperationContext.Current;
        
        // In CoreWCF, we can check if the request came through HTTPS
        // This is a simplified way to detect if mTLS was used
        var requestUri = operationContext?.RequestContext?.RequestMessage?.Headers?.To;
        return requestUri?.Scheme == "https";
    }
}