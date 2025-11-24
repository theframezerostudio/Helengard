using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCastingManager : CharacterCastingManager
{

    [HideInInspector] public float horizontalMoveAmount;
    [HideInInspector] public float verticalMoveAmount;
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        InputManager.Instance.onCast += HandleCastStarted;
        InputManager.Instance.onCast += HandleCastPerforming;
        InputManager.Instance.onCast += HandleCastStopped;
        InputManager.Instance.onSkillSelect += HandleSkillSelect;
        InputManager.Instance.onMove += HandleOnMovement;
    }

    private void HandleOnMovement(Vector2 vector)
    {
        verticalMoveAmount = vector.y;
        horizontalMoveAmount = vector.x;
        //Debug.Log(horizontalMoveAmount + " " + verticalMoveAmount);
    }

    private void HandleSkillSelect(int skillIndex)
    {
        spellCaster.SkillSelector(skillIndex);
    }

    protected override void HandleCastStarted(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            spellCaster.OnCastStart();
        }
    }
    protected override void HandleCastPerforming(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            spellCaster.OnCastPerform();
        }
    }

    protected override void HandleCastStopped(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            spellCaster.OnCastRelease();    
        }
    }

    public override void ClearCurrentStrategy()
    {
        base.ClearCurrentStrategy();
    }

    public override void SetCurrentStrategy(CastingStrategy strategy)
    {
        base.SetCurrentStrategy(strategy);
    }

    private void UnsubscribeEvents()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.onCast -= HandleCastStarted;
            InputManager.Instance.onCast -= HandleCastPerforming;
            InputManager.Instance.onCast -= HandleCastStopped;
            InputManager.Instance.onSkillSelect -= HandleSkillSelect;
            InputManager.Instance.onMove -= HandleOnMovement;
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
