using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCastingManager : CharacterCastingManager
{
    public Camera mainCam;
    private void Awake()
    {
        mainCam = Camera.main; // Getting access to the main camera (Later we will be changing with camera manager or something)

        InputManager.Instance.onCast += HandleCastStarted;
        InputManager.Instance.onCast += HandleCastPerforming;
        InputManager.Instance.onCast += HandleCastStopped;

    }

    protected override void HandleCastStarted(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            currentStrategy.Started(currentSpell, this);
        }
    }
    protected override void HandleCastPerforming(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentStrategy.Performing();
        }
    }

    protected override void HandleCastStopped(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            currentStrategy.Stopped();
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
        InputManager.Instance.onCast -= HandleCastStarted;
        InputManager.Instance.onCast -= HandleCastPerforming;
        InputManager.Instance.onCast -= HandleCastStopped;
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
