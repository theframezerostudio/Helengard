using UnityEngine;

[CreateAssetMenu(menuName = "Camera/Shot Sequence")]
public class ShotSequence : ScriptableObject
{
    [System.Serializable]
    public class Step
    {
        public string shotId;
        public float duration = 1f;
    }

    public Step[] steps;
}