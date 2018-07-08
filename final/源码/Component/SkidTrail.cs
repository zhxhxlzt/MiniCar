using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidTrail : MonoBehaviour {

    [SerializeField] private float m_PersistTime;   //车轮痕迹持续时间
    
    private IEnumerator Start()
    {
        while ( true )
        {
            yield return null;

            if ( transform.parent.parent == null )
            {
                Destroy( gameObject, m_PersistTime );
            }
        }
    }
}
