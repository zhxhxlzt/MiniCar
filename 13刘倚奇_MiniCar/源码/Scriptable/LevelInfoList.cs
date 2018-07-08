using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "InfoList", menuName = "Level/InfoList")]
public class LevelInfoList : ScriptableObject {
    
    public List<LevelInfo> LevelInfo;
    
    //查找关卡信息
    public LevelInfo FindLevelInfo( string levelName )
    {
        foreach ( var item in LevelInfo )
        {
            if(item.LevelName == levelName)
            {
                return item;
            }
        }

        return null;
    }

    //加载所有关卡数据
    public void LoadLevelData()
    {
        foreach ( var item in LevelInfo )
        {
            item.LoadLevelData();
        }
    }

    //保存所有关卡数据
    public void SaveLevelData()
    {
        foreach ( var item in LevelInfo )
        {
            item.SaveLevelData();
        }
    }

    //重置所有关卡数据
    public void ResetLevels()
    {
        foreach ( var item in LevelInfo )
        {
            item.Reset();
        }
    }

}
