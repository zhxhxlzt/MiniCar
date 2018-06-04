using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene( sceneName );
    }



    ////公有场景切换方法
    //public void LoadScene(string sceneName)
    //{
    //    StartCoroutine( SwitchScene( sceneName ) );
    //}

    ////场景更换方法
    //private IEnumerator SwitchScene(string sceneName)
    //{
    //    yield return SceneManager.UnloadSceneAsync( SceneManager.GetActiveScene().buildIndex ); //载出当前场景
    //    yield return StartCoroutine( LoadSceneAndSetActive( sceneName ) ); //载入新场景并激活
    //}

    ////场景载入方法
    //private IEnumerator LoadSceneAndSetActive(string sceneName)
    //{
    //    yield return SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Single );
    //    Scene newlyLoadedScene = SceneManager.GetSceneAt( SceneManager.sceneCount - 1 );
    //    SceneManager.SetActiveScene( newlyLoadedScene );
    //}

}
