using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "new_user_parts", menuName = "CarParts/UserParts" )]
public class UserCarParts : ScriptableObject
{
    //数据路径
    public string wheelPath = "GameDatum/CarParts/Wheels/";
    public string coverPath = "GameDatum/CarParts/Covers/";
    public string motorPath = "GameDatum/CarParts/Motors/";

    public WheelPart wheel;     //车轮零件
    public CoverPart cover;     //车身零件
    public MotorPart motor;     //马达零件

    //重置方法
    public void Reset()
    {
        wheel = Resources.Load<WheelPart>( wheelPath + "DefaultWheel" );
        cover = Resources.Load<CoverPart>( coverPath + "DefaultCover" );
        motor = Resources.Load<MotorPart>( motorPath + "DefaultMotor" );
    }
}