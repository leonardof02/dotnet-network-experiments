var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<PacketParserService>();
builder.Services.AddHostedService<NetworkPacketParserService>();

var host = builder.Build();
host.Run();