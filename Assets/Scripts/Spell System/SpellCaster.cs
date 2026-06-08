using System.Collections;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] private Spell[] spells;
    [SerializeReference] private SpellAnimationController spellAnimator;

    [SerializeField] private CharacterContext characterContext;
    [SerializeField] private PlayerTargeting targeting;
    [SerializeField, ReadOnly] private Spell currentSpell;


    private readonly SpellCastContext castContext = new SpellCastContext();
    private readonly SpellAimResolver aimResolver = new SpellAimResolver();

    private Coroutine performRoutine;
    private SpellAimData startAimData;

    [SerializeField, ReadOnly] private bool isPerforming;

    private void Awake()
    {
        if (targeting == null)
            targeting = GetComponent<PlayerTargeting>();
    }

    private void Start()
    {
        for (int i = 0; i < spells.Length; i++)
        {
            if (spells[i] != null)
                spells[i].Initialize(spellAnimator);
        }

        SkillSelector(0);
    }

    public void SkillSelector(int index)
    {
        if (isPerforming)
            return;

        if (index < 0 || index >= spells.Length)
            return;

        currentSpell = spells[index];
    }

    public void OnCastStart(CastingData data)
    {
        if (currentSpell == null)
            return;

        isPerforming = true;

        startAimData = aimResolver.Resolve(
            transform,
            characterContext,
            targeting,
            currentSpell.AimSettings,
            data
        );

        castContext.Set(
            characterContext,
            data,
            startAimData,
            Time.deltaTime
        );

        if (!castContext.Aim.IsValid)
        {
            isPerforming = false;
            return;
        }

        currentSpell.Activate(castContext);
    }

    public void OnCastPerform(CastingData data)
    {
        if (currentSpell == null)
            return;

        if (!isPerforming)
            OnCastStart(data);

        if (performRoutine != null)
        {
            StopCoroutine(performRoutine);
            performRoutine = null;
        }

        performRoutine = StartCoroutine(PerformCast(data));
    }

    public void OnCastRelease()
    {
        if (!isPerforming)
            return;

        isPerforming = false;

        if (currentSpell != null)
            currentSpell.Deactivate(castContext);
    }

    private IEnumerator PerformCast(CastingData data)
    {
        while (isPerforming)
        {
            SpellAimData aimData = ResolveAimForCurrentSpell(data);

            castContext.Set(
                characterContext,
                data,
                aimData,
                Time.deltaTime
            );

            if (!castContext.Aim.IsValid)
            {
                OnCastRelease();
                yield break;
            }

            currentSpell.Tick(castContext);

            yield return null;
        }

        performRoutine = null;
    }

    private SpellAimData ResolveAimForCurrentSpell(CastingData data)
    {
        if (currentSpell.AimSettings.updateMode == SpellAimUpdateMode.ResolveOnStart)
            return startAimData;

        return aimResolver.Resolve(
            transform,
            characterContext,
            targeting,
            currentSpell.AimSettings,
            data
        );
    }
}