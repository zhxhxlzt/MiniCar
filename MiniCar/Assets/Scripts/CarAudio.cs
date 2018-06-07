using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 赛车音效控制
/// </summary>
public class CarAudio : MonoBehaviour {

    [SerializeField] private AudioSource m_move;            //停止音效
    [SerializeField] private AudioSource m_skid;            //刹车音效

    [SerializeField] private CarController m_carController; //赛车控制器

    private void FixedUpdate()
    {
        PlayEngineSound();  //播放引擎音效
        PlaySkidSound();    //播放打滑音效
    }
    
    //播放引擎音效
    private void PlayEngineSound()
    {
        var ratio = m_carController.Velocity.magnitude / m_carController.MaxSpeed;

        m_move.pitch = Mathf.Lerp( 1f, 2.5f, ratio );   //获取要播放声音的pitch，赛车速度越快,pitch越高

        if (m_move.isPlaying == false)
        {
            m_move.Play();
        }
    }

    //播放打滑音效
    private void PlaySkidSound()
    {
        bool rightDirecton = Vector3.Angle( m_carController.Velocity, m_carController.transform.forward ) < 75f;

        //如果赛车没有打滑，停止播放音效
        if ( !m_carController.Sliped )
        {
            m_skid.Stop();
            return;
        }

        //如果赛车没有播放音效，播放音效
        if( !m_skid.isPlaying && rightDirecton )
        {
            m_skid.Play();
        }
    }
}
