using Crpg.Module.Api.Models.Characters;
using Crpg.Module.Api.Models.Users;

namespace Crpg.Module.Balancing;

internal class WeightedCrpgUser
{
    public WeightedCrpgUser(CrpgUser user, float weight)
    {
        User = user;
        Weight = weight;
    }

    public CrpgUser User { get; }
    public int? ClanId => User.ClanMembership?.ClanId;
    public float Weight { get; }

    // Character can be null in test setups, hence the null-conditional access.
    public bool IsCavalry => User.Character?.Class is CrpgCharacterClass.Cavalry or CrpgCharacterClass.MountedArcher;
}
