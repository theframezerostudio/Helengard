using UnityEngine;
using System;
using System.Runtime.CompilerServices;

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

    public override void Started(Spell spell, CharacterCastingManager castingManager)
    {
        this.castingManager = castingManager;
        this.spell = spell;
        castingManager.SetCurrentStrategy(this);

        cameraTransform = Camera.main.transform; 

        // Start with a point in front of the camera
        targetPosition = cameraTransform.position + cameraTransform.forward * 5f;

        if (castInstance == null && spell.castingProperties.castVFX != null)
        {
            castInstance = GameObject.Instantiate(spell.castingProperties.castVFX, targetPosition, Quaternion.identity);
        }
    }

    public override void Performing()
    {
        // Check if the spell properties are AOECastProperties
        if (spell.castingProperties is AOECastProperties aoeProperties)
        {
            moveSpeed = aoeProperties.circleMoveSpeed;
            castRange = aoeProperties.circleRange;
            groundMask = aoeProperties.groundMask;
            effectRadius = aoeProperties.effectRadius;
        }
        if (castingManager is PlayerCastingManager playerCastingManager)
        {
            horizontalValue = playerCastingManager.horizontalMoveAmount;
            verticalValue = playerCastingManager.verticalMoveAmount;
        }
        else
        {
            Debug.LogWarning("Spell properties are not configured for AOE casting");
        }

        // Ground snapping using raycast (maybe remove)
        if (Physics.Raycast(targetPosition + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 6f, groundMask))
        {
            targetPosition = hit.point;
        }

        Vector3 moveDirection = cameraTransform.forward * verticalValue;
        moveDirection += cameraTransform.right * horizontalValue;

        moveDirection.y = 0f;
        moveDirection.Normalize();

        targetPosition.y = 0f;
        Vector3 cameraPos = cameraTransform.position;
        cameraPos.y = 0f;

        targetPosition += moveDirection * moveSpeed * Time.deltaTime;
        targetPosition = cameraPos + (targetPosition - cameraPos).normalized *
                         Mathf.Min(Vector3.Distance(cameraPos, targetPosition), castRange); // Limiting the cast to move within the given castRange

        targetPosition.y = hit.point.y;

        if (castInstance != null)
            castInstance.transform.position = targetPosition;
    }

    public override void Stopped()
    {   
        if(castInstance)
            GameObject.Destroy(castInstance);

        if(spell.castingProperties.spellVFX != null)
        {
            spellInstance = GameObject.Instantiate(spell.castingProperties.spellVFX, targetPosition , Quaternion.identity);
            GameObject.Destroy(spellInstance,spell.castingProperties.spellDuration);
        }
           
        castingManager.ClearCurrentStrategy();
    }
}
