using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCastingManager : CharacterCastingManager
{   
    public Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main; // Getting access to the main camera (Later we will be changing with camera manager or something)
        spellCaster = GetComponent<SpellCaster>();
    }

    private void Start()
    {
        InputManager.Instance.onCast += HandleCastStarted;
        InputManager.Instance.onCast += HandleCastPerforming;
        InputManager.Instance.onCast += HandleCastStopped;
        InputManager.Instance.onSkillSelect += HandleSkillSelect;
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
