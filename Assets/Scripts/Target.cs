using UnityEngine;

public class Target : MonoBehaviour
{
    [Tooltip("Character Context of Character, if applicable")]
    [field: SerializeField] public CharacterContext characterContext { get; private set; }
}
