using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : MonoBehaviourPun, IDebuffState
{
    public Actor MyActor { get; set; }
    public float CoolTime { get; set; }
    public GameObject effectObject { get; set; }
    public Transform playerTransform { get; set; }

    AudioClip _audioClip = null;
    AudioSource _audioSource;
    private List<float> _xPosSpringAry = new List<float>();
    private List<float> _yzPosSpringAry = new List<float>();

    public void EnterState()
    {
        effectObject = null;
        playerTransform = this.transform.Find("GreenHip").GetComponent<Transform>();
        Transform SoundSourceTransform = transform.Find("GreenHip");
        _audioSource = SoundSourceTransform.GetComponent<AudioSource>();

        for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            _xPosSpringAry.Add(MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive.positionSpring);
            _yzPosSpringAry.Add(MyActor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive.positionSpring);
        }

        TransferDebuffToPlayer((int)InteractableObject.Damage.Shock);
        PlayerDebuffSound("PlayerEffect/electronic_02");
        InstantiateEffect("Effects/Lightning_aura");

        JointDrive angularXDrive;
        JointDrive angularYZDrive;

        for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
        {
            if (i >= (int)Define.BodyPart.Hip && i <= (int)Define.BodyPart.Head) continue;
            if (i == (int)Define.BodyPart.Ball) continue;

            angularXDrive = MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = 0f;
            MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = MyActor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = 0f;
            MyActor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;
        }
    }

    public void UpdateState()
    {
        if (effectObject != null)
            effectObject.transform.position = playerTransform.position;
        
        if (Random.Range(0, 20) > 10)
        {
            for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
            {
                if (i >= (int)Define.BodyPart.Hip && i <= (int)Define.BodyPart.Head) continue;
                if (i == (int)Define.BodyPart.LeftFoot ||
                    i == (int)Define.BodyPart.RightFoot ||
                    i == (int)Define.BodyPart.Ball) continue;

                MyActor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(20, 0, 0);
            }
        }
        else
        {
            for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
            {
                if (i >= (int)Define.BodyPart.Hip && i <= (int)Define.BodyPart.Head) continue;
                if (i == (int)Define.BodyPart.LeftFoot ||
                    i == (int)Define.BodyPart.RightFoot ||
                    i == (int)Define.BodyPart.Ball) continue;
                MyActor.BodyHandler.BodyParts[i].transform.rotation = Quaternion.Euler(-20, 0, 0);
            }
        }
    }

    public void ExitState()
    {
        StartCoroutine("RestoreBodySpring");

        TransferDebuffToPlayer((int)InteractableObject.Damage.Default);

        MyActor.actorState = Actor.ActorState.Stand;
        MyActor.debuffState &= ~Actor.DebuffState.Shock;

        RemoveObject("Lightning_aura");
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

    public IEnumerator RestoreBodySpring()
    {
        float startTime = Time.time;
        float springLerpDuration = 0.07f;

        while (Time.time < startTime + springLerpDuration)
        {
            float elapsed = Time.time - startTime;
            float percentage = elapsed / springLerpDuration;
            photonView.RPC("ASetJointSpring", RpcTarget.All, percentage);
            yield return null;
        }
    }

    [PunRPC]
    public IEnumerator AResetBodySpring()
    {
        photonView.RPC("ASetJointSpring", RpcTarget.All, 0f);
        yield return null;
    }
    [PunRPC]
    void ASetJointSpring(float percentage)
    {
        JointDrive angularXDrive;
        JointDrive angularYZDrive;
        int j = 0;

        //������ ȸ���� ��� ���� �����ÿ� �ۼ�Ƽ���� 0�����ؼ� ���
        for (int i = 0; i < MyActor.BodyHandler.BodyParts.Count; i++)
        {
            if (i == (int)Define.BodyPart.Hip)
                continue;

            angularXDrive = MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive;
            angularXDrive.positionSpring = _xPosSpringAry[j] * percentage;
            MyActor.BodyHandler.BodyParts[i].PartJoint.angularXDrive = angularXDrive;

            angularYZDrive = MyActor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive;
            angularYZDrive.positionSpring = _yzPosSpringAry[j] * percentage;
            MyActor.BodyHandler.BodyParts[i].PartJoint.angularYZDrive = angularYZDrive;

            j++;
        }
    }
}
