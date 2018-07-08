using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField] private Button ExitGame;

    private void Start()
    {
        AddListener();
    }

    //添加事件调用
    private void AddListener()
    {
        //退出游戏
        ExitGame.onClick.AddListener( Director.Instance.QuitGame );
    }
}
