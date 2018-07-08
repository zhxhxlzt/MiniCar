using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "new_cover_part", menuName = "CarParts/Cover" )]
public class CoverPart : ScriptableObject
{
    [SerializeField] private float airDrag;     //空气阻力
    [SerializeField] private float downForce;   //下压力

    public float AirDrag { get { return airDrag; } }
    public float DownForce { get { return downForce; } }
}
