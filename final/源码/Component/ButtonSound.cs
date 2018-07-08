using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour {

    public AudioClip clickClip;     //音效clip
    public AudioSource source;      //音源

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener( PlayClickSound );   //添加点击时音效
    }

    //播放点击音效
    public void PlayClickSound()
    {
        source.clip = clickClip;
        source.Play();
    }
}
