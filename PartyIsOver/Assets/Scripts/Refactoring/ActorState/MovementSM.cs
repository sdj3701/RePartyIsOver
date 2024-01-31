using System.Collections;
using System.Collections.Generic;
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
    //speed�� ScriptableObject �� �����ؼ� �޾ƾ���
    public float Speed = 4;
    public float RunSpeed = 1.35f;

    private void Awake()
    {
        IdleState = new IdleAnimation(this);
        MovingState = new MovingAnimation(this);
        JumpingState = new Jumping(this);

        Init();
    }

    private void Init()
    {
        Transform hip = transform.Find("GreenHip");
        Rigidbody = hip.GetComponent<Rigidbody>();
        Transform foot = transform.Find("foot_l");
        FootRigidbody = foot.GetComponent<Rigidbody>();
        BodyHandler = GetComponent<BodyHandler>();
    }

    protected override BaseState GetInitialState()
    {
        return IdleState;
    }
}
