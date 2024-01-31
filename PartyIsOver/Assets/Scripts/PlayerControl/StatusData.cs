using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status Data", menuName = "Scriptable Object/Status Data", order = int.MaxValue)]
public class StatusData : ScriptableObject
{
    [Header("DebuffTime")]
    [SerializeField] private float _stunTime;
    public float StunTime { get { return _stunTime; } }

    [SerializeField] private float _burnTime;
    public float BurnTime { get { return _burnTime; } }

    [SerializeField] private float _freezeTime;
    public float FreezeTime { get { return _freezeTime; } }

    [SerializeField] private float _powerUpTime;
    public float PowerUpTime { get { return _powerUpTime; } }

    [SerializeField] private float _drunktime;
    public float DrunkTime { get { return _drunktime; } }

    [SerializeField] private float _shockTime;
    public float ShockTime { get { return _shockTime; } }
}
