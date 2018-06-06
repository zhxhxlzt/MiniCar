using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarController : MonoBehaviour {

    [Header( "=== 外部控制器 ===" )]
    [SerializeField] private InputHandler m_inputHandler;    //输入控制器
    [SerializeField] private UserCarParts m_currentCar;       //当前赛车配置
    [SerializeField] private Rigidbody m_carRigidbody;

    [Header( "=== 车轮模型 ===" )]
    [SerializeField] private GameObject[] WheelModels;

    [Header( "=== 车轮碰撞器 ===" )]
    [SerializeField] private WheelCollider[] wheelColliders;

    [Header( "=== 赛车属性 ===" )]
    [SerializeField] private float m_maxSpeed;              //最高速度 m/s
    [SerializeField] private float m_maxMotorTorque;        //最高动力扭矩
    [SerializeField] private float m_maxSteer;              //最大转向角
    [SerializeField] private float m_maxBrakeTorque;        //最大制动扭矩
    [SerializeField] private float m_wheelFriction;         //车轮摩擦力
    [SerializeField] private float m_airDrag;               //空气阻力
    [SerializeField] private float m_downForce;             //下压力
    [SerializeField] private GameObject m_centorOfMass;     //重心

    private float m_OldRotation;

    [Header("=== 赛车状态 ===")]
    public bool Driveable = true;       //是否处于驾驶状态
    //[SerializeField] private bool m_isOnGround = true;      //是否着地
    [SerializeField] [Range(0,1)]private float m_steerHelper;   //转向帮助
    public Vector3 Velocity { get { return m_carRigidbody.velocity; } }

    //[Header( "=== 测试变量 ===" )]
    //public WheelAdjust wheelColliderTest;


    //private void TestWheelCurve()
    //{
    //    for(int i = 0; i < 2; i++ )
    //    {
    //        wheelColliders[i].forwardFriction = wheelColliderTest.front.forwardFriction;
    //        wheelColliders[i].sidewaysFriction = wheelColliderTest.front.sidewaysFriction;
    //    }

    //    for(int i = 2; i < 4; i++ )
    //    {
    //        wheelColliders[i].forwardFriction = wheelColliderTest.rear.forwardFriction;
    //        wheelColliders[i].sidewaysFriction = wheelColliderTest.rear.sidewaysFriction;
    //    }
    //}

    private void Awake()
    {
        m_carRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        m_inputHandler = GameObject.FindGameObjectWithTag( "GameController" ).GetComponent<InputHandler>(); //获取输入组件
        m_currentCar = Resources.Load<UserData>( "UserDatum/UserData" ).currentCar;
        AdjustCarAttribute();     //初始化赛车属性
    }

    private void FixedUpdate()
    {
        //TestWheelCurve();   //测试friction curve

        AdjustCarAttribute();     
        AdjustForce();
        DriveControl();
        SteerHelper();
        SyncWheels();
    }

    private void AdjustCarAttribute()
    {
        if ( m_currentCar == null )
        {
            m_currentCar = FindObjectOfType<UserCarParts>();
            if ( m_currentCar == null )
            {
                Debug.Log( "userParts Not exists" );
                return;
            }
        }

        m_maxSpeed = (2f * Mathf.PI * wheelColliders[0].radius) * m_currentCar.motor.RPM / 60f;   //最大速度(m/s) = 车轮周长 * rpm / 60
        m_maxMotorTorque = m_currentCar.motor.MotorTorque;    //最大动力扭矩为马达零件扭矩
        m_maxBrakeTorque = m_currentCar.wheel.BrakeTorque;    //最大刹车扭矩为车轮零件刹车扭矩
        m_wheelFriction = m_currentCar.wheel.Friction;        //车轮摩擦力
        m_airDrag = m_currentCar.cover.AirDrag;               //空气阻力
        m_downForce = m_currentCar.cover.DownForce;           //下压力
    }

    private void DriveControl()
    {
        if ( !Driveable )
        {
            Debug.Log( "不能驾驶！" );
            return;
        }

        float motorTorque = m_inputHandler.Accel * (( m_carRigidbody.velocity.magnitude <= m_maxSpeed ) ? 
            m_maxMotorTorque / 4 :
            0);   //各车轮动力扭矩,超过最高速度则为0

        float brakeTorque = m_inputHandler.Brake * m_maxBrakeTorque / 2;   //各车轮制动扭矩

        float steerAngle = m_maxSteer * m_inputHandler.Steer;

        //非驾驶状态不接受移动输入命令控制
        

        for ( int i = 0; i < wheelColliders.Length; i++ )
        {
            wheelColliders[i].motorTorque = motorTorque; //动力
        }

        for( int i = 2; i < 4; i++ )
        {
            wheelColliders[i].brakeTorque = brakeTorque; //刹车
        }
        
        for( int i = 0; i < 2; i++ )
        {
            wheelColliders[i].steerAngle = steerAngle;   //转向
        }
    }

    private void SteerHelper()
    {
        //若车轮不着地，则不改变赛车方向
        for ( int i = 0; i < 4; i++ )
        {
            WheelHit wheelhit;
            wheelColliders[i].GetGroundHit( out wheelhit );
            if ( wheelhit.normal == Vector3.zero )
            {
                return;
            }
        }

        if ( Mathf.Abs( m_OldRotation - transform.eulerAngles.y ) < 10f )
        {
            var turnAdjust = (transform.eulerAngles.y - m_OldRotation) * m_steerHelper;
            Quaternion velRotation = Quaternion.AngleAxis( turnAdjust, Vector3.up );
            m_carRigidbody.velocity = velRotation * m_carRigidbody.velocity;
        }

        m_OldRotation = transform.eulerAngles.y;
    }

    private void AdjustForce()
    {
        m_carRigidbody.centerOfMass = m_centorOfMass.transform.localPosition;

        //调整下压力
        foreach ( var item in wheelColliders )
        {
            Vector3 pos;
            Quaternion quat;

            item.GetWorldPose( out pos, out quat );

            m_carRigidbody.AddForceAtPosition( - 0.25f * m_downForce * transform.up, pos);
        }

                 //下压力方向为赛车下方
        
        //调整空气阻力
        m_carRigidbody.AddForce( -m_airDrag * Velocity.sqrMagnitude * Velocity.normalized );    //空气阻力正比于速度的平方
        
        //调整车轮摩擦力
        for(int i = 0; i < wheelColliders.Length; i++ )
        {
            var forward = wheelColliders[i].forwardFriction;
            forward.stiffness = m_wheelFriction;
            wheelColliders[i].forwardFriction = forward;

            var sideways = wheelColliders[i].sidewaysFriction;
            sideways.stiffness = m_wheelFriction;
            wheelColliders[i].sidewaysFriction = sideways;
        }
    }

    //同步车轮模型与碰撞器
    private void SyncWheels()
    {
        for( int i = 0; i < wheelColliders.Length; i++ )
        {
            Vector3 pos;
            Quaternion quat;

            wheelColliders[i].GetWorldPose( out pos, out quat );        
            WheelModels[i].transform.SetPositionAndRotation( pos, quat );
        }
    }

    private void OnGUI()
    {
        WheelDebug();
    }

    private void WheelDebug()
    {
        WheelHit hit;
        wheelColliders[0].GetGroundHit( out hit );

        //Debug.Log( hit.sidewaysSlip );

        Debug.Log( hit.force );
    }
}
