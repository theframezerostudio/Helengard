using System.Collections;
using UnityEngine;

public class CoroutineManager : Singleton<CoroutineManager>
{
    public static Coroutine Run(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }

    public static void Stop(Coroutine coroutine)
    {
        if (Instance != null && coroutine != null)
            Instance.StopCoroutine(coroutine);
    }
}
