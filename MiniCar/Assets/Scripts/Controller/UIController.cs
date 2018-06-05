using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    [SerializeField]private LevelManager m_levelManager;

    private void OnGUI()
    {
        //检查各关卡节点锁定图片状态
        CheckLockImg();
    }

    private void CheckLockImg()
    {
        foreach ( var item in m_levelManager.AllSlot )
        {
            if ( item.CurLevelInfo.Locked )
            {
                item.SetLockImageAlpha( 1 );
            }
            else
            {
                item.SetLockImageAlpha( 0 );
            }
        }
    }
}
