using UnityEngine;
using UnityEngine.AI;

public class AgentMotionHandler : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Character character;
    
    private Transform owner;

    private void Awake()
    {
        owner = character.transform;

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public Vector2 GetMoveIntent()
    {
        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
            return Vector2.zero;

        Vector3 velocity = agent.desiredVelocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude <= 0.01f)
            return Vector2.zero;

        Vector3 localVelocity = owner.InverseTransformDirection(velocity);

        return new Vector2(localVelocity.x, localVelocity.z).normalized;
    }

    public Quaternion GetRotationDelta()
    {
        if (!agent.hasPath)
            return Quaternion.identity;

        Vector3 direction = agent.steeringTarget - owner.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return Quaternion.identity;

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
        Quaternion delta = Quaternion.RotateTowards(owner.rotation, lookRotation, agent.angularSpeed * Time.deltaTime) * Quaternion.Inverse(owner.rotation);

        return delta;
    }

    private void LateUpdate()
    {
        agent.nextPosition = owner.position; // Sync agent to character position 
    }
}
