using System.Collections.Generic;

public sealed class GameplayEffect
{
    private readonly List<AppliedStatModifier> appliedModifiers = new();

    private readonly List<EffectModule> modules = new();

    public EffectDefinition Definition { get; }

    public float RemainingDuration { get; private set; }

    public int StackCount { get; private set; } = 1;
    public bool IsExpired => !Definition.InfiniteDuration && RemainingDuration <= 0f;

    public IReadOnlyList<AppliedStatModifier> AppliedModifiers => appliedModifiers;

    public GameplayEffect(EffectDefinition definition)
    {
        Definition = definition;

        RemainingDuration = definition.Duration;
    }

    public void InitializeModules(GameplayEffectController controller, StatContainer stats)
    {
        for (int i = 0; i < Definition.Modules.Count; i++)
        {
            EffectModule module = Definition.Modules[i].CreateModule();

            module.Initialize(this, controller, stats);

            modules.Add(module);
        }
    }

    public void OnApplied()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].OnApplied();
        }
    }

    public void Tick(float deltaTime)
    {
        if (!Definition.InfiniteDuration)
        {
            RemainingDuration -= deltaTime;
        }

        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].Tick(deltaTime);
        }
    }

    public void OnRemoved()
    {
        for (int i = 0; i < modules.Count; i++)
        {
            modules[i].OnRemoved();
        }
    }

    public void RefreshDuration()
    {
        RemainingDuration = Definition.Duration;
    }

    public void AddDuration(float duration)
    {
        RemainingDuration += duration;
    }

    public bool CanAddStack()
    {
        return StackCount < Definition.MaxStacks;
    }

    public void AddStack()
    {
        StackCount++;
    }

    public void AddAppliedModifier(AppliedStatModifier modifier)
    {
        appliedModifiers.Add(modifier);
    }
}