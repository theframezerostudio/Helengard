using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackPlayer : MonoBehaviour
{
    [SerializeField] private float initialDelay = 0f;
    [SerializeReference, SubclassSelector] private List<Feedback> feedbacks = new();

    private void Start()
    {
        foreach (var feedback in feedbacks)
        {
            feedback.Initialize();
        }
    }

    public void Play()
    {
        StartCoroutine(InitiateFeeback());
    }

    private IEnumerator InitiateFeeback()
    {
        yield return new WaitForSeconds(initialDelay);

        bool hasPause = HasPause();

        if (hasPause)
        {
            StartCoroutine(PlayPausedFeedbacks());
        }
        else
        {
            PlayFeedbacks();
        }
    }

    private void PlayFeedbacks()
    {
        foreach (var feedback in feedbacks)
        {
            feedback.Play();
        }
    } 

    private IEnumerator PlayPausedFeedbacks()
    {
        for (int i = 0; i < feedbacks.Count; i++)
        {
            Feedback feedback = feedbacks[i];

            feedback.Play();

            if (feedback.PauseWait != null)
            {
                yield return feedback.PauseWait;
            }
        }
    }
    
    public void PauseFeedbacks()
    {
        foreach (var feedback in feedbacks)
        {
            feedback.Pause();
        }
    }
    
    public void StopFeedbacks()
    {
        foreach (var feedback in feedbacks)
        {
            feedback.Stop();
        }
    }

    public bool HasPause()
    {
        foreach (var feedback in feedbacks)
        {
            if (feedback is Pause_Feedback)
            {
                return true;
            }
        }

        return false;
    }
}