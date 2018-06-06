using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    [Header("=== 外部控制器 ===")]
    [SerializeField] private LevelManager m_levelManager;
    [SerializeField] private CarController m_carController;
    [SerializeField] private ChallengeController m_challengeController;
    
    private void OnGUI()
    {
        //检查各关卡节点锁定图片状态
        CheckLevelSlotImg();

        //检查各关卡节点用时
        CheckLevelSlotTimeUsage();
    }

    private void CheckLevelSlotImg()
    {
        foreach ( var item in m_levelManager.AllSlot )
        {
            if ( item.CurLevelInfo.Locked )     //若关卡被锁定，则只显示锁定图片
            {
                item.SetLockImageAlpha( 1 );
                item.SetFinishImageAlpha( 0 );
                continue;
            }

            if ( item.CurLevelInfo.Passed )     //若关卡已通过，则只显示通过图片
            {
                item.SetLockImageAlpha( 0 );
                item.SetFinishImageAlpha( 1 );
                continue;
            }

            //若关卡未通过，则不显示图片
            item.SetLockImageAlpha( 0 );
            item.SetFinishImageAlpha( 0 );
        }
    }

    private void CheckLevelSlotTimeUsage()
    {
        foreach ( var item in m_levelManager.AllSlot )
        {
            item.SetTimeUsage();
        }
    }
}
