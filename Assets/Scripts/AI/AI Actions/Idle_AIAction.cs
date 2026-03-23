public class Idle_AIAction : AIAction
{
    public override void Enter(Character Owner, StateContext stateContext)
    {
        Owner.SetAnim("Forward", 0f, 0.1f);
        stateContext.MotionHandler.canRotate = true;
    }

    public override void Tick()
    {

    }

    public override void Exit()
    {

    }
}
