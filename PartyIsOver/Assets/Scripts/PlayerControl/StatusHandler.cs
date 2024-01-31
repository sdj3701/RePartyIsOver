using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Actor;
using static InteractableObject;

public class StatusHandler : MonoBehaviourPun
{
    private float _damageModifer = 1f;
    private float _knockoutThreshold = 15f;
    private float _healthDamage;
    private float _maxSpeed;

    public Actor actor;

    public bool invulnerable = false;
    public bool _isDead;


    // �ʱ� ������
    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();

   
    // ���� DebuffTime Actor������ ����� ����
    private float _stunTime;
    private float _burnTime;
    private float _freezeTime;
    private float _powerUpTime;
    private float _drunkTime;
    private float _shockTime;

    public float BurnDamage;


    public Transform PlayerTransform;
    public GameObject EffectObject = null;

    AudioClip _audioClip = null;
    AudioSource _audioSource;


    public Context Context;
    Stun stunInStance;
    Burn burnInStance;
    Ice IceInStance;
    PowerUp powerUpInStance;
    Drunk drunkInStance;
    Shock shockInStance;
    Exhausted exhaustedInStance;


    private void Init()
    {
        StatusData data = Managers.Resource.Load<StatusData>("ScriptableObject/StatusData");
        
        _stunTime = data.StunTime;
        _burnTime = data.BurnTime;
        _freezeTime = data.FreezeTime;
        _powerUpTime = data.PowerUpTime;
        _drunkTime = data.DrunkTime;
        _shockTime = data.ShockTime;
    }

    private void Awake()
    {
        Init();

        PlayerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();
        Context = GetComponent<Context>();
        stunInStance = gameObject.AddComponent<Stun>();
        burnInStance = gameObject.AddComponent<Burn>();
        IceInStance = gameObject.AddComponent<Ice>();
        powerUpInStance = gameObject.AddComponent<PowerUp>();
        drunkInStance = gameObject.AddComponent<Drunk>();
        shockInStance = gameObject.AddComponent<Shock>();
        exhaustedInStance = gameObject.AddComponent<Exhausted>();
    }

