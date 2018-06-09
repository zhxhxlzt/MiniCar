using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {

    [SerializeField] private AudioMixerSnapshot mute, unMute, finish;
    private bool m_muted = false;

    private void Start()
    {
        m_muted = false;
        unMute.TransitionTo( 0f );
    }

    public void MuteSwitch()
    {
        if( m_muted = !m_muted)
        {
            mute.TransitionTo( 0 );
        }
        else
        {
            unMute.TransitionTo( 0 );
        }
    }

    public void FinishSnapShot()
    {
        finish.TransitionTo( 1f );
    }
}
