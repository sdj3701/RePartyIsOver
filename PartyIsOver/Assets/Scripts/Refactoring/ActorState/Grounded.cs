using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Grounded : BaseState
{

    public Grounded(string name, MovementSM stateMachine) : base(name, stateMachine) {}
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Check");
            stateMachine.ChangeState(sm.JumpingState);
        }
    }
}
