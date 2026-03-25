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
    [SerializeField] protected CastingProperties properties;

    public void Initialize(CastingProperties properties)
    {
        this.properties = properties;
    }

    public virtual void Start()
    {
    }

    public virtual void Performing(CastingData data)
    {

    }

    public virtual void Stop()
    {

    }
    
}
