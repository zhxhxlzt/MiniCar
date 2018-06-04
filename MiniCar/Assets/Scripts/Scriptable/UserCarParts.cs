using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "new_user_parts", menuName = "CarParts/UserParts" )]
public class UserCarParts : ScriptableObject
{
    public WheelPart wheel;     //车轮零件
    public CoverPart cover;     //车身零件
    public MotorPart motor;     //马达零件
}