using Ciot.Protos.V2;
using Ciot.Grpc.Common.Stream;
using Ciot.Sdk.Config;
using Ciot.Sdk.Iface;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var ifacesSubscribers = new ConcurrentDictionary<string, Subscriber<Event>>();

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

builder.Services.AddSingleton(ifacesSubscribers);
builder.Services.AddSingleton<IIfaceRepository, IfaceRepository>();
builder.Services.AddSingleton<IIfaceManager, IfaceManager>();
builder.Services.AddSingleton<IConfigRepository, ConfigRepository>();

var app = builder.Build();

app.MapGrpcService<Ciot.Grpc.Services.IfaceService>();
app.MapGrpcService<Ciot.Grpc.Services.ConfigService>();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

// Configure the HTTP request pipeline.
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
