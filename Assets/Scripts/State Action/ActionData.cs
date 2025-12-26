using UnityEngine;

[CreateAssetMenu(fileName = "ActionData", menuName = "Scriptable Objects/ActionData")]
public class ActionData : ScriptableObject
{
    public string animState;

    public float duration;

    public FrameWindow cancelWindow;
}
