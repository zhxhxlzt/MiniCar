using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 根据关卡状态判断能否进入相应场景
/// </summary>
public class LevelManager : MonoBehaviour {

    [Header("=== 数据 ===")]
    [SerializeField] private LevelSlot[] m_allLevelSlots;       //所有关卡节点
    [SerializeField] private LevelInfoList m_levelInfoList;     //所有关卡信息

    [Header("=== 外部控制器 ===")]
    [SerializeField] private SceneController m_sceneController; //场景控制器
    
    //初始化
    private void Start()
    {
        m_sceneController = FindObjectOfType<SceneController>();
        FindAllLevelSlots();
        m_levelInfoList.Load();     //从磁盘加载关卡信息
    }

    //如果关卡已解锁，则载入当前关卡
    public void LoadLevel(string levelName)
    {
        FindAllLevelSlots();
        LevelSlot curLevel = FindLevelSlot( levelName );
        if ( !curLevel.CurLevelInfo.Locked )
        {
            m_sceneController.LoadScene( levelName );
        }
    }
    
    public LevelSlot[] AllSlot { get { return m_allLevelSlots; } }

    //根据关卡名查找对应关卡节点
    public LevelSlot FindLevelSlot(string levelName)
    {
        foreach ( var item in m_allLevelSlots )
        {
            if( item.name == levelName )
            {
                return item;
            }
        }

        return null;
    }

    //重置所有关卡
    public void ResetAllLevels()
    {
        foreach ( var item in m_allLevelSlots )
        {
            item.CurLevelInfo.Reset();
        }
    }

    //查找所有关卡
    private void FindAllLevelSlots()
    {
        m_allLevelSlots = FindObjectsOfType<LevelSlot>();
    }

    private void OnApplicationQuit()
    {
        m_levelInfoList.Save();
    }
}
