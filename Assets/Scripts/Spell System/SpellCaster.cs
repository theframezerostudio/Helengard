using System.Collections;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [SerializeReference, SubclassSelector] public CastingStrategy[] spells;
    public CastingStrategy currentSpell;

    private bool isPerforming = false;

    private void Awake()
    {
    }

    private void Start()
    {
        SkillSelector(0);
    }

    public void SkillSelector(int index)
    {
        if (index < spells.Length)
        {
            currentSpell = spells[index];
        }
    }

    public void ExecuteSkill()
    {
       
    }

    public void OnCastStart()
    {
        currentSpell.Start();
    }

    public void OnCastPerform(CastingData data)
    {
        isPerforming = true;
        StartCoroutine(PerformCast(data));
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
            currentSpell.Performing(data);
            yield return null;
        }
    }
}
