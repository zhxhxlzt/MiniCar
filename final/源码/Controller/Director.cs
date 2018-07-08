using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "Director")]
public class Director : ScriptableObject {

    public static Director Instance
    {
        get
        {
            if ( _Instance == null)
            {
                _Instance = new Director();
            }
            return _Instance;
        }
    }

    private static Director _Instance;

    private Director() { }
    
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
