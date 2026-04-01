using Unity.Cinemachine;
using UnityEngine;

public class TurnTiltModifier : ShotModifier
{
    public CinemachineCamera vcam;
    public float maxTilt = 8f;
    public float smooth = 6f;

    private CinemachineRotationComposer composer;
    private bool active;

    void Awake()
    {
        composer = vcam.GetComponent<CinemachineRotationComposer>();
    }
    public override void Initialize() { }

    public override void Enable() => active = true;
    public override void Disable()
    {
        active = false;
        composer.m_TrackedObjectOffset.x = 0;
    }

    void Update()
    {
        if (!active) return;

        float turn = Input.GetAxis("Horizontal");
        float target = turn * maxTilt;

        composer.m_TrackedObjectOffset.x =
            Mathf.Lerp(composer.m_TrackedObjectOffset.x, target, Time.deltaTime * smooth);
    }
}