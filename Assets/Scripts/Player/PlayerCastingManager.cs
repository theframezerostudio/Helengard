using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCastingManager : CharacterCastingManager
{
    private CastingData castingData;

    protected override void Awake()
    {
        base.Awake();

        castingData = new();
    }

    private void Start()
    {
        InputManager.Instance.onCastHold += HandleCast;
        InputManager.Instance.onSkillSelect += HandleSkillSelect;
        InputManager.Instance.onAim += HandleOnAim;
    }

    private void HandleOnAim(Vector2 vector)
    {
        //verticalMoveAmount = vector.y;
        //horizontalMoveAmount = vector.x;

        castingData.horizontalMoveAmount = vector.x;
        castingData.verticalMoveAmount = vector.y;

        //Debug.Log(horizontalMoveAmount + " " + verticalMoveAmount);
    }

    private void HandleSkillSelect(int skillIndex)
    {
        spellCaster.SkillSelector(skillIndex);
    }

    private void HandleCast(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                spellCaster.OnCastStart(castingData);
                break;

            case InputActionPhase.Performed:
                spellCaster.OnCastPerform(castingData);
                break;

            case InputActionPhase.Canceled:
                spellCaster.OnCastRelease();
                break;
        }
    }

    protected override void HandleCastPerforming(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            spellCaster.OnCastPerform(castingData);
        }
    }

    protected override void HandleCastStopped(InputAction.CallbackContext context)
    {
        Debug.Log("HandleCastStopped: " + context.phase);
        if (context.canceled)
        {
            spellCaster.OnCastRelease();    
        }
    }

    private void UnsubscribeEvents()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.onCastHold -= HandleCast;
            InputManager.Instance.onSkillSelect -= HandleSkillSelect;
            InputManager.Instance.onAim -= HandleOnAim;
        }
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
