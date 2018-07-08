using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 根据关卡状态判断能否进入相应场景
/// </summary>
public class LevelManager : MonoBehaviour {

    private LevelSlot[] m_allLevelSlots;    //所有关卡节点

    private void Start()
    {
        m_allLevelSlots = FindObjectsOfType<LevelSlot>();
    }
    
    //重置所有关卡
    public void ResetAllLevels()
    {
        foreach ( var item in m_allLevelSlots )
        {
            item.ResetLevelInfo();
        }
    }
}
