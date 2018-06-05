using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InfoList", menuName = "Level/InfoList")]
public class LevelInfoList : ScriptableObject {

    [SerializeField] private List<LevelInfo> m_list;    //所有关卡信息
    
    //返回关卡信息
	public LevelInfo GetLevelInfo(string levelName)
    {
        Debug.Log( levelName );
        foreach ( var item in m_list )
        {
            Debug.Log( item.name );
            if( item.name == levelName )
            {
                Debug.Log( "找到LevelInfo" );
                return item;
            }
        }

        Debug.Log( "未找到LevelInfo" );
        return null;
    }

    //重置关卡记录
    public void ResetLevelInfo()
    {
        foreach ( var item in m_list )
        {
            item.Reset();
        }
    }
}
