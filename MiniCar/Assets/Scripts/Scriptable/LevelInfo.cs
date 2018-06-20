using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum LevelStateEnum { locked, unlocked, passed }

[CreateAssetMenu(fileName = "LevelInfo", menuName = "Level/Info")]
[Serializable]
public class LevelInfo : ScriptableObject {

    [SerializeField] private int      m_levelIndex;                  //关卡序号
    [SerializeField] private string   m_levelName;                   //关卡名
    [SerializeField] private float    m_passTimeLimit;               //通关时间限制
    [SerializeField] private float    m_passTimeUsage;               //通关用时
    [SerializeField] private int      m_passTurnLimit;               //通关圈数限制
    [SerializeField] private LevelStateEnum m_levelState;              //通关状态
    
    //从磁盘载入
    public void LoadLevelData()
    {
        string jss = File.ReadAllText(Application.streamingAssetsPath + "/" + LevelName +  ".json");

        if( jss == null ) { return; }

        JsonUtility.FromJsonOverwrite( jss, this );
    }

    //解锁
    public void UnLock()
    {
        if( LevelState == LevelStateEnum.locked )
        {
            m_levelState = LevelStateEnum.unlocked;
        }
    }

    public void SetPass()
    {
        if( LevelState == LevelStateEnum.unlocked )
        {
            m_levelState = LevelStateEnum.passed;
        }
    }

    //公有数据获取方法
    public int LevelIndex { get { return m_levelIndex; } }
    public string LevelName { get { return m_levelName; } }
    public float PassTimeLimit { get { return m_passTimeLimit; } }
    public float TimeUsage
    {
        get
        {
            return m_passTimeUsage;
        }
        set
        {
            if( m_passTimeUsage == 0 ) { m_passTimeUsage = value; } else
            {
                if( m_passTimeUsage > value ) { m_passTimeUsage = value; }
            }
        }
    }
    public int PassTurnLimit { get { return m_passTurnLimit; } }
    public LevelStateEnum LevelState { get { return m_levelState; } }

    //保存到磁盘
    public void SaveLevelData()
    {
        string jss = JsonUtility.ToJson(this);
        File.WriteAllText( Application.streamingAssetsPath + "/" + LevelName + ".json", jss );
    }

    //重置，如果是开始点，则解锁
    public void Reset()
    {
        if( LevelIndex == 1 )
        {
            m_levelState = LevelStateEnum.unlocked;
        }
        else
        {
            m_levelState = LevelStateEnum.locked;
        }

        m_passTimeUsage = 0f;
    }
}

