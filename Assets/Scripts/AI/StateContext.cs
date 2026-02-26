using UnityEngine;
using UnityEngine.AI;

public class StateContext : MonoBehaviour
{
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public AgentMotionHandler MotionHandler { get; private set; }
    public AICombatContext CombatContext { get; private set; }

    // TODO : Find Target in AI_Patrol or random stroll Action
    public Target Target => CombatContext.target;

    private void Awake()
    {
        CombatContext = new AICombatContext();
    }
}