using Unity.Cinemachine;
using UnityEngine;

public class ShotGraphManager : MonoBehaviour
{
    public ShotData[] shots;
    public string startShotId = "Idle";
    public string testShotId = "Test";

    private string previousShotId = "";
    private ShotData currentShot = null;
    private CinemachineBrain brain;

    void Awake()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();

        foreach (ShotData shot in shots)
            shot.Initialize();

        SetShot(startShotId);
    }

    [ContextMenu("Test Shot")]
    public void TestShot()
    {
        SetShot(testShotId);
    }

    public void SetShot(string id)
    {
        ShotData newShot = FindShot(id);
        if (newShot == null || newShot == currentShot) return;

        ApplyBlend(newShot);

        previousShotId = currentShot != null ? currentShot.id : "";
        currentShot?.Deactivate();
        newShot.Activate();

        currentShot = newShot;
    }

    public void RevertShot()
    {
        if (string.IsNullOrEmpty(previousShotId)) return;

        SetShot(previousShotId);
    }

    private void ApplyBlend(ShotData shot)
    {
        brain.DefaultBlend = new CinemachineBlendDefinition(
            shot.blendStyle,
            shot.blendTime
        );
    }

    private ShotData FindShot(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        foreach (var shot in shots)
            if (shot.id == id) return shot;
        return null;
    }
}