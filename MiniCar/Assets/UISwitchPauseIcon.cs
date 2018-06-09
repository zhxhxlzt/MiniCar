using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISwitchPauseIcon : MonoBehaviour {

    public Sprite pause;        //暂停图片
    public Sprite resume;       //恢复图片
    public Image source;

    private ChallengeController m_c;    //闯关控制器

    private void Start()
    {
        source = GetComponent<Image>();
        m_c = FindObjectOfType<ChallengeController>();
    }

    private void OnGUI()
    {
        ShowButtonImg();
    }

    //响应鼠标点击事件
    public void ShowButtonImg()
    {
        if ( m_c.m_challengeState == ChallengeState.paused )
        {
            source.sprite = resume;
        }
        else
        {
            source.sprite = pause;
        }
    }
}
