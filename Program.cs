var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<NetworkPacketAnalyzer>();

var host = builder.Build();
host.Run();