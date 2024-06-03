using CapturingAndParsingPackets;
using PacketDotNet;
using PacketDotNet.Utils;
using SharpPcap;
using SharpPcap.LibPcap;
using System.Net.NetworkInformation;

public sealed class NetworkPacketParserService : BackgroundService
{

    private LibPcapLiveDevice _defaultGatewayDevice;
    private Dictionary<string, int> _ipTable;

    public NetworkPacketParserService()
    {
        var devices = LibPcapLiveDeviceList.Instance;
        _defaultGatewayDevice = devices[0];
        _ipTable = new Dictionary<string, int>();
    }

    public void Device_OnPacketArrival(object s, PacketCapture packetCapture)
    {
        var rawCapture = packetCapture.GetPacket();
        var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
        Console.WriteLine(p.ToString(StringOutputType.Normal));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _defaultGatewayDevice.Open();
        _defaultGatewayDevice.OnPacketArrival += Device_OnPacketArrival;
        _defaultGatewayDevice.StartCapture();

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1_000, stoppingToken);
        }
    }
}