using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    [Header( "=== 外部控制器 ===" )]
    [SerializeField] private InputHandler m_inputHandler;    //输入控制器

    [Header( "=== 车轮模型 ===" )]
    [SerializeField] private GameObject[] m_wheelModels;

    [Header( "=== 车轮碰撞器 ===" )]
    [SerializeField] private WheelCollider[] m_wheelColliders;
    [SerializeField] private Rigidbody m_carRigidbody;

    [Header( "=== 赛车属性 ===" )]
    [SerializeField] private float m_maxSpeed;               //最高速度 m/s
    [SerializeField] private float m_maxMotorTorque;        //最高动力扭矩
    [SerializeField] private float m_maxSteer;              //最大转向角
    [SerializeField] private float m_maxBrakeTorque;        //最大制动所知

    [SerializeField] private bool m_isDriving = true;       //是否处于驾驶状态
    [SerializeField] private bool m_isOnGround = true;      //是否着地
    [SerializeField] private Vector3 m_centreOfMass = Vector3.zero;   //重心
    
    public Vector3 Velocity { get { return m_carRigidbody.velocity; } }

    private void Awake()
    {
        GetComponent<Rigidbody>().centerOfMass = m_centreOfMass;    //设置赛车重心位置
    }

    private void Start()
    {
        m_inputHandler = GameObject.FindGameObjectWithTag( "GameController" ).GetComponent<InputHandler>();
        m_carRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        DriveControl();
        SyncWheels();
    }

    private void DriveControl()
    {
        Debug.Log( Velocity.magnitude );
        //Debug.Log( m_wheelColliders[0].rpm );
        //非驾驶状态不接受移动输入命令控制
        if ( !m_isDriving )
        {
            return;
        }

        float motorTorque = Velocity.magnitude <= m_maxSpeed ? m_maxMotorTorque / 4 : 0f;   //各车轮动力扭矩,超速则不提供动力
        float brakeTorque = m_maxBrakeTorque / 2;   //各车轮制动扭矩

        for( int i = 0; i < m_wheelColliders.Length; i++ )
        {
            m_wheelColliders[i].motorTorque = motorTorque * m_inputHandler.Accel; //动力
        }

        for ( int i = 2; i < 4; i++ )
        {
            m_wheelColliders[i].brakeTorque = brakeTorque * m_inputHandler.Brake; //刹车
        }

        for( int i = 0; i < 2; i++ )
        {
            m_wheelColliders[i].steerAngle = m_maxSteer * m_inputHandler.Steer;   //转向
        }

        if(m_inputHandler.jump)
        {
            m_carRigidbody.velocity += Vector3.up * 10;
            m_inputHandler.jump = false;
        }

        if(m_inputHandler.forwardDash)
        {
            m_carRigidbody.velocity += transform.forward * 5;
            m_inputHandler.forwardDash = false;
        }

        if(m_inputHandler.leftDash)
        {
            m_carRigidbody.velocity += transform.right * -5;
            m_inputHandler.leftDash = false;
        }

        if(m_inputHandler.rightDash)
        {
            m_carRigidbody.velocity += transform.right * 5;
            m_inputHandler.rightDash = false;
        }
    }

    //同步车轮模型与碰撞器
    private void SyncWheels()
    {
        for( int i = 0; i < m_wheelColliders.Length; i++ )
        {
            Vector3 pos;
            Quaternion quat;

            m_wheelColliders[i].GetWorldPose( out pos, out quat );        
            m_wheelModels[i].transform.SetPositionAndRotation( pos, quat );
        }
    }
}
