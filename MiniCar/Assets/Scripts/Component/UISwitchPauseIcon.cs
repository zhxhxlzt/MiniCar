using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISwitchPauseIcon : MonoBehaviour {

    public Sprite pause;        //暂停图片
    public Sprite resume;       //恢复图片
    public Image source;

    private void Start()
    {
        source = GetComponent<Image>();

        //调用闯关控制器事件
        ChallengeController.Instance.OnChallengePaused += ResumeImage;
        ChallengeController.Instance.OnChallengeStart += PauseImage;
    }
    
    //暂停开关
    public void PauseSwitch()
    {
        ChallengeController.Instance.PauseSwitch();
    }

    //继续时图片
    public void PauseImage()
    {
        source.sprite = pause;
    }

    //暂停时图片
    public void ResumeImage()
    {
        source.sprite = resume;
    }
}
