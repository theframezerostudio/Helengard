using UnityEngine;

/// <summary>
/// Identity-only asset used to route an attack to a reaction module.
/// 
/// Examples:
/// RK_Stagger.asset
/// RK_Flight.asset
/// RK_AirJuggle.asset
/// RK_GroundSmash.asset
/// 
/// Do not store reaction tuning data here.
/// </summary>
[CreateAssetMenu(
    fileName = "RK_NewReaction",
    menuName = "Combat/Reactions/Reaction Key")]
public sealed class ReactionKey : ScriptableObject
{
}