using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class TaskAttack : Node
{
    private Animator _animator;

    private Transform _lastTarget;
    private Transform _transform;
    private PlayerCharacter playerCharacter;

    private float _attackTime = 1f;
    private float _attackCounter = 0f;

    public TaskAttack(Transform transform)
    {
        _transform = transform;
        _animator = transform.GetComponent<Animator>();
    }

    public override NodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");
        if(target != _lastTarget)
        {
            playerCharacter = target.parent.GetComponent<PlayerCharacter>();
            _lastTarget = target;
        }

        _attackCounter += Time.deltaTime;
        if(_attackCounter >= _attackTime)
        {
            bool enemyIsDead = playerCharacter.TakeHit();
            if (enemyIsDead)
            {
                ClearData("target");
                _animator.SetBool("Attacking", false);
                _animator.SetBool("Walking", true);
            }
            else
            {
                _attackCounter = 0f;
                // << : 폭탄이여서 한번 터지면 삭제
                Object.Destroy(_transform.gameObject);
                // >> : 
            }
        }
        state = NodeState.RUNNIG;
        return state;
    }
}