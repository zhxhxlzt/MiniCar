using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [Header( "=== 控制对象 ===" )]
    [SerializeField] private GameObject m_horizontal;
    [SerializeField] private GameObject m_vertical;
    [SerializeField] private GameObject m_camera;

    [Header("=== 跟随目标 ===")]
    [SerializeField]private GameObject m_target;

    [Header( "=== 相机参数 ===" )]
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool m_isFollowing = true;
    [SerializeField] private bool m_isLookAt = true;

    private void Awake()
    {
        m_horizontal = GameObject.Find( "CamHor" );
        m_vertical = GameObject.Find( "CamVer" );
        m_camera = GameObject.Find( "Main Camera" );
        m_target = GameObject.FindGameObjectWithTag( "Player" );
    }

    private void Start()
    {
        AdjustOffset();
    }
    private void FixedUpdate()
    {
        FollowTarget();
        LookAtTarget();
        AdjustOffset();
    }


    private void FollowTarget()
    {
        if ( !m_isFollowing ) return;

        transform.position = m_target.transform.position;
        transform.forward = m_target.transform.forward;
    }

    private void LookAtTarget()
    {
        if ( !m_isLookAt ) return;

        m_camera.transform.LookAt(m_target.transform);
    }

    private void AdjustOffset()
    {
        
        m_horizontal.transform.position = transform.position + transform.forward * offset.z;
        m_vertical.transform.position = m_horizontal.transform.position + m_horizontal.transform.up * offset.y;
    }
}
