using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>, PlayerControls.IPlayerActions
{
    private PlayerControls controls;

    public PermissionManager permissionManager;

    // Character and Camera Movement Interaction Event
    public Action<Vector2> onMove;
    public Action<Vector2> onCameraMove;

    // Hold Interaction Events
    public event Action<bool> onSprint;
    public event Action<bool> onGuard;
    public event Action<InputAction.CallbackContext> onCast;
    public event Action<AttackInput> onAttack;

    // Tap or Press Interaction Events
    public event Action onDash;
    public event Action onJump;
    public event Action onLock;

    public event Action<int> onSkillSelect;

    public Vector2 MoveInput { get; private set; }

    private void Awake()
    {
        permissionManager = new PermissionManager();
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.SetCallbacks(this);
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.RemoveCallbacks(this);
        controls.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (permissionManager.IsAllowed(AbilityTag.Move))
            MoveInput = context.ReadValue<Vector2>();
        else
            MoveInput = Vector2.zero;

        onMove?.Invoke(MoveInput);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        onCameraMove?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (!permissionManager.IsAllowed(AbilityTag.Move))
        {
            onSprint?.Invoke(false);
            return;
        }

        if (context.performed)
            onSprint?.Invoke(true);
        else if (context.canceled)
            onSprint?.Invoke(false);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!permissionManager.IsAllowed(AbilityTag.Move))
            return;

        if (context.performed)
            onDash?.Invoke();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!permissionManager.IsAllowed(AbilityTag.Jump))
            return;

        onJump?.Invoke();  
    }

    public void OnLockOn(InputAction.CallbackContext context)
    {
        onLock?.Invoke();
    }

    public void OnCast(InputAction.CallbackContext context)
    {
        if (permissionManager.IsAllowed(AbilityTag.Cast))
            return;

        onCast?.Invoke(context);
    }

    public void OnSpellSelect(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        Vector2 val = context.ReadValue<Vector2>();
        int index = 0;

        if (val == Vector2.up)
            index = 0;
        else if (val == Vector2.right)
            index = 1;
        else if (val == Vector2.down)
            index = 2;
        else if (val == Vector2.left)
            index = 3;

        Debug.Log("Test" + index);
        onSkillSelect?.Invoke(index);
    }

    public void OnGuard(InputAction.CallbackContext context)
    {
        if (!permissionManager.IsAllowed(AbilityTag.Guard))
        {
            onGuard?.Invoke(false);
            return;
        }

        if (context.performed)
            onGuard?.Invoke(true);
        else if (context.canceled)
            onGuard?.Invoke(false);
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (!permissionManager.IsAllowed(AbilityTag.Attack))
            return;

        if (context.performed)
            onAttack?.Invoke(AttackInput.Light);
    }

    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (!permissionManager.IsAllowed(AbilityTag.Attack))
            return;

        if (context.performed)
            onAttack?.Invoke(AttackInput.Heavy);
    }

    public void OnLightHoldAttack(InputAction.CallbackContext context)
    {
        if (!permissionManager.IsAllowed(AbilityTag.Attack))
            return;

        if (context.performed)
            onAttack?.Invoke(AttackInput.LightHold);
    }

    public void OnHeavyHoldAttack(InputAction.CallbackContext context)
    {
        if (!permissionManager.IsAllowed(AbilityTag.Attack))
            return;

        if (context.performed)
            onAttack?.Invoke(AttackInput.HeavyHold);
    }
}
