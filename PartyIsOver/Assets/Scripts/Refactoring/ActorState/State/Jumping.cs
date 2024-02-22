using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Jumping : BaseState
{
    private bool grounded;
    private bool change;

    private LayerMask groundLayer;

    public Jumping(MovementSM stateMachine) : base("Jumping", stateMachine)
    {
        groundLayer = LayerMask.GetMask("ClimbObject");
    }

    public override void Enter()
    {
        base.Enter();

        change = false;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        moveInput = new Vector3(sm.PlayerCharacter.horizontalInput, 0, sm.PlayerCharacter.verticalInput);
        RunCyclePoseBody();
        //change 함수를 추가한 이유 : UpdateLogic이 더빨라서 Enter에서 false로 반환해도 ture로 반환해서 change로 변경이 되었는지 확인
        if (grounded && change)
            stateMachine.ChangeState(sm.IdleState);
    }

    public override void UpdatePhysics()
    {
        grounded = sm.FootRigidbody.velocity.y < Mathf.Epsilon && IsGrounded();

        base.UpdatePhysics();

        Vector3 lookForward = new Vector3(sm.PlayerCharacter.CameraTransform.forward.x, 0f, sm.PlayerCharacter.CameraTransform.forward.z).normalized;
        Vector3 lookRight = new Vector3(sm.PlayerCharacter.CameraTransform.right.x, 0f, sm.PlayerCharacter.CameraTransform.right.z).normalized;
        moveDir = lookForward * moveInput.z + lookRight * moveInput.x;

        sm.PlayerCharacter.bodyHandler.Chest.PartRigidbody.AddForce((runVectorForce10 + moveDir), ForceMode.VelocityChange);
        sm.PlayerCharacter.bodyHandler.Hip.PartRigidbody.AddForce((-runVectorForce5 + -moveDir), ForceMode.VelocityChange);

        AlignToVector(sm.PlayerCharacter.bodyHandler.Chest.PartRigidbody, -sm.PlayerCharacter.bodyHandler.Chest.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Chest.PartRigidbody, sm.PlayerCharacter.bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Waist.PartRigidbody, -sm.PlayerCharacter.bodyHandler.Waist.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Waist.PartRigidbody, sm.PlayerCharacter.bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Hip.PartRigidbody, -sm.PlayerCharacter.bodyHandler.Hip.transform.up, moveDir, 0.1f, 8f * applyedForce);
        AlignToVector(sm.PlayerCharacter.bodyHandler.Hip.PartRigidbody, sm.PlayerCharacter.bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);

        if (sm.Rigidbody.velocity.magnitude > maxSpeed)
            sm.Rigidbody.velocity = sm.Rigidbody.velocity.normalized * maxSpeed;
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        float rayLength = 0.05f; 

        //충돌이 되었는지 확인 확인후 참이면 Idle 상태로 전환
        if (Physics.Raycast(sm.FootRigidbody.position, Vector3.down, out hit, rayLength, groundLayer))
        {
            return true; 
        }
        change = true;
        return false;
    }
    public void RunCyclePoseBody()
    {
        Vector3 lookForward = new Vector3(sm.PlayerCharacter.CameraTransform.forward.x, 0f, sm.PlayerCharacter.CameraTransform.forward.z).normalized;
        Vector3 lookRight = new Vector3(sm.PlayerCharacter.CameraTransform.right.x, 0f, sm.PlayerCharacter.CameraTransform.right.z).normalized;
        moveDir = lookForward * moveInput.z + lookRight * moveInput.x;

        sm.BodyHandler.Chest.PartRigidbody.AddForce((runVectorForce10 + moveDir), ForceMode.VelocityChange);
        sm.BodyHandler.Hip.PartRigidbody.AddForce((-runVectorForce5 + -moveDir), ForceMode.VelocityChange);

        AlignToVector(sm.BodyHandler.Chest.PartRigidbody, -sm.BodyHandler.Chest.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(sm.BodyHandler.Chest.PartRigidbody, sm.BodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(sm.BodyHandler.Waist.PartRigidbody, -sm.BodyHandler.Waist.transform.up, moveDir / 4f + -Vector3.up, 0.1f, 4f * applyedForce);
        AlignToVector(sm.BodyHandler.Waist.PartRigidbody, sm.BodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);
        AlignToVector(sm.BodyHandler.Hip.PartRigidbody, -sm.BodyHandler.Hip.transform.up, moveDir, 0.1f, 8f * applyedForce);
        AlignToVector(sm.BodyHandler.Hip.PartRigidbody, sm.BodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 8f * applyedForce);

        sm.Rigidbody.AddForce(moveDir.normalized * sm.Speed * Time.deltaTime);
        if (sm.Rigidbody.velocity.magnitude > maxSpeed)
            sm.Rigidbody.velocity = sm.Rigidbody.velocity.normalized * maxSpeed;

    }
}