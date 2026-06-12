using UnityEngine;
using System;
using UnityEngine.Rendering;

[Serializable]
public class AOECasting : CastingStrategy
{   
    // private instances
    private GameObject castInstance;
    private GameObject spellInstance;
    private Transform cameraTransform;
    private Vector3 targetPosition;

    // Casting properties (AOE)
    private float moveSpeed;
    private float castRange;
    private LayerMask groundMask;
    private float effectRadius;

    // Input related
    private float horizontalValue;
    private float verticalValue;

    public override void Activate(SpellCastContext context)
    {
        base.Activate(context);

        cameraTransform = Camera.main.transform;

        spellAnimator.SetIntent(1f);
        spellAnimator.PlayAnim(StartAnimState);

        // Start with a point in front of the camera
        //targetPosition = cameraTransform.position + cameraTransform.forward * 5f;
        
        targetPosition = context.Aim.Target.transform.position;
        //targetPosition = context.Aim.Origin;
        if (castInstance == null && properties.castVFX != null)
        {
            castInstance = GameObject.Instantiate(
            properties.castVFX,
            targetPosition,
            Quaternion.identity
        );
            //castInstance = GameObject.Instantiate(properties.castVFX, targetPosition, Quaternion.identity);
        }
    }

    public override void Performing(SpellCastContext context)
    {
        base.Performing(context);

        CastingData data = context.CastingData;
        // Check if the spell properties are AOECastProperties
        if (properties is AOECastProperties aoeProperties)
        {
            moveSpeed = aoeProperties.circleMoveSpeed;
            castRange = aoeProperties.circleRange;
            groundMask = aoeProperties.groundMask;
            effectRadius = aoeProperties.effectRadius;
        }
        else
        {
            Debug.LogWarning("Spell properties are not configured for AOE casting");
        }

        horizontalValue = data.horizontalMoveAmount;
        verticalValue = data.verticalMoveAmount;

        // Ground snapping using raycast (maybe remove)
        if (Physics.Raycast(targetPosition + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 6f, groundMask))
        {
            
        }

        Vector3 moveDirection = cameraTransform.forward * verticalValue;
        moveDirection += cameraTransform.right * horizontalValue;

        moveDirection.y = 0f;
        moveDirection.Normalize();

        //targetPosition.y = 0f;
        Vector3 cameraPos = cameraTransform.position;
        cameraPos.y = 0f;

        targetPosition += moveDirection * moveSpeed * Time.deltaTime;
        targetPosition = cameraPos + (targetPosition - cameraPos).normalized *
                         Mathf.Min(Vector3.Distance(cameraPos, targetPosition), castRange); // Limiting the cast to move within the given castRange

        targetPosition.y = hit.point.y;

        if (castInstance != null)
            castInstance.transform.position = targetPosition;
    }

    public override void Deactivate()
    {   
        base.Deactivate();

        if(castInstance)
            GameObject.Destroy(castInstance);

        float duration = spellAnimator.PlayAnim(ExecuteAnimState);
        StartRecovery(duration, 0.4f);
        if (properties.spellVFX != null)
        {
            spellInstance = GameObject.Instantiate(properties.spellVFX, targetPosition , Quaternion.identity);
            GameObject.Destroy(spellInstance,properties.spellDuration);
        }
    }
}
