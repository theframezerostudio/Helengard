using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class FreeMoveMode : LocomotionMode
{
    public FreeMoveMode(Player player) : base(player)
    {
    }

    public override void Move(Vector3 moveDir, float speed)
    {
        player.Controller.SimpleMove(moveDir * speed);
    }

    public override void Rotate(Vector3 moveDir)
    {
        if (moveDir.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(moveDir);
        player.transform.rotation = Quaternion.Slerp(
            player.transform.rotation,
            targetRot,
            Time.deltaTime * player.rotationDamping
        );
    }

    public override void PerformDodge(Vector2 dir)
    {
    }

    public override void PlayAnimation(Vector3 input)
    {
        player.SetAnim("Speed", input.magnitude, 0.1f);

        bool isSprinting = player.Context.isSprinting;
        player.SetAnim("IsSprinting", isSprinting);
    }
}
