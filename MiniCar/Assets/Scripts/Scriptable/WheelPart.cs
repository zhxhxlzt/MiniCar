using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "new_wheel_part", menuName = "CarParts/Wheel" )]
public class WheelPart : ScriptableObject
{
    [SerializeField] [Range( 0, 1 )] private float friction;    //摩擦因数
    [SerializeField] private float brakeTorque; //制动扭矩

    public float Friction { get { return friction; } }
    public float BrakeTorque { get { return brakeTorque; } }
}
