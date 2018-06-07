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

    //保存所有关卡信息到本地
    public void Save()
    {
        Debug.Log( "已保存" );
        PlayerPrefs.SetInt( "LevelInfoSaved", 1 );  //设置为已保存
        foreach ( var item in m_list )
        {
            item.Save();
        }
    }

    //从本地读取所有关卡信息，只有已保存的数据才能载入
    public void Load()
    {
        Debug.Log( PlayerPrefs.GetInt( "LevelInfoSaved", 0 ) );
        if( PlayerPrefs.GetInt( "LevelInfoSaved", 0) == 1)  //如果已保存，则载入
        {
            Debug.Log( "已载入" );
            foreach ( var item in m_list )
            {
                item.Load();
            }
            PlayerPrefs.SetInt( "LevelInfoSaved", 0 );  //载入后，设置为未保存
        }
    }

    //重置关卡记录
    public void ResetLevelInfo()
    {
        PlayerPrefs.SetInt( "LevelInfoSaved", 0 );  //在重置后，设置为未保存
        foreach ( var item in m_list )
        {
            item.Reset();
        }
    }
}
