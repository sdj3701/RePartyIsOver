using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class TaskAttack : Node
{
    private Animator _animator;

    private Transform _lastTarget;
    //��ģ���� �ٲ����
    private PlayerController playerController;
    StatusHandler playerstatu;


    private float _attackTime = 1f;
    private float _attackCounter = 0f;

    public TaskAttack(Transform transform)
    {
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if(target != _lastTarget)
        {
            //� �Լ��� �ҷ����� �ٷ� ������Ű�� �Լ��� �ҷ����� ��ũ��Ʈ�� �ٲ����
            playerController = target.GetComponent<PlayerController>();
            _lastTarget = target;
        }
        
        _attackCounter += Time.deltaTime;
        if(_attackCounter >= _attackTime)
        {
            playerstatu = playerController.GetComponent<StatusHandler>();
            //bool playerIsDead = playerstatu.HasFreeze;
            // hip �ϴ� ��ũ��Ʈ �߰�

            //bool playerIsDead �� ���̸���
            if(true)
            {
                ClearData("target");
                _animator.SetBool("Attacking", false);
                _animator.SetBool("Walking", true);
            }
            else
            {
                _attackCounter = 0f;
            }
        }

        state = NodeState.Running;
        return state;
    }

}
