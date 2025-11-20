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
    public event Action<bool> onConjure;

    // Tap or Press Interaction Events
    public event Action onDash;
    public event Action onJump;
    public event Action onLock;


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
    public void OnConjure(InputAction.CallbackContext context)
    {
        onConjure?.Invoke(true);
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

}
