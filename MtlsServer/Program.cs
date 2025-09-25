using CoreWCF;
using CoreWCF.Configuration;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net;
using MtlsServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddHttpContextAccessor();

// Configure Kestrel for both HTTP and HTTPS
builder.Services.Configure<KestrelServerOptions>(options =>
{
    // HTTP endpoint (no SSL, for basic testing)
    options.Listen(IPAddress.Any, 8080);
    
    // HTTPS endpoint with mutual TLS
    options.Listen(IPAddress.Any, 8443, listenOptions =>
    {
        listenOptions.UseHttps(httpsOptions =>
        {
            // Try both local and Docker paths
            var certPath = File.Exists("/app/certificates/server.pfx") 
                ? "/app/certificates/server.pfx" 
                : "../certificates/server.pfx";
            
            var serverCert = X509CertificateLoader.LoadPkcs12FromFile(certPath, "password");
            httpsOptions.ServerCertificate = serverCert;
            httpsOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
            httpsOptions.AllowAnyClientCertificate();
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseServiceModel(builder =>
{
    builder.AddService<GreetingService>();
    
    // HTTP endpoint (no security)
    var httpBinding = new CoreWCF.BasicHttpBinding();
    builder.AddServiceEndpoint<GreetingService, IGreetingService>(
        httpBinding, 
        "/GreetingService");
        
    // HTTPS endpoint (with certificate)
    var httpsBinding = new CoreWCF.BasicHttpBinding();
    httpsBinding.Security.Mode = CoreWCF.Channels.BasicHttpSecurityMode.Transport;
    builder.AddServiceEndpoint<GreetingService, IGreetingService>(
        httpsBinding,
        "https://localhost:8443/GreetingService");
});

Console.WriteLine("CoreWCF Service is running...");
Console.WriteLine("HTTP endpoint: http://localhost:8080/GreetingService");
Console.WriteLine("HTTPS endpoint: https://localhost:8443/GreetingService");

app.Run();
