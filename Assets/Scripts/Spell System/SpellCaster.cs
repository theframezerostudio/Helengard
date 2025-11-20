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

    public void SkillSelector(int index)
    {
        if (index < spells.Length)
        {
            currentSpell = spells[index];
            castingManager.UpdateCurrentSpell(currentSpell);
        }
    }

    public void ExecuteSkill()
    {

    }
}
