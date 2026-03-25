using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpellCaster))]
public class CharacterCastingManager : MonoBehaviour
{
    [SerializeField] protected SpellCaster spellCaster;
    [HideInInspector] protected float horizontalMoveAmount;
    [HideInInspector] protected float verticalMoveAmount;

    protected virtual void Awake()
    {
        spellCaster = GetComponent<SpellCaster>();
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
}
