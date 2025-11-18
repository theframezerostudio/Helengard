using UnityEngine;

public class PlayerConjuringManager : CharacterConjuringManager
{
    //[HideInInspector] public PlayerManager player;
    public override void Start()
    {
        base.Start();
        //player = GetComponent<PlayerManager>();

    }

    public override void Update()
    {
        base.Update();

    }

    public override void ClearCurrentStrategy()
    {
        base.ClearCurrentStrategy();
    }

    public override void SetCurrentStrategy(ConjuringStrategy strategy)
    {
        base.SetCurrentStrategy(strategy);
    }
}
