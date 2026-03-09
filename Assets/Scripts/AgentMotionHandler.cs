using GLTFast.Schema;
using UnityEngine;
using UnityEngine.AI;

public class AgentMotionHandler : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Character owner;

    private Transform ownerTransform;

    private void Awake()
    {
        ownerTransform = owner.transform;

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void SetStoppingDistance(float dist)
    {
        agent.stoppingDistance = dist;
    }

    public void Update()
    {
        Vector2 intent = GetMoveIntent();
        owner.SetAnim("Speed", intent.magnitude, 0.1f);

        //owner.SetAnim("Forward", intent.y, 0.1f);
        //owner.SetAnim("Strafe", intent.x, 0.1f);

        Quaternion deltaRotation = GetRotationDelta();
        owner.motionAccumulator.AddRotation(deltaRotation);
    }

    private void LateUpdate()
    {
        // Syncs agent to character position 
        agent.nextPosition = ownerTransform.position; 
    }

    public Vector2 GetMoveIntent()
    {
        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
            return Vector2.zero;

        Vector3 velocity = agent.desiredVelocity;
        velocity.y = 0f;

        if (velocity.sqrMagnitude <= 0.01f)
            return Vector2.zero;

        Vector3 localVelocity = ownerTransform.InverseTransformDirection(velocity);

        return new Vector2(localVelocity.x, localVelocity.z).normalized;
    }

    public Quaternion GetRotationDelta()
    {
        if (!agent.hasPath)
            return Quaternion.identity;

        Vector3 direction = agent.steeringTarget - ownerTransform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return Quaternion.identity;

        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
        Quaternion delta = Quaternion.RotateTowards(ownerTransform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime) * Quaternion.Inverse(ownerTransform.rotation);

        return delta;
    }

    public void Stop() => agent.ResetPath();
}
