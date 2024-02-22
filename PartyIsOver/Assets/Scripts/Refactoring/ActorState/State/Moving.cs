using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Moving : Grounded
{
    private float inputspeed;
    //private float horizontalInput;
    //private float verticalInput;
    protected float MovingMotionSpeed;
    protected float MovingMotionTimer = 0;

    protected bool isRun;

    public Define.Pose leftLegPose;
    public Define.Pose rightLegPose;
    public Define.Pose leftArmPose;
    public Define.Pose rightArmPose;

    public Moving(MovementSM stateMachine) : base("Moving", stateMachine) 
    {
    }
    public override void Enter()
    {
        base.Enter();
        inputspeed = 0f;
        //�õ尪
        if (Random.Range(0, 2) == 1)
        {
            leftLegPose = Define.Pose.Bent;
            rightLegPose = Define.Pose.Straight;
            leftArmPose = Define.Pose.Straight;
            rightArmPose = Define.Pose.Bent;
        }
        else
        {
            leftLegPose = Define.Pose.Straight;
            rightLegPose = Define.Pose.Bent;
            leftArmPose = Define.Pose.Bent;
            rightArmPose = Define.Pose.Straight;
        }
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        moveInput = new Vector3(sm.PlayerCharacter.horizontalInput, 0, sm.PlayerCharacter.verticalInput);

        if (sm.PlayerCharacter.horizontalInput > 0 || sm.PlayerCharacter.horizontalInput < 0)
        {
            sm.turnOnCommand.Execute();
        }    

        if(sm.PlayerCharacter.verticalInput > 0 || sm.PlayerCharacter.verticalInput < 0)
        {
            sm.turnOffCommand.Execute();
        }


        if(Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
            MovingMotionSpeed = 0.15f;
        }
        else
        {
            isRun = false;
            MovingMotionSpeed = 0.1f;
        }

        if (moveInput != Vector3.zero)
        {
            inputspeed = 1f;
        }
        else
        {
            stateMachine.ChangeState(sm.IdleState);
        }
    }
}