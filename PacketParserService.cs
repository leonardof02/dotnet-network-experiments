using System.Text.RegularExpressions;

public class PacketParserService
{

    private readonly Regex _sourceIPAddressRegex = new Regex(@"\[IPv4Packet:\s*SourceAddress=(?<SourceAddress>\S+)");

    public PacketParserService() { }

    public string? GetSourceIP(string packetDataString)
    {
        Match match = _sourceIPAddressRegex.Match(packetDataString);
        if (!match.Success) return null;
        string sourceAddress = match.Groups["SourceAddress"].Value;
        return sourceAddress;
    }
}