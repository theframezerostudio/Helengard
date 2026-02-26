public class Attack_CombatAction : CombatSubAction
{
    private AICombatDecision combatDecision;
    private AICombatContext context;
    private Character owner;
    private Weapon weapon;
    private ComboNode comboNode;

    private void Awake()
    {
        combatDecision = new AICombatDecision();
    }

    public override void Enter(Character owner, AICombatContext context)
    {
        this.owner = owner;
        weapon = owner.CurrentWeapon;
        this.context = context;
    }

    public override void Tick()
    {
        AttackInput input = combatDecision.Decide(context);
        if (input != AttackInput.None)
        {
            print($"AI decided to attack with input: {input}");
            comboNode = weapon.InitiateAttack(owner.Context, input);
        }
    }

    public override void Exit()
    {
    }
}