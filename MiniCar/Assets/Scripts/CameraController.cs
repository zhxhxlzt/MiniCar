using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [Header( "=== 控制对象 ===" )]
    [SerializeField] private GameObject m_camera;

    [Header("=== 目标 ===")]
    [SerializeField]private GameObject m_follow;
    [SerializeField] private GameObject m_lookAt;

    [Header( "=== 相机参数 ===" )]
    [SerializeField] private Vector3 offset;
    private Vector3 lastOffset;
    [SerializeField] private bool m_isFollowing = true;
    [SerializeField] private bool m_isLookAt = true;

    private void Awake()
    {
        m_camera = GetComponentInChildren<Camera>().gameObject;
    }

    private void FixedUpdate()
    {
        AdjustOffset();
        FollowTarget();
        LookAtTarget();
        Rotate();
    }

    private void FollowTarget()
    {
        if ( !m_isFollowing ) return;

        transform.position = m_follow.transform.position;
        
    }

    private void LookAtTarget()
    {
        if ( !m_isLookAt ) return;

        m_camera.transform.LookAt(m_lookAt.transform);
    }

    private void AdjustOffset()
    {
        if( lastOffset != offset )
        {
            m_camera.transform.position = transform.position + offset;
        }

        lastOffset = offset;
    }

    private void Rotate()
    {
        transform.forward = m_follow.transform.forward;
    }
}
