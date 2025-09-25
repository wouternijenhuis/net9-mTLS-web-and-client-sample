using CoreWCF;

namespace MtlsServer;

[ServiceContract]
public interface IGreetingService
{
    [OperationContract]
    string GetGreeting(string name);
    
    [OperationContract]
    string GetSecureInfo();
}