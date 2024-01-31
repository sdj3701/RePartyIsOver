using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using UnityEngine.SceneManagement;
using System.Numerics;


public class Actor : MonoBehaviourPun, IPunObservable
{
    //��������Ʈ ����
    public delegate void ChangePlayerStatus(float HP,float Stamina, DebuffState debuffstate, int viewID);
    //��������Ʈ�� �̿��� �̺�Ʈ ����
    public event ChangePlayerStatus OnChangePlayerStatus;

    public delegate void KillPlayer(int viewID);
    public event KillPlayer OnKillPlayer;

    public delegate void ChangeStaminaBar();
    public event ChangeStaminaBar OnChangeStaminaBar;

    public AudioListener _audioListener;

    public StatusHandler StatusHandler;
    public BodyHandler BodyHandler;
    public PlayerController PlayerController;
    public Grab Grab;
    public CameraControl CameraControl;

    public enum ActorState
    {
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
        Default =   0x0,
        PowerUp =   0x1,
        Burn =      0x2,
        Exhausted = 0x4,
        Slow =      0x8,
        Ice =       0x10,
        Shock =     0x20, 
        Stun =      0x40, 
        Drunk =     0x80,  
        Ghost =     0x200,
    }

    public GrabState GrabState = GrabState.None;

    //���ݷ� ����(100�� ����)
    [SerializeField]
    private float _damageReduction = 0f;
    public float DamageReduction { get { return _damageReduction; } set { _damageReduction = value; } }

    [SerializeField]
    private float _playerAttackPoint = 1f;
    public float PlayerAttackPoint { get { return _playerAttackPoint; } set { _damageReduction = value; } }


    // ü��
    [SerializeField]
    private float _health;
    [SerializeField]
    private float _maxHealth = 200f;
    public float Health { get { return _health; } set { _health = value; } }
    public float MaxHealth { get { return _maxHealth; } }

    [Header("Stamina Recovery")]
    public float RecoveryTime = 0.1f;
    public float RecoveryStaminaValue = 1f;
    public float ExhaustedRecoveryTime = 0.2f;
    float currentRecoveryTime;
    float currentRecoveryStaminaValue; 
    float accumulatedTime = 0.0f;

    // ���׹̳�
    [SerializeField]
    private float _stamina;
    [SerializeField]
    private float _maxStamina = 100f;
    public float Stamina { get { return _stamina; } set { _stamina = value; } }
    public float MaxStamina { get { return _maxStamina; } }

    // ���罺��
    [SerializeField]
    private int _magneticStack = 0;
    public int MagneticStack { get { return _magneticStack; } set { _magneticStack = value; } }

    //�⺻ �ʱ�ȭ
    public ActorState actorState = ActorState.Stand;
    public ActorState lastActorState = ActorState.Run;
    public DebuffState debuffState = DebuffState.Default;

    public static GameObject LocalPlayerInstance;

    //Layer �߰�
    public static int LayerCnt = (int)Define.Layer.Player1;

    //���� ��ȯ �̺�Ʈ
    public void InvokeStatusChangeEvent()
    {
        if (OnChangePlayerStatus != null)
        {
            OnChangePlayerStatus(_health, _stamina, debuffState, photonView.ViewID);
        }
        else
        {
            Debug.Log(photonView.ViewID + " OnChangePlayerStatus �̺�Ʈ null");
            return;
        }
    }

    public void InvokeDeathEvent()
    {
        if (OnKillPlayer == null)
        {
            Debug.Log(photonView.ViewID + " OnKillPlayer �̺�Ʈ null");
            return;
        }

        OnKillPlayer(photonView.ViewID);
    }

    private void Awake()
    {
        Transform SoundListenerTransform = transform.Find("GreenHead");
        if(SoundListenerTransform != null)
            _audioListener = SoundListenerTransform.gameObject.AddComponent<AudioListener>();
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;

            if (CameraControl == null)
            {
                Debug.Log("ī�޶� ��Ʈ�� �ʱ�ȭ");
                CameraControl = GetComponent<CameraControl>();
            }
        }
        else
        {
            // ���� ����
            Destroy(_audioListener);
            //_audioListener.enabled = false;
        }

        //if (SceneManager.GetActiveScene().name != "[4]Room")
        //    DontDestroyOnLoad(this.gameObject);

