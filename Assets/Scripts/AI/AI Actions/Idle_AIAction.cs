public class Idle_AIAction : AIAction
{
    public override void Enter(Character Owner, StateContext stateContext)
    {
        Owner.SetAnim("Speed", 0f, 0.1f);
    }

    public override void Tick()
    {

    }

    public override void Exit()
    {

    }
}