    void Start()
    {
        actor = transform.GetComponent<Actor>();
        _maxSpeed = actor.PlayerController.RunSpeed;

        actor.BodyHandler.BodySetup();

        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            _xPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }
    }

    private void LateUpdate()
    {

        // ��ħ ����� Ȱ��ȭ/��Ȱ��ȭ
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (actor.Stamina <= 0)
            {
                actor.debuffState |= DebuffState.Exhausted;

                Debug.Log((actor.debuffState & DebuffState.Exhausted) == DebuffState.Exhausted);
                if ((actor.debuffState & DebuffState.Exhausted) == DebuffState.Exhausted)
                {
                    if (actor.GrabState != Define.GrabState.PlayerLift)
                    {
                        actor.GrabState = Define.GrabState.None;
                        actor.Grab.GrabResetTrigger();
                    }
                    actor.debuffState |= Actor.DebuffState.Exhausted;
                    photonView.RPC("RPCExhaustedCreate", RpcTarget.All);
                }
            }
        }
    }

    // ����� ��������(trigger)
    public void AddDamage(InteractableObject.Damage type, float damage, GameObject causer=null)
    {
        // ������ üũ
        damage *= _damageModifer;

        if (!invulnerable && actor.actorState != Actor.ActorState.Dead && !((actor.debuffState & Actor.DebuffState.Stun) == DebuffState.Stun))
        {
            _healthDamage += damage;
        }

        if (_healthDamage != 0f)
            UpdateHealth();

        if (actor.actorState != Actor.ActorState.Dead)
        {
            // �����̻� üũ
            DebuffCheck(type);
            DebuffAction();
            //CheckProjectile(causer);
        }

        photonView.RPC("InvulnerableState", RpcTarget.All, 0.5f);
        actor.InvokeStatusChangeEvent();
    }

    void CheckProjectile(GameObject go)
    {
        if (go.GetComponent<ProjectileStandard>() != null)
        {
            go.GetComponent<ProjectileStandard>().DestoryProjectileTrigger();
        }
    }

    [PunRPC]
    void PlayerDebuffSound(string path)
    {
        _audioClip = Managers.Sound.GetOrAddAudioClip(path);
        _audioSource.clip = _audioClip;
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);
    }

    public void DebuffCheck(InteractableObject.Damage type)
    {
        switch (type)
        {
            case Damage.Ice: // ����
                actor.debuffState |= Actor.DebuffState.Ice;
                break;
            case Damage.PowerUp: // �Ҳ�
                actor.debuffState |= Actor.DebuffState.PowerUp;
                break;
            case Damage.Burn: // ȭ��
                actor.debuffState |= Actor.DebuffState.Burn;
                break;
            case Damage.Shock: // ����
                    if ((actor.debuffState & DebuffState.Stun) == DebuffState.Stun || (actor.debuffState & DebuffState.Drunk) == DebuffState.Drunk)
                    break;
                else
                    actor.debuffState |= Actor.DebuffState.Shock;
                break;
            case Damage.Stun: // ����
                if ((actor.debuffState & DebuffState.Shock) == DebuffState.Shock || (actor.debuffState & DebuffState.Drunk) == DebuffState.Drunk)
                    break;
                else
                    actor.debuffState |= Actor.DebuffState.Stun;
                break;
            case Damage.Drunk: // ����
                if ((actor.debuffState & DebuffState.Stun) == DebuffState.Stun || (actor.debuffState & DebuffState.Shock) == DebuffState.Shock)
                    break;
                else
                {
                    actor.debuffState |= Actor.DebuffState.Drunk;
                }
                break;
        }
    }

    public void DebuffAction()
    {
        foreach (Actor.DebuffState state in System.Enum.GetValues(typeof(Actor.DebuffState)))
        {
            Actor.DebuffState checking = actor.debuffState & state;
            switch (checking)
            {
                case Actor.DebuffState.Default:
                    break;
                case Actor.DebuffState.PowerUp:
                    photonView.RPC("RPCPowerUpCreate", RpcTarget.All);
                    break;
                case Actor.DebuffState.Burn:
                    photonView.RPC("RPCBurnCreate", RpcTarget.All);
                    break;
                case Actor.DebuffState.Shock:
                    photonView.RPC("RPCShockCreate", RpcTarget.All);
                    break;
                case Actor.DebuffState.Stun:
                    EnterUnconsciousState();
                    break;
                case Actor.DebuffState.Ghost:
                    break;
                case Actor.DebuffState.Drunk:
                    photonView.RPC("RPCPoisonCreate", RpcTarget.All);
                    break;
                case Actor.DebuffState.Ice:
                    photonView.RPC("RPCIceCreate", RpcTarget.All);
                    break;
            }
        }
    }

    [PunRPC]
    void RPCPoisonCreate()
    {
        Context.ChangeState(drunkInStance, _drunkTime);
    }

    [PunRPC]
    void RPCShockCreate()
    {
        actor.Grab.GrabResetTrigger();
        Context.ChangeState(shockInStance, _shockTime);
    }

    [PunRPC]
    void RPCExhaustedCreate()
    {
        Context.ChangeState(exhaustedInStance);
    }
    [PunRPC]
    void RPCPowerUpCreate()
    {
        Context.ChangeState(powerUpInStance, _powerUpTime);
    }

    [PunRPC]
    void RPCBurnCreate()
    {
        actor.Grab.GrabResetTrigger();
        Context.ChangeState(burnInStance, _burnTime);
    }

    [PunRPC]
    void RPCIceCreate()
    {
        Context.ChangeState(IceInStance, _freezeTime);
    }

    [PunRPC]
    public void DestroyEffect(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        EffectObject = null;
    }

    public void EffectObjectCreate(string path)
    {
        EffectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
        //effectObject.transform.position = playerTransform.position;
    }
    [PunRPC]
    public void MoveEffect()
    {
        //LateUpdate���� �ʰ� ������ �Ǿ NullReference�� ���� ���� if ���� �־���
        if (EffectObject != null && EffectObject.name == "Stun_loop")
            EffectObject.transform.position = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y + 1, PlayerTransform.position.z);
        else if (EffectObject != null && EffectObject.name == "Fog_frost")
        {
            EffectObject.transform.position = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y - 2, PlayerTransform.position.z);
        }
        else
            EffectObject.transform.position = PlayerTransform.position;
    }


    public void UpdateHealth()
    {
        if (_isDead)
            return;

        //���� ü�� �޾ƿ���
        float tempHealth = actor.Health;

        //�������°� �ƴҶ��� ������ ����
        if (tempHealth > 0f && !invulnerable)
            tempHealth -= _healthDamage;

        float realDamage = actor.Health - tempHealth;

        //����� ü���� 0���� ������ Death��
        if (tempHealth <= 0f)
        {
            KillPlayer();
        }
        else
        {
            //�������°� �ƴҶ� ���� �̻��� �������� ������ ����
            if (!((actor.debuffState & Actor.DebuffState.Stun) == DebuffState.Stun))
            {

                if (realDamage >= _knockoutThreshold)
                {
                    if ((actor.debuffState & DebuffState.Ice) == DebuffState.Ice) //�����̻� �Ŀ� �߰�
                        return;

                    actor.debuffState |= Actor.DebuffState.Stun;
                }
            }
        }
        actor.Health = Mathf.Clamp(tempHealth, 0f, actor.MaxHealth);

        _healthDamage = 0f;
    }
    [PunRPC]
    IEnumerator InvulnerableState(float time)
    {
        invulnerable = true;
        yield return new WaitForSeconds(time);
        invulnerable = false;
    }

    void KillPlayer()
    {
        actor.actorState = Actor.ActorState.Dead;
        _isDead = true;
        actor.Grab.GrabResetTrigger();
        actor.InvokeDeathEvent();
    }

    void EnterUnconsciousState()
    {
        //������ ����Ʈ�� ���� ���� �߰�

        //actor.debuffState = Actor.DebuffState.Stun;
        actor.Grab.GrabResetTrigger();
        photonView.RPC("ChangeStateMachines", RpcTarget.All, _stunTime);
        //StartCoroutine(ResetBodySpring());
        actor.BodyHandler.LeftHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        actor.BodyHandler.LeftForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        actor.BodyHandler.RightHand.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        actor.BodyHandler.RightForearm.PartRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    [PunRPC]
    void ChangeStateMachines(float durationTime)
    {
        Context.ChangeState(stunInStance, durationTime);
    }

    void SetJointSpring(float percentage)
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;
        int j = 0;

        //������ ȸ���� ��� ���� �����ÿ� �ۼ�Ƽ���� 0�����ؼ� ���
        for (int i = 0; i < actor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            angularXDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = _xPosSpringAry[j] * percentage;
            actor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = _yzPosSpringAry[j] * percentage;
            actor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;

            j++;
        }
    }

    public IEnumerator ResetBodySpring()
    {
        SetJointSpring(0f);
        yield return null;
    }

    public IEnumerator RestoreBodySpring(float _springLerpTime = 1f)
    {
        float startTime = Time.time;
        float springLerpDuration = _springLerpTime;

        while (Time.time - startTime < springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            SetJointSpring(percentage);
            yield return null;
        }
    }
}