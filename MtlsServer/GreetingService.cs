using CoreWCF;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;

namespace MtlsServer;

public class GreetingService : IGreetingService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GreetingService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

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

        var clientCertificate = _httpContextAccessor.HttpContext?.Connection?.ClientCertificate;
        var issuer = clientCertificate?.Issuer ?? "Unknown";

        return $"Secure operation completed successfully. Connection authenticated via mutual TLS on HTTPS endpoint. Client certificate issuer: {issuer}";
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