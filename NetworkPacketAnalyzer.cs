using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

public sealed class NetworkPacketAnalyzer : BackgroundService
{

    private readonly LibPcapLiveDevice _device;
    private ProtocolCounter _protocolCounter;

    public NetworkPacketAnalyzer()
    {
        _device = LibPcapLiveDeviceList.Instance[0];
        _protocolCounter = new ProtocolCounter();
    }

    public void Device_OnPacketArrival(object s, PacketCapture packetCapture)
    {
        var rawCapture = packetCapture.GetPacket();
        var packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

        var ethernetPacket = packet.Extract<EthernetPacket>();
        var ipv4Packet = packet.Extract<IPv4Packet>();
        var tcp = packet.Extract<TcpPacket>();
        var udp = packet.Extract<UdpPacket>();

        if (ethernetPacket is null)
        {
            _protocolCounter.NoEthernet++;
            return;
        }

        _protocolCounter.Ethernet++;
        if (ipv4Packet is null)
        {
            _protocolCounter.NoIPv4++;
            return;
        }

        _protocolCounter.IPv4++;
        if (tcp is not null) _protocolCounter.Tcp++;
        if (udp is not null) _protocolCounter.Udp++;
        if (tcp is null && udp is null) _protocolCounter.NoTcpUdp++;

        LogToConsolePacketCounter();
    }

    public void LogToConsolePacketCounter()
    {
        Console.WriteLine($"No Ethernet: {_protocolCounter.NoEthernet}");
        Console.WriteLine($"Ethernet: {_protocolCounter.Ethernet}");
        Console.WriteLine($"    IPv4: {_protocolCounter.IPv4}");
        Console.WriteLine($"        TCP: {_protocolCounter.Tcp}");
        Console.WriteLine($"        UDP: {_protocolCounter.Udp}");
        Console.WriteLine($"        No TCP-UDP: {_protocolCounter.NoTcpUdp}");
        Console.WriteLine($"    No IPv4: {_protocolCounter.NoIPv4}");
    }

    public void LogToFilePacketCounter()
    {
        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _device.Open();
        _device.OnPacketArrival += Device_OnPacketArrival;
        _device.StartCapture();

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.CompletedTask;
        }
    }
}