using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSlot : MonoBehaviour {

    [SerializeField] private LevelInfo m_levelInfo;      //当前关卡
    [SerializeField] private RawImage m_lockImage;       //锁定图片
    [SerializeField] private List<LevelSlot> m_LevelSlotList;   //子关卡
    
    //获取当前关卡信息
    public LevelInfo CurLevelInfo { get { return m_levelInfo; } }

    //设置锁定图片透明度
    public void SetLockImageAlpha(float a)
    {
        var tempColor = m_lockImage.color;

        tempColor.a = a;
        m_lockImage.color = tempColor;
    }

    private void OnGUI()
    {
        CheckUnlock();
    }

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

        CurLevelInfo.SetNext( temp );
    }

}
