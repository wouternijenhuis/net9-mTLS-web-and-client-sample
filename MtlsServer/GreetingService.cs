using CoreWCF;
using System.Security.Cryptography.X509Certificates;

namespace MtlsServer;

public class GreetingService : IGreetingService
{
    public string GetGreeting(string name)
    {
        var context = OperationContext.Current;
        var clientCertificate = GetClientCertificate(context);
        
        return clientCertificate != null 
            ? $"Hello {name}! Your client certificate subject is: {clientCertificate.Subject}"
            : $"Hello {name}! No client certificate provided.";
    }

    public string GetSecureInfo()
    {
        var context = OperationContext.Current;
        var clientCertificate = GetClientCertificate(context);
        
        if (clientCertificate == null)
        {
            throw new FaultException("Client certificate is required for this operation.");
        }

        return $"Secure operation completed. Client certificate thumbprint: {clientCertificate.Thumbprint}";
    }

    private X509Certificate2? GetClientCertificate(OperationContext? context)
    {
        if (context?.RequestContext?.RequestMessage?.Properties != null)
        {
            if (context.RequestContext.RequestMessage.Properties.TryGetValue("httpRequest", out var httpRequestProperty))
            {
                if (httpRequestProperty is Microsoft.AspNetCore.Http.HttpContext httpContext)
                {
                    return httpContext.Connection.ClientCertificate;
                }
            }
        }
        return null;
    }
}