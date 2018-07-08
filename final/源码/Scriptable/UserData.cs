using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserData")]
public class UserData : ScriptableObject {

    public List<CoverPart> coverList;       //用户车身数据
    public List<MotorPart> motorList;       //用户马达数据
    public List<WheelPart> wheelList;       //用户车轮数据
    public UserCarParts currentCar;         //当前赛车配置

    public LevelInfoList levelInfoList;     //用户所有关卡信息
    
    //重设所有用户数据
    public void Reset()
    {
        //重设关卡信息
        
        //重设赛车零件配置
        currentCar.Reset();
    }
}
