using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlot : MonoBehaviour {

    [SerializeField] private Sprite m_unlocked;                 //解锁sprite
    [SerializeField] private Sprite m_locked;                   //锁定sprite
    [SerializeField] private Sprite m_passed;                   //通过sprite
    private Image image;

    [SerializeField] private LevelInfo m_levelInfo;             //当前关卡数据
    [SerializeField] private Text m_timeUsage;                  //闯关时间显示Text
    [SerializeField] private List<LevelSlot> m_levelSlotList;   //子关卡节点

    //初始化
    private void Start()
    {
        image = GetComponent<Image>();

        //获取所有子Text组件
        Text[] allText = GetComponentsInChildren<Text>();  

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
    }

    //获取当前关卡信息
    public LevelInfo CurLevelInfo { get { return m_levelInfo; } }

    //设置button图片
    private void SetButtonImage()
    {
        if(m_levelInfo.Locked)
        {
            image.sprite = m_locked;
            return;
        }

        if(m_levelInfo.Passed)
        {
            image.sprite = m_passed;
        }
        else
        {
            image.sprite = m_unlocked;
        }
    }

    //设置闯关用时
    private void SetTimeUsage()
    {
        //如果通过关卡，显示最佳通关时间记录，否则关闭时间框
        if ( CurLevelInfo.Passed) 
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

    //界面刷新
    private void OnGUI()
    {
        CheckUnlock();
        SetButtonImage();
        SetTimeUsage();
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

        //生成levelInfo链表
        foreach ( var item in m_levelSlotList )
        {
            temp.Add( item.CurLevelInfo );
        }

        CurLevelInfo.SetNext( temp );   //设置子关系链表表
    }
}
