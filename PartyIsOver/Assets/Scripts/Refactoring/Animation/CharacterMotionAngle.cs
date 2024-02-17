using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotionAngle : MonoBehaviour
{
    public Rigidbody[] StandardRigidbodies;
    public Transform[] ActionDirections;
    public Transform[] TargetDirections;
    public Define.ForceDirection[] ActionRotationDirections;
    public Define.ForceDirection[] TargetRotationDirections;
    public float[] AngleStabilities;
    public float[] AnglePowerValues;
}
