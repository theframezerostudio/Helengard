using UnityEngine;

/// <summary>
/// AI-facing read-only state used to make decisions.
/// Built from Player and AI snapshot data.
/// </summary>
public class AICombatContext
{
    public Target target;

    // Normalized (0–1)
    public float DistanceNormalized;
    public float AIStaminaNormalized;
    public float AIHealthNormalized;

    // Binary or Likelihood values (0–1)
    public float PlayerIsOpen;
    public float PlayerIsBlocking;
    public float PlayerIsAttacking;
    public float AIUnderPressure;
    public float AIHasInitiative;

    public bool AIIsAirborne;

    // Tunable distance, can be supplied per-agent too
    public float MaxAttackRange = 6f;

    // Raw snapshots kept for advanced systems
    public CombatSnapshot AISnapshot;
    public CombatSnapshot PlayerSnapshot;

    /// <summary>
    /// Builds a fresh AI decision context using snapshots from both sides.
    /// This is called ONCE per decision tick, usually from AIController.
    /// </summary>
    public void Build(CombatSnapshot aiData, CombatSnapshot playerData)
    {
        // TODO: Changes per Combo Node
        MaxAttackRange = 6f;

        // store raw snapshots
        AISnapshot = aiData;
        PlayerSnapshot = playerData;

        // ========== DERIVED VALUES ==========
        float dist = Vector3.Distance(aiData.position, playerData.position);
        DistanceNormalized = Mathf.Clamp01(dist / MaxAttackRange);

        // TODO: Correctly handle max stamina and hp for normalization (currently assumes 1)
        AIStaminaNormalized = Mathf.Clamp01(aiData.stamina);    
        AIHealthNormalized = Mathf.Clamp01(aiData.hp);

        // Player states (turn booleans into 0–1)
        PlayerIsAttacking = playerData.isAttacking ? 1f : 0f;
        PlayerIsBlocking = playerData.isDefending ? 1f : 0f;

        // Example heuristic:
        PlayerIsOpen = (!playerData.isDefending && !playerData.isAttacking) ? 1f : 0f;

        // AI initiative heuristics
        AIUnderPressure = (playerData.isAttacking && dist < 4f) ? 1f : 0f;
        AIHasInitiative = (!playerData.isAttacking && aiData.isAttacking) ? 1f : 0f;

        // Airborne check from velocity or animator snapshot
        AIIsAirborne = aiData.velocity.y > 0.1f;
    }
}