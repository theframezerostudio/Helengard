using UnityEngine;

[CreateAssetMenu(
    fileName = "EffectCategory",
    menuName =
        "Gameplay/Stats/Effect Category"
)]
public sealed class EffectCategory : ScriptableObject
{
    [SerializeField] private string id;

    public string Id => id;
}