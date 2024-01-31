using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Actor;
using static Define;
using static PlayerCharacter;

public class PlayerCharacter : MonoBehaviour, ICharacterBase
{
    #region �������̽� ����
    // ���� -> (100 ����)
    private float _defense = 0f;
    public float Defense { get { return _defense; } set { _defense = value; } }
    // ���ݷ� -> (0 ������ ��ȿ)
    private float _attackDamage = 1f;
    public float AttackDamage { get { return _attackDamage; } set { _attackDamage = value; } }

    private float _maxHealth = 200f;
    public float MaxHealth { get { return _maxHealth; } set { _maxHealth = value; } }

    private float _currentHealth = 200f;
    public float CurrentHealth { get { return _currentHealth; } set { _currentHealth = value; } }

    private float _maxStamina = 200f;
    public float MaxStamina { get { return _maxStamina; } set { _maxStamina = value; } }

    private float _currentStamina = 200f;
    public float CurrentStamina { get { return _currentStamina; } set { _currentStamina = value; } }

    #endregion

/*    public delegate void ChangeStaminaBar();
    public event ChangeStaminaBar OnChangeStaminaBar;*/

    #region ���� ���� ����
    public enum ActorState
    {
        Default = 0x0,
        Dead = 0x1,
        Stand = 0x4,
        Walk = 0x8,
        Run = 0x10,
        Roll = 0x20,
        Jump = 0x40,
        Fall = 0x80,
        Climb = 0x100,
        Debuff = 0x200,
    }

    public enum DebuffState
    {
        Default = 0x0,
        PowerUp = 0x1,
        Burn = 0x2,
        Exhausted = 0x4,
        Slow = 0x8,
        Ice = 0x10,
        Shock = 0x20,
        Stun = 0x40,
        Drunk = 0x80,
        Ghost = 0x200,
    }

    public ActorState actorState = ActorState.Stand;
    public ActorState lastActorState = ActorState.Default;
    public DebuffState debuffState = DebuffState.Default;
    public GrabState GrabState = GrabState.None;
    #endregion

    #region ����

    #endregion

    #region ������Ʈ

    public Rigidbody hip;
    public BodyHandler bodyHandler;
    //Moving���� ������
    public Transform CameraTransform;
    public CameraControl CameraControl;
    AudioListener _audioListener;
    #endregion
    void Awake()
    {
        Init();

        //null�� �ƴ϶�� AudioListener ����
        Transform SoundListenerTransform = transform.Find("GreenHead");
        if (SoundListenerTransform != null)
            _audioListener = SoundListenerTransform.gameObject.AddComponent<AudioListener>();
    }

    void Init()
    {
        _audioListener = GetComponent<AudioListener>();
        bodyHandler = GetComponent<BodyHandler>();
        CameraControl = GetComponent<CameraControl>();

    }

    void Start()
    {
        CameraTransform = CameraControl.CameraArm;
    }

    void FixedUpdate()
    {
        //switch (actorState)
        //{
        //    case ActorState.Dead:
        //        break;
        //    case ActorState.Stand:
        //        Stand();
        //        break;
        //    case ActorState.Walk:
        //        Move();
        //        break;
        //    case ActorState.Run:
        //        Move();
        //        break;
        //    case ActorState.Jump:
        //        Jump();
        //        break;
        //    case ActorState.Fall:
        //        break;
        //    case ActorState.Climb:
        //        break;
        //    case ActorState.Roll:
        //        break;
        //}

        //lastActorState = actorState;

        //OnChangeStaminaBar();
    }

    void LateUpdate()
    {
        CameraControl.LookAround(bodyHandler.Hip.transform.position);
        CameraControl.CursorControl();
    }

    // << : Stand ���·� ��ȯ �ϸ� õõ�� ���ߴ� ���� / ���� Idle ���ٰ� ���� ����
    //public void Stand()
    //{
    //    if (isStateChange)
    //    {
    //        _idleTimer = 0f;
    //    }
    //    if (_idleTimer < 30f)
    //    {
    //        _idleTimer = Mathf.Clamp(_idleTimer + Time.deltaTime, -60f, 30f);
    //    }
    //    if (actorState == ActorState.Run && !leftGrab && !rightGrab)
    //    {
    //    }
    //    else
    //    {
    //        // Ilde -> UpdatePhysics�� �־�� �ҵ�
    //        AlignToVector(bodyHandler.Head.PartRigidbody, -bodyHandler.Head.transform.up, _moveDir + new Vector3(0f, 0.2f, 0f), 0.1f, 2.5f * 1);
    //        AlignToVector(bodyHandler.Head.PartRigidbody, bodyHandler.Head.transform.forward, Vector3.up, 0.1f, 2.5f * 1);
    //        AlignToVector(bodyHandler.Chest.PartRigidbody, -bodyHandler.Chest.transform.up, _moveDir, 0.1f, 4f * 1);
    //        AlignToVector(bodyHandler.Chest.PartRigidbody, bodyHandler.Chest.transform.forward, Vector3.up, 0.1f, 4f * 1);
    //        AlignToVector(bodyHandler.Waist.PartRigidbody, -bodyHandler.Waist.transform.up, _moveDir, 0.1f, 4f * 1);
    //        AlignToVector(bodyHandler.Waist.PartRigidbody, bodyHandler.Waist.transform.forward, Vector3.up, 0.1f, 4f * 1);
    //        AlignToVector(bodyHandler.Hip.PartRigidbody, bodyHandler.Hip.transform.forward, Vector3.up, 0.1f, 3f * 1);
    //    }

    //    //������ �ƴҶ� �����߰��ؾ���
    //    /*if (_hip.velocity.magnitude > 1f)
    //        _hip.velocity = _hip.velocity.normalized * _hip.velocity.magnitude * 0.6f;*/
    //}
    // >> : Stand
}
