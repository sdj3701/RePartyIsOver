using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class Jumping : BaseState
{
    private float horizontalInput;
    private float verticalInput;
    private bool grounded;
    private bool change;

    private LayerMask groundLayer;
    private CharacterPhysicsMotion[] JumpingAnimation;

    public Jumping(MovementSM stateMachine) : base("Jumping", stateMachine)
    {
        groundLayer = LayerMask.GetMask("ClimbObject");
    }

    public override void Enter()
    {
        base.Enter();

        //sm.Rigidbody.AddForce(Vector3.up * sm.Speed * 0.5f);
        change = false;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        moveInput = new Vector3(horizontalInput, 0, verticalInput);
        RunCyclePoseBody();
        //change �Լ��� �߰��� ���� : UpdateLogic�� ������ Enter���� false�� ��ȯ�ص� ture�� ��ȯ�ؼ� change�� ������ �Ǿ����� Ȯ��
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

        //�浹�� �Ǿ����� Ȯ�� Ȯ���� ���̸� Idle ���·� ��ȯ
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