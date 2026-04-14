using MapApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiRuntimeOptions(builder.Configuration)
    .AddApiDataAccess(builder.Configuration, builder.Environment)
    .AddApiAuthenticationServices(builder.Configuration, builder.Environment)
    .AddApiContracts()
    .AddApiExternalClients(builder.Configuration)
    .AddApiApplicationServices();

var app = builder.Build();
app.UseApiDefaults();
app.MapControllers();

app.Run();
