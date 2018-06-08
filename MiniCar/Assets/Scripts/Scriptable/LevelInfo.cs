using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelInfo", menuName = "Level/Info")]
[Serializable]
public class LevelInfo : ScriptableObject {

    [SerializeField] private bool m_passed;     //当前关卡是否已通过
    [SerializeField] private bool m_locked;     //当前关卡是否已解锁
    [SerializeField] private bool m_isBegin;   //当前关卡是否为开始关卡
    [SerializeField] private bool m_isEnd;    //当前关卡是否为最终关卡
    [SerializeField] private float m_timeUsage; //当前关卡通关时间

    [SerializeField] private List<LevelInfo> m_next;    //子关卡

    public bool Passed { get { return m_passed; } set { m_passed = value; } }
    public bool Locked { get { return m_locked; } set { m_locked = value; } }
    public bool IsBegin { get { return m_isBegin; } set { m_isBegin = value; } }
    public bool IsEnd { get { return m_isEnd; } set { m_isEnd = value; } }
    public float TimeUsage { get { return m_timeUsage; } set { m_timeUsage = value; } }

    //设置子关卡
    public void SetNext( List<LevelInfo> next )
    {
        if ( m_next.Equals( next ) )
        {
            return;
        }
        
        m_next = next;
    }
    
    //设置闯关用时
    public void SetTimeUsage(float time)
    {
        if( Passed )
        {
            if( m_timeUsage == 0)
            {
                m_timeUsage = time;
            }
            else
            {
                m_timeUsage = Mathf.Min( m_timeUsage, time );
            }
        }
    }

    //解锁子关卡
    public void UnlockNext()
    {
        if ( m_next.Count != 0 )
        {
            foreach ( var item in m_next )
            {
                item.m_locked = false;
            }
        }
    }

    //获取数据
    public void CopyFrom( LevelInfoAccess data)
    {
        Debug.Log( "读取一次" );
        m_passed = data.m_passed;
        m_locked = data.m_locked;
        m_isBegin = data.m_isBegin;
        m_isEnd = data.m_isEnd;
        m_timeUsage = data.m_timeUsage;
    }

    //设置数据
    public void CopyTo( ref LevelInfoAccess data )
    {
        data.m_passed = m_passed;
        data.m_locked = m_locked;
        data.m_isBegin = m_isBegin;
        data.m_isEnd = m_isEnd;
        data.m_timeUsage = m_timeUsage;
    }

    //重置关卡
    public void Reset()
    {
        m_passed = false;
        m_locked = m_isBegin ? false : true;
        m_timeUsage = 0f;
    }
}
