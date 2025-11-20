using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterCastingManager : MonoBehaviour
{
    [SerializeField] protected CastingStrategy currentStrategy;
    [SerializeField] protected SpellCaster spellCaster;

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

    protected virtual void HandleSkillSelect(InputAction.CallbackContext context)
    {

    }
    public virtual void ClearCurrentStrategy()
    {
        currentStrategy = null;
    }
}
