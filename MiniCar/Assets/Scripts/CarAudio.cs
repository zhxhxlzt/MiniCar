using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 赛车音效控制
/// </summary>
public class CarAudio : MonoBehaviour {
    [SerializeField] private AudioSource m_move;            //引擎音效
    [SerializeField] private AudioSource m_skid;            //打滑音效
    [SerializeField] private AudioSource m_collide;         //碰撞音效

    //播放引擎音效
    public void PlayEngineSound( float pitch )
    {
        m_move.pitch = pitch;
    }

    //播放打滑音效
    public void PlaySkidSound( bool isSkid )
    {
        //如果赛车没有播放音效，播放音效
        if( isSkid && !m_skid.isPlaying )
        {
            m_skid.Play();
        }

        if( !isSkid )
        {
            m_skid.Stop();
        }
    }

    public void PlayCollideSound(float factor)
    {
        m_collide.Stop();
        m_collide.volume = factor;
        m_collide.Play();
    }
}
