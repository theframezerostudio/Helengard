using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public Spell[] spells;
    public Spell currentSpell;
    public CharacterCastingManager castingManager;

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
        currentSpell.conjurationProperties.conjuringStrategy.Started(currentSpell, castingManager);
    }

    public void OnCastPerform()
    {
        currentSpell.conjurationProperties.conjuringStrategy.Performing();
    }

    public void OnCastRelease()
    {
        currentSpell.conjurationProperties.conjuringStrategy.Stopped();
    }
}
