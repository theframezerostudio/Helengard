using System;
using UnityEngine;

[Serializable]
public class CharacterContext
{
    public bool isSprinting;

    //Serialized for testing purposes
    [SerializeField] private bool isLockedOn;
    [SerializeField] private bool isGuarding;
    public bool isPerfectGuarding;

    [Header("Character Context Events")]
    public bool IsGuarding
    {
        get { return isGuarding; }
        set
        {
            if (value == isGuarding) return;
            isGuarding = value;
            OnGuard?.Invoke(isGuarding);
        }
    }

    public bool IsLockedOn
    {
        get { return isLockedOn; }
        set
        {
            if (value == isLockedOn) return;

            isLockedOn = value;
            OnTargetLock?.Invoke(isLockedOn);
        }
    }

    public Action<bool> OnTargetLock;
    public Action<bool> OnGuard;
}
