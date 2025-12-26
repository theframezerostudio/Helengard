using Unity.Cinemachine;
using UnityEngine;

public class Timeline : MonoBehaviour
{
    public bool IsWindowValid(FrameWindow window, float time)
    {
        if (window == null)
        {
            Debug.LogError("FrameWindow is null.");
            return false;
        }

        return time >= window.startTime && time <= window.endTime;
    }
}