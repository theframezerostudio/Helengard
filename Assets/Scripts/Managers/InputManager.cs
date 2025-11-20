using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>, PlayerControls.IPlayerActions
{
    private PlayerControls controls;

    // Character and Camera Movement Interaction Event
    public event Action<Vector2> onMove;
    public event Action<Vector2> onCameraMove;

    // Hold Interaction Events
    public event Action<bool> onSprint;
    public event Action<InputAction.CallbackContext> onCast;

    // Tap or Press Interaction Events
    public event Action onDash;
    public event Action onJump;
    public event Action onLock;

    public event Action<int> onSkillSelect;


    private void OnEnable()
    {
        controls  = new PlayerControls();
        controls.Player.SetCallbacks(this);
        controls.Enable();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        onMove?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        onCameraMove?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        onSprint?.Invoke(true);
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        onDash?.Invoke();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        onJump?.Invoke();  
    }

    public void OnLockOn(InputAction.CallbackContext context)
    {
        onLock?.Invoke();
    }
    private void OnDisable()
    {   
        controls.Player.RemoveCallbacks(this);
        controls.Disable();
    }

    public void OnCast(InputAction.CallbackContext context)
    {
        onCast?.Invoke(context);
    }

    public void OnSpellSelect(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        Vector2 val = context.ReadValue<Vector2>();
        int amt = 0;

        if (val == Vector2.up)
            amt = 1;
        else if (val == Vector2.right)
            amt = 2;
        else if (val == Vector2.down)
            amt = 3;
        else if (val == Vector2.left)
            amt = 4;

        Debug.Log("Test" + amt);
        onSkillSelect?.Invoke(amt);
    }
}
