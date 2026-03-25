using UnityEngine;
using System;

[Serializable]
public class CastingData
{
    public float horizontalMoveAmount;
    public float verticalMoveAmount;
}

[System.Serializable]
public class CastingStrategy
{
    protected CastingProperties properties;
    private PermissionManager permissionManager;

    public void Initialize(CastingProperties properties)
    {
        this.properties = properties;
        permissionManager = InputManager.Instance.permissionManager;
    }

    public virtual void Start()
    {
        for (int i = 0; i < properties.blockAbilities.Length; i++)
        {
            permissionManager.Block(properties.blockAbilities[i]);
        }
    }

    public virtual void Performing(CastingData data)
    {

    }

    public virtual void Stop()
    {
        for (int i = 0; i < properties.blockAbilities.Length; i++)
        {
            permissionManager.Release(properties.blockAbilities[i]);
        }
    }
}
