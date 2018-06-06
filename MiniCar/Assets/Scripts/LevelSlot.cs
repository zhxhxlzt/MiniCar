using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlot : MonoBehaviour {

    [SerializeField] private LevelInfo m_levelInfo;      //当前关卡
    [SerializeField] private Text m_timeUsage;
    [SerializeField] private RawImage m_lockImage;       //锁定图片
    [SerializeField] private RawImage m_finishImage;     //完成图片
    [SerializeField] private List<LevelSlot> m_LevelSlotList;   //子关卡

    private void Start()
    {
        var allText = GetComponentsInChildren<Text>();  //获取所有子Text组件

        //查找其中名为TimeUsage的Text组件
        foreach ( var item in allText )
        {
            if( m_timeUsage != null )
            {
                break;
            }

            if( item.name == "TimeUsage")
            {
                m_timeUsage = item;
            }
        }

        var allRawImage = GetComponentsInChildren<RawImage>();  //获取所有子RawImage组件

        //查找其中名为LockImag，FinishImg的组件
        foreach ( var item in allRawImage )
        {
            if( m_lockImage != null && m_finishImage != null )
            {
                break;
            }

            if( item.name == "LockImg" )
            {
                m_lockImage = item;
                continue;
            }

            if( item.name == "FinishImg" )
            {
                m_finishImage = item;
                continue;
            }
        }
    }

    //获取当前关卡信息
    public LevelInfo CurLevelInfo { get { return m_levelInfo; } }

    //设置锁定图片透明度
    public void SetLockImageAlpha(float a)
    {
        SetImageAlpha( m_lockImage, a );
    }

    //设置完成图片透明度
    public void SetFinishImageAlpha(float a)
    {
        SetImageAlpha( m_finishImage, a );
    }

    public void SetTimeUsage()
    {
        if(CurLevelInfo.Passed)
        {
            m_timeUsage.enabled = true;
            m_timeUsage.text = "用时：" + m_levelInfo.TimeUsage.ToString() + "秒";
        }
        else
        {
            m_timeUsage.enabled = false;
            m_timeUsage.text = "";
        }
    }

    //设置图片透明度
    private void SetImageAlpha(RawImage img, float a)
    {
        var tempColor = img.color;

        tempColor.a = a;
        img.color = tempColor;
    }

    private void OnGUI()
    {
        CheckUnlock();
    }

    //存储子关卡关系
    private void OnEnable()
    {
        CopyInfoListToLevelInfo();
    }

    //检查能否解锁子关卡
    private void CheckUnlock()
    {
        if ( m_levelInfo.Passed )
        {
            m_levelInfo.UnlockNext();
        }
    }

    //将levelSlot的子关卡拷贝到用户数据
    private void CopyInfoListToLevelInfo()
    {
        List<LevelInfo> temp = new List<LevelInfo>();

        foreach ( var item in m_LevelSlotList )
        {
            temp.Add( item.CurLevelInfo );
        }

        CurLevelInfo.SetNext( temp );   //设置子关系表
    }
}
