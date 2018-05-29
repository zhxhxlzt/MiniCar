using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour {

	
	
	// Update is called once per frame
	void Update () {
        float x = Input.GetAxis( "Horizontal" );
        float y = Input.GetAxis( "Vertical" );

        transform.position += new Vector3( x, 0f, z );
	}
}
