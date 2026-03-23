using UnityEngine;

[CreateAssetMenu(fileName = "ActionData", menuName = "Scriptable Objects/ActionData")]
public class ActionData : ScriptableObject
{
    public string animState;

    public float duration;

    public float transitionTime = 0.1f;

    public FrameWindow cancelWindow;
}
