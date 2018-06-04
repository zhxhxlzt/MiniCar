using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

	public void SwitchScene( string sceneName )
    {
        Debug.Log( "Switch to scene :" + sceneName );
        SceneManager.LoadScene( sceneName );
    }

    
}
