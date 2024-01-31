using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Burn : MonoBehaviourPun , IDebuffState
{
    public Actor MyActor { get; set; }
    public float CoolTime { get; set; }
    public GameObject effectObject { get; set; }
    public Transform playerTransform { get; set; }

    AudioClip _audioClip = null;
    AudioSource _audioSource;

    float _burnDamage = 5f;
    float lastBurnTime;

    public void EnterState()
    {
        effectObject = null;
        lastBurnTime = Time.time;

        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();

        InstantiateEffect("Effects/Fire_large");
        PlayerDebuffSound("PlayerEffect/SFX_FireBall_Projectile");
        TransferDebuffToPlayer((int)InteractableObject.Damage.Burn);
        //StartCoroutine("BurnDmanage");

    }

    public void UpdateState()
    {
        if (effectObject != null)
            effectObject.transform.position = playerTransform.position;

        if (Time.time - lastBurnTime >= 1.0f) // 1�ʰ� ������+�׼�
        {
            MyActor.Health -= _burnDamage;
            MyActor.BodyHandler.Waist.PartRigidbody.AddForce((MyActor.BodyHandler.Hip.transform.right) * 40f, ForceMode.VelocityChange);
            MyActor.BodyHandler.Hip.PartRigidbody.AddForce((MyActor.BodyHandler.Hip.transform.right) * 40f, ForceMode.VelocityChange);
            lastBurnTime = Time.time;
        }
    }

    public void ExitState()
    {
        TransferDebuffToPlayer((int)InteractableObject.Damage.Default);
        MyActor.actorState = Actor.ActorState.Stand;
        MyActor.debuffState &= ~Actor.DebuffState.Burn;

        RemoveObject("Fire_large");
        MyActor.InvokeStatusChangeEvent();
        _audioClip = null;
    }

    public void InstantiateEffect(string path)
    {
        effectObject = Managers.Resource.PhotonNetworkInstantiate($"{path}");
    }
    public void RemoveObject(string name)
    {
        GameObject go = GameObject.Find($"{name}");
        Managers.Resource.Destroy(go);
        effectObject = null;
    }
    void PlayerDebuffSound(string path)
    {
        //���� ���� ����
        _audioClip = Managers.Sound.GetOrAddAudioClip(path);
        _audioSource.clip = _audioClip;
        _audioSource.spatialBlend = 1;
        Managers.Sound.Play(_audioClip, Define.Sound.PlayerEffect, _audioSource);
    }
    public void TransferDebuffToPlayer(int DamageType)
    {
        ChangeDamageModifier((int)Define.BodyPart.LeftFoot, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.RightFoot, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.LeftLeg, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.RightLeg, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.Head, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.LeftHand, DamageType);
        ChangeDamageModifier((int)Define.BodyPart.RightHand, DamageType);
    }
    private void ChangeDamageModifier(int bodyPart, int DamageType)
    {
        switch ((Define.BodyPart)bodyPart)
        {
            case Define.BodyPart.LeftFoot:
                MyActor.BodyHandler.LeftFoot.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.RightFoot:
                MyActor.BodyHandler.RightFoot.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.LeftLeg:
                MyActor.BodyHandler.LeftLeg.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.RightLeg:
                MyActor.BodyHandler.RightLeg.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.LeftThigh:
                break;
            case Define.BodyPart.RightThigh:
                break;
            case Define.BodyPart.Hip:
                break;
            case Define.BodyPart.Waist:
                break;
            case Define.BodyPart.Chest:
                break;
            case Define.BodyPart.Head:
                MyActor.BodyHandler.Head.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.LeftArm:
                break;
            case Define.BodyPart.RightArm:
                break;
            case Define.BodyPart.LeftForeArm:
                break;
            case Define.BodyPart.RightForeArm:
                break;
            case Define.BodyPart.LeftHand:
                MyActor.BodyHandler.LeftHand.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
            case Define.BodyPart.RightHand:
                MyActor.BodyHandler.RightHand.PartInteractable.damageModifier = (InteractableObject.Damage)DamageType;
                break;
        }
    }
}
