using UnityEngine;
using UnityEngine.AI;

public class StateContext : MonoBehaviour
{
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public AgentMotionHandler MotionHandler { get; private set; }
    public AICombatData CombatData { get; private set; }
    public AICombatMemory CombatMemory { get; private set; } 

    // TODO : Find Target in AI_Patrol or random stroll Action
    public Target Target => CombatData.Target;

    private void Awake()
    {
        CombatData = new AICombatData();
        CombatMemory = new AICombatMemory();
    }
}