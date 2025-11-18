using UnityEngine;
using System;

[System.Serializable]
public abstract class ConjuringStrategy
{
    public abstract void Started();
    public abstract void Performing();
    public abstract void Stopped();
    
}
