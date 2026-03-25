using System.Collections;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] private Spell[] spells;
    [SerializeField, ReadOnly] private Spell currentSpell;

    private Coroutine performRoutine = null;
    private bool isPerforming;

    private void Start()
    {
        for (int i = 0; i < spells.Length; i++)
        {
            spells[i].Initialize();
        }

        SkillSelector(0);
    }

    public void SkillSelector(int index)
    {
        if (index < spells.Length)
        {
            currentSpell = spells[index];
        }
    }

    public void OnCastStart()
    {
        currentSpell.Start();
    }

    public void OnCastPerform(CastingData data)
    {
        isPerforming = true;

        if (performRoutine != null)
        {
            StopCoroutine(performRoutine);
            performRoutine = null;
        }

        performRoutine = StartCoroutine(PerformCast(data));
    }

    public void OnCastRelease()
    {
        isPerforming = false;
        currentSpell.Stop();
    }

    private IEnumerator PerformCast(CastingData data)
    {
        while (isPerforming)
        {
            currentSpell.Tick(data);
            yield return null;
        }

        performRoutine = null;
    }
}
