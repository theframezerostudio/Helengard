using UnityEngine;

public abstract class StatDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;

    [SerializeField] private string displayName;

    [Header("Flags")]
    [SerializeField] private bool hidden;

    public string Id => id;

    public string DisplayName => displayName;

    public bool Hidden => hidden;

    public virtual bool IsDerived => false;
}