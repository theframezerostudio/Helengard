using System;
using UnityEngine;

public class Temp : MonoBehaviour
{
    private void Awake()
    {
        InputManager.Instance.onMove += HandleMove;
    }

    private void HandleMove(Vector2 vector)
    {
        Debug.Log(vector);
    }
}
