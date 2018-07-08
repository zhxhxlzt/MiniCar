using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {

    [SerializeField] private AudioMixerSnapshot mute, unMute, finish;   //snapshot,用于声音控制

    private ChallengeController m_challengeController;  //闯关控制器
    
    private void Start()
    {
        unMute.TransitionTo( 0f );

        m_challengeController = ChallengeController.Instance;

        if( m_challengeController != null )
        {
            //订阅闯关控制器事件
            m_challengeController.OnChallengeSucceed += FinishSnapShot;
            m_challengeController.OnChallengeFailed += FinishSnapShot;
            m_challengeController.OnChallengePaused += Mute;
            m_challengeController.OnChallengeStart += UnMute;
        }
    }

    //静音
    public void Mute()
    {
        mute.TransitionTo( 0 );
    }

    //关闭静音
    public void UnMute()
    {
        unMute.TransitionTo( 0 );
    }

    //完成时的音效
    public void FinishSnapShot()
    {
        finish.TransitionTo( 1f );
    }
}
