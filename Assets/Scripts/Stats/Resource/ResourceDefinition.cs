using UnityEngine;

[CreateAssetMenu(
    fileName = "Resource",
    menuName = "Gameplay/Resources/Resource"
)]
public sealed class ResourceDefinition : ScriptableObject
{
    public enum ResourceStartMode
    {
        Empty,
        Full,
        Custom
    }

    public enum ResourceSyncMode
    {
        HardClamp,
        PreservePercentage,
        PreserveDelta
    }

    [Header("Identity")]
    [SerializeField] private string id;
    [SerializeField] private string displayName;

    [Header("Capacity")]
    [SerializeField] private StatDefinition maxValueStat;

    [Header("Initialization")]
    [SerializeField] private ResourceStartMode startMode = ResourceStartMode.Full;
    [SerializeField] private float customStartValue;

    [Header("Sync")]
    [SerializeField] private ResourceSyncMode syncMode = ResourceSyncMode.PreservePercentage;

    public string Id => id;
    public string DisplayName => displayName;
    public StatDefinition MaxValueStat => maxValueStat;
    public ResourceStartMode StartMode => startMode;
    public float CustomStartValue => customStartValue;
    public ResourceSyncMode SyncMode => syncMode;
}