        BodyHandler = GetComponent<BodyHandler>();
        StatusHandler = GetComponent<StatusHandler>();
        PlayerController = GetComponent<PlayerController>();
        Grab = GetComponent<Grab>();
        ChangeLayerRecursively(gameObject, LayerCnt++);
        Init();
    }

    private void Init()
    {
        PlayerStatData statData = Managers.Resource.Load<PlayerStatData>("ScriptableObject/PlayerStatData");

        _health = statData.Health;
        _stamina = statData.Stamina;
        _maxHealth = statData.MaxHealth;
        _maxStamina = statData.MaxStamina;
        _damageReduction = statData.DamageReduction;
        _playerAttackPoint = statData.PlayerAttackPoint;
    }

    private void ChangeLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursively(child.gameObject, layer);
        }
    } 

    private void Update()
    {
        if (!photonView.IsMine || actorState == ActorState.Dead) return;

        if(CameraControl == null || BodyHandler == null) return;
        CameraControl.LookAround(BodyHandler.Hip.transform.position);
        CameraControl.CursorControl();
    }

    void RecoveryStamina()
    {
        //ȸ�����ִ� ��ġ
        if (!((debuffState & DebuffState.Exhausted) == DebuffState.Exhausted))
        {
            currentRecoveryTime = RecoveryTime;
            currentRecoveryStaminaValue = RecoveryStaminaValue;
        }
        else
        {
            PlayerController.isRun = false;
            currentRecoveryTime = ExhaustedRecoveryTime;
            currentRecoveryStaminaValue = RecoveryStaminaValue;
        }
    }
    [PunRPC]
    void SetStemina(float amount)
    {
        Stamina = amount;
    }

    [PunRPC]
    void DecreaseStamina(float amount)
    {
        Stamina -= amount;
    }

    [PunRPC]
    void RecoverStamina(float amount)
    {
        Stamina += amount;
    }

    private void FixedUpdate()
    {
        if (actorState == ActorState.Dead) return;
            
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (_stamina <= 0)
            {
                //��Ÿ�� �Ұ���
                Grab.GrabResetTrigger();
                GrabState = GrabState.None;
            }

            //ȸ���ϴ� ��ġ�� ����
            RecoveryStamina();

            accumulatedTime += Time.fixedDeltaTime;
            //Time.fixedDeltaTime(0.02�� �������� ��� �ݺ�) >= ����ȸ���ð�
            //0.02�� ��� ���ؼ� >= 0.1,0.2�� ���� Ŀ���� 
            if (accumulatedTime >= currentRecoveryTime)
            {
                //�ٰų� ��� ���¿�����
                if (actorState == ActorState.Run || GrabState == GrabState.Climb)
                {
                    //�ٰų� ��� �����϶� ���� Ư�� ����� ���°� ������ ��� ���̴� ������ �ִµ� ������ �ɾ ����
                    if ((debuffState & DebuffState.Ice) == DebuffState.Ice || (debuffState & DebuffState.Shock) == DebuffState.Shock)
                    {
                        _stamina -= 0;
                        //photonView.RPC("DecreaseStamina", RpcTarget.All, 0f);
                        Grab.GrabResetTrigger();
                        GrabState = GrabState.None;
                        //PlayerController.isRun = false;
                    }
                    else if(_stamina == 0)
                    {
                        _stamina = -1f;
                        actorState = ActorState.Walk;
                    }
                    else
                        _stamina -= 1;
                        //photonView.RPC("DecreaseStamina", RpcTarget.All, 1f);
                }
                else if (PlayerController._isRSkillCheck || PlayerController.isHeading || PlayerController._isCoroutineDrop)
                    //��ų ���� ȸ�� �Ұ���
                    //photonView.RPC("RecoverStamina",RpcTarget.All, 0f);
                    _stamina += 0;
                else
                    //���¿� �´� ȸ���ϱ�
                    //photonView.RPC("RecoverStamina", RpcTarget.All, currentRecoveryStaminaValue);
                    _stamina += currentRecoveryStaminaValue;
                accumulatedTime = 0f;
            }
            //���׹̳ʰ� �ִ�ġ�� �Ѵ°� ����
            if (_stamina > MaxStamina)
                _stamina = MaxStamina;

            OnChangePlayerStatus(_health, _stamina,  debuffState, photonView.ViewID);
        }


        if (!photonView.IsMine) return;

        OnChangeStaminaBar();

        if (actorState != lastActorState)
        {
            PlayerController.isStateChange = true;
        }
        else
        {
            PlayerController.isStateChange = false;
        }
        switch (actorState)
        {
            case ActorState.Dead:
                break;
            case ActorState.Stand:
                PlayerController.Stand();
                break;
            case ActorState.Walk:
                PlayerController.Move();
                break;
            case ActorState.Run:
                PlayerController.Move();
                break;
            case ActorState.Jump:
                PlayerController.Jump();
                break;
            case ActorState.Fall:
                break;
            case ActorState.Climb:
                break;
            case ActorState.Roll:
                break;
        }

        lastActorState = actorState;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(actorState);
        }
        else
        {
            if (this.actorState != ActorState.Dead)
                this.actorState = (ActorState)stream.ReceiveNext();
        }
    }
}
