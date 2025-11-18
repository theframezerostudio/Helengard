using UnityEngine;

public class Caster : MonoBehaviour
{
    public Conjuration[] Conjurations;
    public Conjuration currentConjuration;
    public CharacterConjuringManager conjuringManager;


    private void Awake()
    {
        conjuringManager = GetComponent<CharacterConjuringManager>();
    }

    public void SkillSelector(int index)
    {
        if (index < Conjurations.Length)
        {
            currentConjuration = Conjurations[index];
        }
    }

    public void ExecuteSkill()
    {

    }
}
