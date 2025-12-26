using UnityEngine;

[System.Serializable]
public class FrameWindow
{
    public float startTime;
    public float endTime;

    public bool IsValid(float currentTime)
    {
        return currentTime >= startTime && currentTime <= endTime;
    }
}
