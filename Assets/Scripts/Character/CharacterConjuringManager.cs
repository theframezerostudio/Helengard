using UnityEngine;

public class CharacterConjuringManager : MonoBehaviour
{
    protected ConjuringStrategy currentStrategy;

    public virtual void Start()
    {

    }
    public virtual void Update()
    {

    }

    public virtual void SetCurrentStrategy(ConjuringStrategy strategy)
    {
        currentStrategy = strategy;
    }

    public virtual void ClearCurrentStrategy()
    {
        currentStrategy = null;
    }
}
