using System.Net;
using System.Net.Sockets;
using System.Text;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Common;

/// <summary>
/// Emit server metrics. Since it's only a few metrics emitted, to avoid a dependency to an external library, the
/// Datadog datagrams are crafted and sent to the DogStatsD port.
/// </summary>
internal class ServerMetricsBehavior : MissionBehavior
{
    private static readonly EndPoint DogStatsDEndpoint = new DnsEndPoint("localhost", 8125);

    private MissionTimer? _updateTimer;
    private Socket? _socket;

    public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

    public override void OnBehaviorInitialize()
    {
        _updateTimer = new MissionTimer(60);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.Connect(DogStatsDEndpoint);

        // https://docs.datadoghq.com/developers/dogstatsd/datagram_shell/?tab=events
        string title = "Mission started";
        string content = $"Mission started on scene {Mission.SceneName}";
        string datagramStr = $"_e{{{title.Length},{content.Length}}}:{title}|{content}|k:mission-started|s:csharp|#{BuildTags()}";
        SendDatagram(datagramStr);
    }

    public override void OnRemoveBehavior()
    {
        _socket?.Dispose();
    }

    private string BuildTags()
    {
        string region = CrpgServerConfiguration.Region.ToString().ToLowerInvariant();
        string service = CrpgServerConfiguration.Service;
        string instance = CrpgServerConfiguration.Instance;
        return $"region:{region},service:{service},instance:{instance}";
    }

    private void SendDatagram(string datagramStr)
    {
        byte[] datagram = Encoding.ASCII.GetBytes(datagramStr);
        _socket!.Send(datagram);
    }
}
