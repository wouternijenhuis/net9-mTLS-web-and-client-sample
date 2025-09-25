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

// Configure Kestrel for mutual TLS
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Listen(IPAddress.Any, 8080, listenOptions =>
    {
        listenOptions.UseHttps(httpsOptions =>
        {
            var serverCert = X509CertificateLoader.LoadPkcs12FromFile("/app/certificates/server.pfx", "password");
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
    
    // Use BasicHttpBinding for HTTP endpoint
    var binding = new CoreWCF.BasicHttpBinding();
    binding.Security.Mode = CoreWCF.Channels.BasicHttpSecurityMode.Transport;
    binding.Security.Transport.ClientCredentialType = CoreWCF.Channels.HttpClientCredentialType.Certificate;
    
    builder.AddServiceEndpoint<GreetingService, IGreetingService>(
        binding, 
        "/GreetingService");
});

app.Run();
