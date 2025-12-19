using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private CharacterContext characterContext;

    private void Start()
    {
        InputManager.Instance.onSprint += HandleSprintInput;
        InputManager.Instance.onGuard += HandleGuardInput;
    }

    private void HandleGuardInput(bool isguarding)
    {
        characterContext.IsGuarding = isguarding;
    }

    public void Initialize(CharacterContext context)
    {
        characterContext = context;
    }

    public void HandleSprintInput(bool isSprinting)
    {
        characterContext.isSprinting = isSprinting;
    }

    public void OnDestroy()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.onSprint -= HandleSprintInput;
        InputManager.Instance.onGuard -= HandleGuardInput;
    }
}
