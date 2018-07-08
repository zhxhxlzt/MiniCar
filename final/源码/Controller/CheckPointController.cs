using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointController : Object{

    private CheckPoint[] m_allCheckPoints;       //所有检查点

    public CheckPointController()
    {
        FindAllCheckPoints();   //查找所有检查点
        ResetAllCheckPoint();   //重置所有检查点
    }

    //查找所有检查点
    public void FindAllCheckPoints()
    {
        m_allCheckPoints = FindObjectsOfType<CheckPoint>();
        if(m_allCheckPoints.Length == 0)
        {
            Debug.Log( "没有找到检查点！" );
        }

        Debug.Log( "检查点个数为：" + m_allCheckPoints.Length );
    }

    //获取所有检查点
    public CheckPoint[] GetAllCheckPoints()
    {
        return m_allCheckPoints;
    }

    //重置所有检查点的通过状态
    public void ResetAllCheckPoint()
    {
        if( m_allCheckPoints == null)
        {
            return;
        }

        foreach( var item in m_allCheckPoints)
        {
            item.ResetPassState();
        }
    }

    //检查赛车是否通过全部检查点
    public bool AllPassed()
    {
        if ( m_allCheckPoints == null )
        {
            return true;
        }

        //若有一个点为未通过，则未完成一圈
        foreach ( var item in m_allCheckPoints )
        {
            if ( !item.Passed )
            {
                return false;
            }
        }
        return true;
    }
}
