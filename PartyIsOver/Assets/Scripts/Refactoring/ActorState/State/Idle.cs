using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Idle : Grounded
{
    private float idleTimer = 0;

    public Idle(MovementSM stateMachine) : base("Idel", stateMachine) {}
    public override void Enter()
    {
        base.Enter();
        //TODO : �ӵ� 0���� ����
        sm.PlayerCharacter.horizontalInput = 0f;
        sm.PlayerCharacter.verticalInput = 0f;
        idleTimer = 0f;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        //TODO : Ű �Է��� ����� Moving���� ����
        if (Mathf.Abs(sm.PlayerCharacter.horizontalInput) > Mathf.Epsilon || Mathf.Abs(sm.PlayerCharacter.verticalInput) > Mathf.Epsilon)
            stateMachine.ChangeState(((MovementSM)stateMachine).MovingState);

    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        idleTimer = Mathf.Clamp(idleTimer + Time.fixedDeltaTime, -60f, 30f);

        if (sm.Rigidbody.velocity.magnitude > 1f)
            sm.Rigidbody.velocity = sm.Rigidbody.velocity.normalized * sm.Rigidbody.velocity.magnitude * 0.6f;
    }

}