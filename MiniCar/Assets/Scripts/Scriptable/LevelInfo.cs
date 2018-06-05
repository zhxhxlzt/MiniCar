using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelInfo", menuName = "Level/Info")]
public class LevelInfo : ScriptableObject {

    [SerializeField] private bool m_passed;     //当前关卡是否已通过
    [SerializeField] private bool m_locked;     //当前关卡是否已解锁
    [SerializeField] private bool m_isBegin;   //当前关卡是否为开始关卡
    [SerializeField] private bool m_isEnd;    //当前头卡是否为最终关卡

    [SerializeField] private List<LevelInfo> m_next;    //子关卡

    public bool Passed { get { return m_passed; } set { m_passed = value; } }
    public bool Locked { get { return m_locked; } set { m_locked = value; } }
    public bool IsBegin { get { return m_isBegin; } set { m_isBegin = value; } }
    public bool IsEnd { get { return m_isEnd; } set { m_isEnd = value; } }

    public void SetNext( List<LevelInfo> next )
    {
        if( m_next.Equals(next) )
        {
            return;
        }

        //m_next = new List<LevelInfo>( next );
        m_next = next;
    }
    
    //解锁子关卡
    public void UnlockNext()
    {
        if( m_next.Count != 0 )
        {
            foreach ( var item in m_next )
            {
                item.m_locked = false;
            }
        }
    }

    //重置关卡
    public void Reset()
    {
        m_passed = false;
        m_locked = m_isBegin ? false : true;
    }
}
