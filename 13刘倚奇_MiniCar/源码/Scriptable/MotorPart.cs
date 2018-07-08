using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "new_motor_part", menuName = "CarParts/Motor" )]
public class MotorPart : ScriptableObject
{
    [SerializeField] private float rpm;     //每分钟转速
    [SerializeField] private float motorTorque; //动力扭矩

    public float RPM { get { return rpm; } }
    public float MotorTorque { get { return motorTorque; } }
}
