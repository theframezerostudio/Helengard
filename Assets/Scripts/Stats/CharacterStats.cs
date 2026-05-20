using UnityEngine;

public sealed class CharacterStats : MonoBehaviour
{
    [SerializeField] private CharacterStatProfile profile;

    private StatContainer statContainer;
    public StatContainer Stats => statContainer;

    private void Awake()
    {
        statContainer = new StatContainer(profile);
    }
}