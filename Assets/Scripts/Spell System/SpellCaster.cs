using System.Collections;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public Spell[] spells;
    public Spell currentSpell;
    public CharacterCastingManager castingManager;

    private bool isPerforming = false;

    private void Awake()
    {
        castingManager = GetComponent<CharacterCastingManager>();
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
        currentSpell.castingProperties.castingStrategy.Started(currentSpell, castingManager);
    }

    public void OnCastPerform()
    {
        isPerforming = true;
        StartCoroutine(PerformCast());
    }

    public void OnCastRelease()
    {
        isPerforming = false;
        currentSpell.castingProperties.castingStrategy.Stopped();
    }

    private IEnumerator PerformCast()
    {
        while (isPerforming)
        {
            currentSpell.castingProperties.castingStrategy.Performing();
            yield return null;
        }
    }
}
