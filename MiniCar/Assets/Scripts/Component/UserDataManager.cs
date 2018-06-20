using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoBehaviour {

    public LevelInfoList m_levelInfoList;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        m_levelInfoList.LoadLevelData();
        Director.Instance.LoadScene( "MainScene" );
    }

    private void OnDestroy()
    {
        m_levelInfoList.SaveLevelData();
    }
}
