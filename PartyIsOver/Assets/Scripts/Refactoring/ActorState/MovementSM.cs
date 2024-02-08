using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TreeEditor;
using UnityEngine;

public class MovementSM : StateMachine
{
    [HideInInspector]
    public Idle IdleState;
    [HideInInspector]
    public Moving MovingState;
    [HideInInspector]
    public Jumping JumpingState;

    public Rigidbody Rigidbody;
    public Rigidbody FootRigidbody;
    public BodyHandler BodyHandler;
    public ReadSpreadSheet ReadSpreadSheet;
    public CharacterPhysicsMotion JumpAnimation;
    //speed는 ScriptableObject 로 변경해서 받아야함
    public float Speed = 4;
    public float RunSpeed = 1.35f;

    private void Awake()
    {
        IdleState = new IdleAnimation(this);
        MovingState = new MovingAnimation(this);
        JumpingState = new JumpingAnimation(this);
        JumpAnimation = new CharacterPhysicsMotion();
        Init();
    }

    private void Init()
    {
        Transform hip = transform.Find("GreenHip");
        Rigidbody = hip.GetComponent<Rigidbody>();
        Transform foot = transform.Find("foot_l");
        FootRigidbody = foot.GetComponent<Rigidbody>();
        BodyHandler = GetComponent<BodyHandler>();
        ReadSpreadSheet = GetComponent<ReadSpreadSheet>();
    }

    protected override BaseState GetInitialState()
    {
        return IdleState;
    }

    public void DataSave(int count,int num, string dataName)
    {
        switch (num)
        {
            case 0:
                JumpAnimation.ReferenceRigidbodies[count] = Part(dataName);
                Debug.Log(JumpAnimation.ReferenceRigidbodies[count]);
                break;
            case 1:
                JumpAnimation.ActionRigidbodies[count] = Part(dataName);
                break;
            case 2:
                JumpAnimation.ActionForceDirections[count] = Direction(dataName);
                break;
            case 3:
                JumpAnimation.ActionForceValues[count] = PowerValue(dataName);
                break;
            default:
                break;
        }
    }

    private Rigidbody Part(string part)
    {
        Rigidbody partRigidboy;
        switch (part)
        {
            case "GreenHead":
                partRigidboy = BodyHandler.Head.PartRigidbody;
                break;
            case "GreenChest":
                partRigidboy = BodyHandler.Chest.PartRigidbody;
                break;
            case "GreenWaist":
                partRigidboy = BodyHandler.Waist.PartRigidbody;
                break;
            case "GreenHip":
                partRigidboy = BodyHandler.Hip.PartRigidbody;
                break;
            case "GreenLegR1":
                partRigidboy = BodyHandler.RightLeg.PartRigidbody;
                break;
            case "GreenLegL1":
                partRigidboy = BodyHandler.LeftLeg.PartRigidbody;
                break;
            case "GreenLegR2":
                partRigidboy = BodyHandler.RightThigh.PartRigidbody;
                break;
            case "GreenLegL2":
                partRigidboy = BodyHandler.LeftThigh.PartRigidbody;
                break;
            case "foot_r":
                partRigidboy = BodyHandler.RightFoot.PartRigidbody;
                break;
            case "foot_l":
                partRigidboy = BodyHandler.LeftFoot.PartRigidbody;
                break;
            case "GreenUpperArmL":
                partRigidboy = BodyHandler.LeftArm.PartRigidbody;
                break;
            case "GreenUpperArmR":
                partRigidboy = BodyHandler.RightArm.PartRigidbody;
                break;
            case "GreenForeArmL":
                partRigidboy = BodyHandler.LeftForearm.PartRigidbody;
                break;
            case "GreenForeArmR":
                partRigidboy = BodyHandler.RightForearm.PartRigidbody;
                break;
            case "GreenFistL":
                partRigidboy = BodyHandler.LeftHand.PartRigidbody;
                break;
            case "GreenFistR":
                partRigidboy = BodyHandler.RightHand.PartRigidbody;
                break;
            default:
                partRigidboy = Rigidbody;
                break;
        }
        return partRigidboy;
    }

    private Define.ForceDirection Direction(string name)
    {
        Define.ForceDirection direction = 0;
        switch (name)
        {
            case "Zero":
                direction = Define.ForceDirection.Zero;
                break;
            case "ZeroReverse":
                direction = Define.ForceDirection.ZeroReverse;
                break;
            case "Forward":
                direction = Define.ForceDirection.Forward;
                break;
            case "Backward":
                direction = Define.ForceDirection.Backward;
                break;
            case "Up":
                direction = Define.ForceDirection.Up;
                break;
            case "Down":
                direction = Define.ForceDirection.Down;
                break;
            case "Right":
                direction = Define.ForceDirection.Right;
                break;
            case "Left":
                direction = Define.ForceDirection.Left;
                break;
        }
        return direction;
    }

    private float PowerValue(string value)
    {
        float power = float.Parse(value);
        return power;
    }

}
