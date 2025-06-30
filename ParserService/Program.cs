using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options => { options.MaxReceiveMessageSize = 50 * 1024 * 1024; });
builder.Services.AddGrpcReflection();
builder.Services.AddScopedInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseGrpcWeb();
app.MapGrpcService<ParserService.Services.ParserService>();
app.MapGrpcReflectionService();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
app.Run();