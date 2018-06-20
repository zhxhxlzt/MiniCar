using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStorageTest : MonoBehaviour {

    public LevelInfoList LevelInfoList;

    private void OnGUI()
    {
        if( GUILayout.Button("Load") )
        {
            LevelInfoList.LoadLevelData();
        }

        if( GUILayout.Button("Save"))
        {
            LevelInfoList.SaveLevelData();
        }
    }
}
