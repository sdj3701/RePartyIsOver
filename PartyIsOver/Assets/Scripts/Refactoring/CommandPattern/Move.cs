using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    //public MovingAnimation MovingAnimation;
    MovementSM sm;

    private void Awake()
    {
        sm = GetComponent<MovementSM>();
        sm.MovingState = new MovingAnimation(sm);
    }

    public void TurnOn()
    {
        Debug.Log("전등이 켜졌습니다.");
    }

    public void TurnOff()
    {
        Debug.Log("전등이 꺼졌습니다.");
    }

}
