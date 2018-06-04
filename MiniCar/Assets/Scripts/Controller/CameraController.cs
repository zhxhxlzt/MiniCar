using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [Header( "=== 控制对象 ===" )]
    [SerializeField] private GameObject m_camera;   //相机

    [Header("=== 目标 ===")]
    [SerializeField]private GameObject m_follow;    //跟随目标
    [SerializeField] private GameObject m_lookAt;   //看向目标

    [Header( "=== 相机参数 ===" )]
    [SerializeField] private Vector3 offset;        //相机与跟随目标之间的偏移量
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

    //跟随
    private void FollowTarget()
    {
        if ( !m_isFollowing ) return;

        transform.position = m_follow.transform.position;
        
    }

    //看向目标
    private void LookAtTarget()
    {
        if ( !m_isLookAt ) return;

        m_camera.transform.LookAt(m_lookAt.transform);
    }

    //调整偏移量
    private void AdjustOffset()
    {
        if( lastOffset != offset )
        {
            m_camera.transform.position = transform.position + offset;
        }

        lastOffset = offset;
    }

    //旋转
    private void Rotate()
    {
        transform.forward = m_follow.transform.forward;
    }
}
