using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using static AniFrameData;
using static CharacterPhysicsMotion;

public class BaseState
{
    protected float maxSpeed = 2f;
    protected float applyedForce = 800f;
    public string Name;
    protected StateMachine stateMachine;
    protected MovementSM sm;

    protected Vector3 runVectorForce2 = new Vector3(0f, 0f, 0.2f);
    protected Vector3 runVectorForce5 = new Vector3(0f, 0f, 0.4f);
    protected Vector3 runVectorForce10 = new Vector3(0f, 0f, 0.8f);
    protected Vector3 moveDir;
    protected Vector3 moveInput;

    public BaseState(string name, StateMachine stateMachine)
    {
        sm = (MovementSM)stateMachine;
        this.Name = name;  
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit() { }

    protected void AlignToVector(Rigidbody part, Vector3 alignmentVector, Vector3 targetVector, float stability, float speed)
    {
        if (part == null)
        {
            return;
        }
        Vector3 vector = Vector3.Cross(Quaternion.AngleAxis(part.angularVelocity.magnitude * 57.29578f * stability / speed, part.angularVelocity) * alignmentVector, targetVector * 10f);
        if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z))
        {
            part.AddTorque(vector * speed * speed);
            {
                Debug.DrawRay(part.position, alignmentVector * 0.2f, Color.red, 0f, depthTest: false);
                Debug.DrawRay(part.position, targetVector * 0.2f, Color.green, 0f, depthTest: false);
            }
        }
    }

    // ���ڵ带 ����� �ұ�?
    protected void AnimateWithDirectedForce(CharacterPhysicsMotion[] _forceSpeed, int _elementCount, Vector3 _dir = new Vector3(), float _punchpower = 1)
    {
        for (int i = 0; i < _forceSpeed[_elementCount].ReferenceRigidbodies.Length; i++)
        {
            if (_forceSpeed[_elementCount].ActionForceDirections[i] == Define.ForceDirection.Zero || _forceSpeed[_elementCount].ActionForceDirections[i] == Define.ForceDirection.ZeroReverse)
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_dir * _forceSpeed[_elementCount].ActionForceValues[i], ForceMode.Impulse);
            else
            {
                Vector3 _direction = GetForceDirection(_forceSpeed[_elementCount], i);
                _forceSpeed[_elementCount].ActionRigidbodies[i].AddForce(_direction * _forceSpeed[_elementCount].ActionForceValues[i], ForceMode.Impulse);
            }
        }
    }
    protected void AnimateWithDirectedForce(CharacterPhysicsMotion _forceSpeed, Vector3 _dir = new Vector3(), float _punchpower = 1)
    {
        for (int i = 0; i < _forceSpeed.ReferenceRigidbodies.Length; i++)
        {
            if (_forceSpeed.ActionForceDirections[i] == Define.ForceDirection.Zero || _forceSpeed.ActionForceDirections[i] == Define.ForceDirection.ZeroReverse)
                _forceSpeed.ActionRigidbodies[i].AddForce(_dir * _forceSpeed.ActionForceValues[i], ForceMode.Impulse);
            else
            {
                Vector3 _direction = GetForceDirection(_forceSpeed, i);
                _forceSpeed.ActionRigidbodies[i].AddForce(_direction * _forceSpeed.ActionForceValues[i], ForceMode.Impulse);
            }   
        }
    }

    protected Vector3 GetForceDirection(CharacterPhysicsMotion data, int index)
    {
        Define.ForceDirection _rollState = data.ActionForceDirections[index];
        Vector3 _direction;

        switch (_rollState)
        {
            case Define.ForceDirection.Zero:
                _direction = new Vector3(0, 0, 0);
                break;
            case Define.ForceDirection.ZeroReverse:
                _direction = new Vector3(-1, -1, -1);
                break;
            case Define.ForceDirection.Forward:
                _direction = -data.ReferenceRigidbodies[index].transform.up;
                break;
            case Define.ForceDirection.Backward:
                _direction = data.ReferenceRigidbodies[index].transform.up;
                break;
            case Define.ForceDirection.Up:
                _direction = data.ReferenceRigidbodies[index].transform.forward;
                break;
            case Define.ForceDirection.Down:
                _direction = -data.ReferenceRigidbodies[index].transform.forward;
                break;
            case Define.ForceDirection.Left:
                _direction = -data.ReferenceRigidbodies[index].transform.right;
                break;
            case Define.ForceDirection.Right:
                _direction = data.ReferenceRigidbodies[index].transform.right;
                break;
            default:
                _direction = Vector3.zero;
                break;
        }
        return _direction;
    }

    

}
