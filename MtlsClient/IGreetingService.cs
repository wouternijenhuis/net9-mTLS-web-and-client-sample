using System.ServiceModel;

namespace MtlsClient;

[ServiceContract]
public interface IGreetingService
{
    [OperationContract]
    string GetGreeting(string name);
    
    [OperationContract]
    string GetSecureInfo();
}