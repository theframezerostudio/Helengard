using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterCastingManager : MonoBehaviour
{
    protected CastingStrategy currentStrategy;
    protected Spell currentSpell;


    public void UpdateCurrentSpell(Spell spell)
    {
        if (spell == currentSpell)
        {
            return;
        }

        currentSpell = spell;
    }

    public virtual void SetCurrentStrategy(CastingStrategy strategy)
    {
        currentStrategy = strategy;
    }

    protected virtual void HandleCastStarted(InputAction.CallbackContext context)
    {

    }
    protected virtual void HandleCastPerforming(InputAction.CallbackContext context)
    {

    }
    protected virtual void HandleCastStopped(InputAction.CallbackContext context)
    {

    }

    public virtual void ClearCurrentStrategy()
    {
        currentStrategy = null;
    }
}
