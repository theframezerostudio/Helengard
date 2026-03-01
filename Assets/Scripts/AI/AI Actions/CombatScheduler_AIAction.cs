using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatScheduler_AIAction : AIAction
{
    public List<CombatSubAction> subActions;

    [SerializeField] private CombatPersona persona;

    [SerializeField, ReadOnly] private CombatSubAction current = null;
    [SerializeField, ReadOnly] private AICombatData combatData;

    private AICombatMemory memory;
    
    private Target target;

    private CombatSnapshot targetCombat;
    private CombatSnapshot selfCombat;
    
    private bool falseTarget = false;
    private bool isActive = false;
    private bool hasInitialized = false;

    // TODO: For Inspector debugging, remove later
    [SerializeField, ReadOnly] private float attackScore;
    [SerializeField, ReadOnly] private float defenseScore;
    [SerializeField, ReadOnly] private float recoveryScore;
    [SerializeField, ReadOnly] private float dodgeScore;

    public override void Enter(Character Owner, StateContext stateContext)
    {
        owner = Owner;
        context = stateContext;

        combatData = stateContext.CombatData;
        memory = stateContext.CombatMemory;

        target = stateContext.Target;

        InitializeStates();

        if (target.Context == null)
        {
            falseTarget = true;
            return;
        }

        isActive = true;
        StartCoroutine(StateEvaluation());
        //combatData.Build(selfCombat, targetCombat);
    }

    public override void Tick()
    {
        if (falseTarget)
            return;

        targetCombat = target.Context.CombatData;
        selfCombat = owner.Context.CombatData;

        combatData.Build(selfCombat, targetCombat);
        memory.Tick(Time.deltaTime);

        current?.Tick();
    }

    public override void Exit()
    {
        current?.Exit();

        memory.ResetAll();

        isActive = false;
        current = null;
    }

    private void InitializeStates()
    {
        if (hasInitialized)
            return;

        foreach (var act in subActions)
        {
            act.Initialize(owner, context);
        }

        hasInitialized = true;
    }

    private IEnumerator StateEvaluation()
    {
        while (isActive)
        {
            CombatSubAction next = ChooseBestAction();
            if (next != current)
            {
                current?.Exit();
                Debug.Log($"Switching to {next.Label}");
                memory.OnStateChanged(next);
                next.Enter();
                current = next;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private CombatSubAction ChooseBestAction()
    {
        float bestScore = float.MinValue;
        CombatSubAction bestAction = null;

        foreach (var act in subActions)
        {
            float score = act.Evaluate(persona);

            // TODO: For Inspector debugging, remove later
            {
                if (act is Attack_CombatAction)
                    attackScore = score;
                else if (act is Defense_CombatAction)
                    defenseScore = score;
                else if (act is Recovery_CombatAction)
                    recoveryScore = score;
                else if (act is Dodge_CombatAction)
                    dodgeScore = score;
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestAction = act;
            }
        }

        return bestAction;
    }
}