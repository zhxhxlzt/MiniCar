using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [Header( "=== 控制对象 ===" )]
    [SerializeField] private Transform m_camera;   //相机

    [Header("=== 目标 ===")]
    [SerializeField] private Transform m_follow;    //跟随目标
    [SerializeField] private Rigidbody m_carRig;   //朝向速度方向
    [SerializeField] private Transform m_lookAt;   //看向目标

    [Header( "=== 相机参数 ===" )]
    [SerializeField] private Vector3 offset;        //相机与跟随目标之间的偏移量
    [SerializeField] [Range(0,1)] private float rotSpeed;
    [SerializeField] private float rotAngleLimit;
    [SerializeField] private bool m_isFollowing = true;
    [SerializeField] private bool m_isLookAt = true;

    //初始化，获取相机gameobject
    private void Start()
    {
        m_camera = GetComponentInChildren<Camera>().transform;
        m_carRig = GameObject.FindGameObjectWithTag( "Player" ).GetComponent<Rigidbody>();
        m_follow = m_carRig.transform.Find( "CameraHelp" ).Find( "FollowPoint" );
        m_lookAt = m_carRig.transform.Find( "CameraHelp" ).Find( "LookAtPoint" );
    }

    //相机跟随所处循环与赛车一致
    private void FixedUpdate()
    {
        AdjustCameraOffset();
        FollowTarget();
        LookAtTarget();
        Rotate();
    }

    //跟随
    private void FollowTarget()
    {
        if ( !m_isFollowing ) return;

        transform.position = m_follow.position;
    }

    //看向目标
    private void LookAtTarget()
    {
        if ( !m_isLookAt ) return;

        m_camera.LookAt(m_lookAt.transform);
    }

    //调整偏移量
    private void AdjustCameraOffset()
    {
        m_camera.localPosition = offset;
    }

    //旋转
    private void Rotate()
    {
        transform.forward = Vector3.Slerp( transform.forward, FigureTargetForward(), rotSpeed );
    }
    
    //计算目标指向
    private Vector3 FigureTargetForward()
    {
        Vector3 horVelocity = m_carRig.velocity; //赛车速度
        Vector3 horCarForward = m_follow.forward;//赛车朝向

        if (horVelocity.magnitude == 0 || Vector3.SignedAngle(m_carRig.velocity, m_follow.forward, Vector3.up) < 5f)
        {
            return horCarForward.normalized;
        }
        
        if( Vector3.Angle( horCarForward, horVelocity) <= rotAngleLimit )
        {
            return horVelocity.normalized;
        }

        return Vector3.RotateTowards( horCarForward, horVelocity, rotAngleLimit, 1f ).normalized;
    }
}
