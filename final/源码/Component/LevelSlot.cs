using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelSlot : MonoBehaviour {

    [SerializeField] private Sprite m_unlocked;                 //解锁sprite
    [SerializeField] private Sprite m_locked;                   //锁定sprite
    [SerializeField] private Sprite m_passed;                   //通过sprite
    [SerializeField] private Image  m_curImage;

    [SerializeField] private LevelInfo m_levelInfo;             //当前关卡数据
    [SerializeField] private Text m_timeUsage;                  //闯关时间显示Text
    [SerializeField] private List<LevelSlot> m_NextLevels;      //子关卡节点
    
    //初始化
    private void Awake()
    {
        m_curImage = GetComponent<Image>();     //获取当前节点Imag
        m_timeUsage = transform.Find( "TimeUsage" ).GetComponent<Text>();   //获取text组件
        
        GetComponent<Button>().onClick.AddListener( LoadLevel );
    }

    private void Start()
    {
        Refresh();
    }
    

    //载入相应关卡
    public void LoadLevel()
    {
        if(m_levelInfo.LevelState >= LevelStateEnum.unlocked)
        {
            Director.Instance.LoadScene( m_levelInfo.LevelName );
        }
        Refresh();
    }
    
    //解锁此关卡
    public void Unlock()
    {
        m_levelInfo.UnLock();
        Refresh();
    }

    //重置关卡
    public void ResetLevelInfo()
    {
        m_levelInfo.Reset();    //重置关卡数据
        Refresh();
    }

    //从json文件载入
    public void LoadLevelInfo()
    {
        m_levelInfo.LoadLevelData();
        Refresh();
    }
    //保存关卡信息来磁盘
    public void SaveLevelInfo()
    {
        m_levelInfo.SaveLevelData();
    }

    public void Refresh()
    {
        SetButtonImage();
        ShowTimeUsage();
        CheckUnlock();
    }

    //设置button图片
    private void SetButtonImage()
    {
        switch ( m_levelInfo.LevelState )
        {
            case LevelStateEnum.locked:
                m_curImage.sprite = m_locked;
                break;
            case LevelStateEnum.unlocked:
                m_curImage.sprite = m_unlocked;
                break;
            case LevelStateEnum.passed:
                m_curImage.sprite = m_passed;
                break;
        }
    }

    //设置闯关用时
    private void ShowTimeUsage()
    {
        //如果通过关卡，显示最佳通关时间记录，否则关闭时间框
        if ( m_levelInfo.LevelState == LevelStateEnum.passed )
        {
            m_timeUsage.enabled = true;
            m_timeUsage.text = "用时：" + Math.Round(m_levelInfo.TimeUsage,2) + "秒";
        } else
        {
            m_timeUsage.enabled = false;
            m_timeUsage.text = "";
        }
    }

    //检查能否解锁子关卡
    private void CheckUnlock()
    {
        if( m_levelInfo.LevelState == LevelStateEnum.passed )
        {
            foreach ( var item in m_NextLevels )
            {
                item.Unlock();
            }
        }
    }
    
}
