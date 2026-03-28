using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [field: SerializeField] public AnimatorController Animator { get; protected set; }
    public CharacterController Controller { get; private set; }
    [field: SerializeField] public CharacterContext Context { get; private set; }
    [field: SerializeField] public FeetIKResolver FeetIKResolver { get; protected set; }
    [field: SerializeField] public Weapon CurrentWeapon { get; protected set; }

    public MotionAccumulator motionAccumulator;

    public float movementSpeed;
    public float rotationTime;

    [Header("InAir Settings")]
    public float gravity = -9.8f;
    public float verticalVelocity = 0f;
    public float groundSnapForce = -12f;
    public float terminalVelocity = -20f;
    public bool hasGravity = true;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector3 groundCheck;
    [SerializeField] private float footLength;
    [SerializeField] private float rayLength = 0.2f;
    [SerializeField] private int groundRays;
    [SerializeField] private float minSlope = 0.7f;

    // TODO: Do something about this
    private int baseLayer = 0;

    protected virtual void Awake()
    {
        motionAccumulator = new MotionAccumulator();
        Controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    protected virtual void Update()
    {
        Context.dataAggregator.SetPosition(transform.position, transform.forward);
    }

    protected float ApplyGravity(float dt)
    {
        if (!hasGravity)
            return 0f;

        if (Context.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = groundSnapForce;
        }
        else
        {
            verticalVelocity += gravity * Context.GravityScale * dt;
            verticalVelocity = Mathf.Max(verticalVelocity, terminalVelocity);
        }

        return verticalVelocity * dt;
        //LocomotionMode.AddImpulse(Vector3.up, verticalVelocity * dt);
    }

    public void CheckGround()
    {
        Vector3 origin = transform.position + groundCheck;

        float radius = footLength * 0.5f;
        float minSlopeDot = Mathf.Cos(minSlope * Mathf.Deg2Rad);

        bool grounded = false;

        for (int i = 0; i < groundRays; i++)
        {
            float angle = (360f / groundRays) * i;
            Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;

            Vector3 rayOrigin = origin + offset;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayLength, groundLayer))
            {
                if (Vector3.Dot(hit.normal, Vector3.up) >= minSlopeDot)
                {
                    grounded = true;
                    break;
                }
            }

        }

        Context.UpdateGrounded(grounded, Time.fixedDeltaTime);
    }

    public abstract void Suspend(float duration);
    public abstract void Recover(ActionData actionData);
    public abstract void Unsuspend();

    public void SetAnim(string anim, float value, float dampTime = 0f, float intent = 1)
    {
        Animator.SetAnim(anim, value, dampTime, baseLayer, intent);
    }

    public void SetAnim(string anim, bool value, float intent = 1)
    {
        Animator.SetAnim(anim, value, baseLayer, intent);
    }

    public void PlayAnim(string anim, float transitionTime = 0.1f, float intent = 1)
    {
        Animator.PlayAnim(anim, transitionTime, baseLayer, intent);
    }

    private void OnDrawGizmos()
    {
        if (groundRays <= 0) return;

        Vector3 origin = transform.position + groundCheck;
        float radius = footLength * 0.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + Vector3.down * rayLength);

        Gizmos.color = Color.green;

        for (int i = 0; i < groundRays; i++)
        {
            float angle = (360f / groundRays) * i;
            Vector3 offset =
                Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;

            Vector3 rayOrigin = origin + offset;
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * rayLength);
        }

        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(origin, radius);
    }
}
