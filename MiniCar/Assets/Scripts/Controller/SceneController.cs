using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public UserData userData;   //用户数据

    private void Start()
    {
        //userData = Resources.Load<UserData>( "UserDatum/UserData" );
    }

    //载入场景
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene( sceneName );
    }

    //获取当前场景名
    public string CurrentSceneName
    {
        get { return SceneManager.GetActiveScene().name; }
    }

    //开始新游戏
    public void StartNewGame()
    {
        userData.Reset();
    }

    //游戏退出方法
    public void QuitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();

#endif
    }
}
