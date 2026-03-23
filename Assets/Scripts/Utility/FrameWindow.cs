using UnityEngine;

[System.Serializable]
public class FrameWindow
{
    public float startTime = 0f;
    public float endTime = 0f;

    public FrameWindow(float startTime, float endTime)
    {
        this.startTime = startTime;
        this.endTime = endTime;
    }

    public bool IsValid(float currentTime)
    {
        if (endTime == 0f) return false;

        return currentTime >= startTime && currentTime <= endTime;
    }

    public bool IsAccepted(float currentTime, float graceTime)
    {
        if (endTime == 0f) return false;

        return currentTime >= (startTime - graceTime) && currentTime <= endTime;
    }

    public bool IsOver(float currentTime)
    {
        return currentTime >= endTime;
    }
}
