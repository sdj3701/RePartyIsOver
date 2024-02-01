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
    public CharacterPhysicsMotion aaa;
    //speed는 ScriptableObject 로 변경해서 받아야함
    public float Speed = 4;
    public float RunSpeed = 1.35f;

    private void Awake()
    {
        IdleState = new IdleAnimation(this);
        MovingState = new MovingAnimation(this);
        JumpingState = new JumpingAnimation(this);

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
        aaa = GetComponent<CharacterPhysicsMotion>();
    }

    protected override BaseState GetInitialState()
    {
        return IdleState;
    }

    public void StartAnimation(int row =0, int col = 0)
    {
        StartCoroutine(ReadSpreadSheet.LoadData(row,col));
    }
}
