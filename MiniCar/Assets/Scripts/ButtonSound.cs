using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour {

    public AudioClip clickClip;
    public AudioSource source;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener( PlayClickSound );
    }

    public void PlayClickSound()
    {
        source.clip = clickClip;
        source.Play();
    }
}
