using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//界面音效控制
public class UIAudioController : MonoBehaviour {
    
    [SerializeField] private AudioSource m_bgm;
    [SerializeField] private AudioSource m_btClick;

    //播放点击按钮音效
    public void PlayButtonClickSound()
    {
        if( !m_btClick.isPlaying)
        {
            m_btClick.Play();
        }
    }
}
