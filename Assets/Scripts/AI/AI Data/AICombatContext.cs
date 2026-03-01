public class AICombatContext
{
    public AICombatData Frame { get; private set; }
    public AICombatMemory Memory { get; private set; }
    public CombatPersona Persona { get; private set; }

    public AICombatContext(AICombatData frame, AICombatMemory memory, CombatPersona persona)
    {
        Frame = frame;
        Memory = memory;
        Persona = persona;
    }
}