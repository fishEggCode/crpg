using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Crpg.Module.Common.Network;

[DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
internal sealed class UpdateRewardMultiplier : GameNetworkMessage
{
    /// <summary>
    /// Bounds of the integer compression info below. Any value written outside this range makes the native
    /// serializer assert and crashes the process, so writers must clamp to this range.
    /// </summary>
    public const int RewardMultiplierMin = 1;
    public const int RewardMultiplierMax = 5;

    private static readonly CompressionInfo.Integer RewardMultiplierCompressionInfo = new(RewardMultiplierMin, RewardMultiplierMax, true);

    public int RewardMultiplier { get; set; }

    protected override void OnWrite()
    {
        WriteIntToPacket(RewardMultiplier, RewardMultiplierCompressionInfo);
    }

    protected override bool OnRead()
    {
        bool bufferReadValid = true;
        RewardMultiplier = ReadIntFromPacket(RewardMultiplierCompressionInfo, ref bufferReadValid);
        return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter()
    {
        return MultiplayerMessageFilter.GameMode;
    }

    protected override string OnGetLogFormat()
    {
        return "Update reward multiplier";
    }
}
