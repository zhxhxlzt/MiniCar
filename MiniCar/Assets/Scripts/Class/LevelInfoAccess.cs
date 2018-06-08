using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelInfoAccess {
    public bool m_passed;     //当前关卡是否已通过
    public bool m_locked;     //当前关卡是否已解锁
    public bool m_isBegin;   //当前关卡是否为开始关卡
    public bool m_isEnd;    //当前关卡是否为最终关卡
    public float m_timeUsage; //当前关卡通关时间
}

public class LevelInfoListAccess
{
    public bool m_loaded;   //是否已加载过
    public List<LevelInfoAccess> m_list;

    //构造函数，将关卡信息表中的每一项赋值给此表
    public LevelInfoListAccess( List<LevelInfo> infoList )
    {
        m_loaded = false;
        m_list = new List<LevelInfoAccess>();

        foreach ( var item in infoList )
        {
            LevelInfoAccess info = new LevelInfoAccess();
            item.CopyTo( ref info );

            m_list.Add( info );
        }
    }
}
