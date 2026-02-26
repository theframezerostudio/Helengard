using System.Collections.Generic;
using UnityEngine;

public class CombatScheduler_AIAction : AIAction
{
    public List<CombatSubAction> subActions;

    private CombatSubAction current;
    private AICombatContext combatContext;

    private Target target;

    [SerializeField] private CombatSnapshot targetCombat;
    [SerializeField] private CombatSnapshot selfCombat;
    
    [SerializeField] private bool falseTarget = false;

    public override void Enter(Character Owner, StateContext stateContext)
    {
        owner = Owner;
        context = stateContext;
        combatContext = stateContext.CombatContext;
        target = stateContext.Target;

        if (target.characterContext == null)
        {
            falseTarget = true;
            return;
        }

        targetCombat = target.characterContext.CombatData;
        selfCombat = owner.Context.CombatData;

        combatContext.Build(selfCombat, targetCombat);
    }

    public override void Tick()
    {
        if (falseTarget)
            return;

        CombatSubAction next = ChooseBestAction();

        if (next != current)
        {
            //current?.Exit();
            next.Enter(owner, combatContext);
            current = next;
        }

        current.Tick();
    }

    public override void Exit()
    {
        current.Exit();
        current = null;
    }

    private CombatSubAction ChooseBestAction()
    {
        float bestScore = float.MinValue;
        CombatSubAction bestAction = null;

        foreach (var act in subActions)
        {
            float score = act.Evaluate(combatContext);

            if (score > bestScore)
            {
                bestScore = score;
                bestAction = act;
            }
        }
        return bestAction;
    }
}