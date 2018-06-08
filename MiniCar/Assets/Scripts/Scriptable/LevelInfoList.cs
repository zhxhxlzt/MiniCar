using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "InfoList", menuName = "Level/InfoList")]
[System.Serializable]
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
        LevelInfoListAccess info = new LevelInfoListAccess( m_list );
        SaveData( info );
    }

    //从本地读取所有关卡信息，只有已保存的数据才能载入
    public void Load()
    {
        LevelInfoListAccess info;

        LoadData( out info );   //加载生成资源

        if(info == null)
        {
            Debug.Log( "文件不存在！" );
            return;
        }

        //若资源已被加载过，则不重复加载
        if ( info.m_loaded == true )
        {
            Debug.Log( "已载入资源，不能重复载入！" );
            return;
        }

        if ( info.m_list.Count != m_list.Count )
        {
            Debug.Log( "链表数目不统一！无法赋值！,改写为当前数据！" );
            Save();
            return;
        }

        //对关卡信息的每一项，将json数据赋值给它
        for ( int i = 0; i < m_list.Count; i++ )
        {
            m_list[i].CopyFrom( info.m_list[i] );
        }

        info.m_loaded = true;   //记录已加载过
            
        SaveData( info );   //重新保存json
    }

    private void SaveData( LevelInfoListAccess list )
    {
        Debug.Log( "保存数据" );

        string path = Application.streamingAssetsPath + "/levelInfo.json";
        string jsonString = JsonUtility.ToJson( list ); //将关卡信息转为json

        File.WriteAllText( path, jsonString );  //存入文件
    }

    private void LoadData(out LevelInfoListAccess info)
    {
        Debug.Log( "读取数据" );

        string path = Application.streamingAssetsPath + "/levelInfo.json";

        //如果文件不存在，则info置为空，直接返回
        if( !File.Exists(path) )
        {
            info = null;
            return;
        }

        string jsonString = File.ReadAllText( path );   //从路径读取json

        info = JsonUtility.FromJson<LevelInfoListAccess>( jsonString );     //实例化
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
