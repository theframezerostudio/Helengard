using UnityEngine;

public abstract class CameraElementDefinition : ScriptableObject
{
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            Id = System.Guid.NewGuid().ToString("N");
    }
#endif
}