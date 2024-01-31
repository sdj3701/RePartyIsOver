using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Context : MonoBehaviourPun
{
    private List<IDebuffState> _currentStateList = new List<IDebuffState>();

    public void SetState(IDebuffState state)
    {
        state.MyActor = GetComponent<Actor>();

        _currentStateList.Add(state);
        state.EnterState();
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _currentStateList.Count; i++)
        {
            var state = _currentStateList[i];

            if (state != null)
            {
                if (state.ToString().Contains("Exhausted") && state.MyActor.Stamina == 100)
                {
                    state.ExitState();
                    _currentStateList[i] = null;
                }

                if (state.CoolTime > 0f)
                    state.CoolTime -= Time.deltaTime;

                if (state.CoolTime <= 0f)
                {
                    if(!state.ToString().Contains("Exhausted"))
                    {
                        state.ExitState();
                        _currentStateList[i] = null;
                    }
                }
                state.UpdateState();
            }
        }
        _currentStateList.RemoveAll(state => state == null);
    }

    public void ChangeState(IDebuffState newState, float time = 0)
    {
        foreach(var state in _currentStateList)
        {
            //���� ���°� �ߺ��Ǹ� ���� �ø��� �ͺ��� �׳� �ִ� ���� ������ �� ���� �����̸� return
            if (state != null)
            {
                if (state == newState)
                    return;
            }

            //���� ���� ���°� Ice ���� ������ ���µ��� �ϴ� �� ����
            if(newState.ToString().Contains("Ice"))
            {
                state.ExitState();
                _currentStateList.Remove(state);
            }

            //���� �����ε� ������ ������ return;
            if(state.ToString().Contains("Shock"))
            {
                if (newState.ToString().Contains("Stun"))
                    return;
            }
        }
        newState.CoolTime = time;

        SetState(newState);
    }
}
