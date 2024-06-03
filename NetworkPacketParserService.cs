using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

public sealed class NetworkPacketParserService : BackgroundService
{

    private readonly PacketParserService _parser;
    private readonly LibPcapLiveDevice _defaultGatewayDevice;
    private Dictionary<string, int> _ipTable;

    public NetworkPacketParserService(PacketParserService packetParserService)
    {
        var devices = LibPcapLiveDeviceList.Instance;
        _defaultGatewayDevice = devices[0];
        _parser = packetParserService;
        _ipTable = new Dictionary<string, int>();
    }

    public void Device_OnPacketArrival(object s, PacketCapture packetCapture)
    {
        var rawCapture = packetCapture.GetPacket();
        var p = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
        var packetInfo = p.ToString(StringOutputType.Normal);
        var sourceIP = _parser.GetSourceIP(packetInfo) != null ? _parser.GetSourceIP(packetInfo) : "NOT FOUND";
        if (sourceIP is not null) TrackIP(sourceIP);
        Console.WriteLine(sourceIP);
    }

    public void TrackIP(string ipAddress)
    {
        if (_ipTable.TryGetValue(ipAddress, out int count)) _ipTable[ipAddress] = count + 1;
        else _ipTable[ipAddress] = 1;
        saveReportToFile();
    }

    public void saveReportToFile()
    {
        try
        {
            using (StreamWriter writer = File.CreateText("resume.txt"))
            {
                foreach (KeyValuePair<string, int> item in _ipTable)
                {
                    writer.WriteLine($"{item.Key}: {item.Value},");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR Writing File: {ex.Message}");
        }
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

        saveReportToFile();
    }
}