using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


/// <summary>
/// 赛车音效控制
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class CarAudio : MonoBehaviour {
    [SerializeField] private AudioMixerGroup carGroup;
    [SerializeField] private CarController m_carcontroller;
    [SerializeField] private AudioSource m_collsion;         //碰撞音效
    private AudioClip col;

    //播放碰撞音效
    public void PlayCollideSound(float speedFactor)
    {
        m_collsion.Stop();
        m_collsion.volume = speedFactor;
        m_collsion.Play();
    }
    
    public AudioClip lowAccelClip;                  // 音效

    public float minVolume = 0.3f;             // 调整pitch因数
    public float maxVolume = 1f;              // 调整高速音效pitch因数
    public float minPitch = 0.8f;                  // pitch下限
    public float maxPitch = 1.5f;                  // pitch上限

    //用于生成audio source
    private AudioSource m_LowAccel;

    private CarController m_CarController;      //赛车控制器

    private void Start()
    {
        m_carcontroller.OnCollid += PlayCollideSound;
        StartSound();
    }
    //根据音效种类构建audio source
    private void StartSound()
    {
        //获取赛车控制器
        m_CarController = GetComponent<CarController>();

        // 建立音源
        m_LowAccel = SetUpEngineAudioSource( lowAccelClip );
        
    }
    private void Update()
    {
        
        PlayEngineAudio();
    }

    private void PlayEngineAudio()
    {
        float volume = Mathf.Lerp( minVolume, maxVolume, m_CarController.SpeedFactor );
        float pitch = Mathf.Lerp( minPitch, maxPitch, Mathf.Pow( m_CarController.SpeedFactor, 2) );

        m_LowAccel.volume = volume;
        m_LowAccel.pitch = pitch;
    }

    //构建音源
    private AudioSource SetUpEngineAudioSource(AudioClip clip)
    {
        //添加audioSource组件并设置clip，loop
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = minVolume;
        source.pitch = minPitch;
        source.loop = true;

        source.outputAudioMixerGroup = carGroup;

        //选取clip随机时间点开始播放
        source.time = Random.Range( 0f, clip.length );
        source.Play();
        return source;
    }
}
