using UnityEngine;

[System.Serializable]
public class AIState 
{
    [field: SerializeField] public string Label {  get; private set; }
    [SerializeField] private string animName;
    [field: SerializeField] public AIAction[] Actions { get; private set; }
    [field: SerializeField] public AIDecision[] Decisions {  get; private set; }

    public void Enter(Character Owner, StateContext stateContext)
    {
        if (!string.IsNullOrEmpty(animName))
        {
            Owner.PlayAnim(animName);
        }

        foreach (AIAction action in Actions)
        {
            action.Enter(Owner, stateContext);
        }
    }

    public void Tick()
    {
        foreach (AIAction action in Actions)
        {
            action.Tick();
        }
    }

    public void Exit()
    {
        foreach (AIAction action in Actions)
        {
            action.Exit();
        }
    }
}
