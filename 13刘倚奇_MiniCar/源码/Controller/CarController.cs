using UnityEngine;
using System;

public enum CarState { forward, backward, idle };

public class CarController : MonoBehaviour {
    
    //单例
    public static CarController Instance { get; private set; }

    //检查单例
    private void Awake()
    {
        if ( Instance == null ) { Instance = this; } else
        {
            Destroy( this );
        }
    }

    //初始化
    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();                    //获取刚体
        m_centorOfMass = transform.Find( "CenterOfMass" );          //获取重心
        m_rigidbody.centerOfMass = m_centorOfMass.position;         //设置重心
        m_parts = Resources.Load<UserData>( "UserDatum/UserData" ).currentCar;  //零件资源加载
        AdjustCarAttribute();                                       //初始化赛车属性
        m_wheelColliders[0].ConfigureVehicleSubsteps( 5, 10, 5 );   //应用车轮碰撞器迭代步数

        InputHandler.Instance.OnDriveInputChange += Drive;          //订阅赛车驾驶输入
    }

    public event Action<float> OnCollid;                    //发送碰撞事件

    [Header( "=== 外部组件 ===" )]
    [SerializeField] private UserCarParts m_parts;          //当前赛车配置
    [SerializeField] private Rigidbody m_rigidbody;         //赛车刚体RigidBody
    [SerializeField] private Transform m_centorOfMass;      //重心

    [Header( "=== 车轮模型 ===" )]
    [SerializeField] private Transform[] m_wheelModels;

    [Header( "=== 车轮碰撞器 ===" )]
    [SerializeField] private WheelCollider[] m_wheelColliders;

    [Header( "=== 赛车属性 ===" )]
    [SerializeField] private float m_maxSpeed;              //最高速度 m/s
    [SerializeField] private float m_maxReverseSpeed;       //最高倒车速度
    [SerializeField] private float m_maxMotorTorque;        //最高动力扭矩
    [SerializeField] private float m_maxSteer;              //最大转向角
    [SerializeField] private float m_maxBrakeTorque;        //最大制动扭矩
    [SerializeField] private float m_wheelFriction;         //车轮摩擦力
    [SerializeField] private float m_airDrag;               //空气阻力
    [SerializeField] private float m_downForce;             //下压力
    
    [SerializeField] private int m_maxGear = 5;             //最高档位
    [SerializeField] private int m_curGear = 0;             //初始为0
    [SerializeField] private float m_gearFactor;            //档位因数
    [SerializeField] private float m_curTorque;             //当前档位

    [Header( "=== 赛车调试 ===" )]
    [SerializeField] [Range( 0, 1 )] private float m_slipLimit = 0.8f;                      //侧滑判定上限
    [SerializeField] [Range( 1000, 10000 )] private float m_collisionImpulseLimit = 1500f;  //产生碰撞音效的力大小限定
    [SerializeField] [Range( 0, 1 )] private float m_steerHelper = 0.25f;                   //转向帮助
    [SerializeField] [Range( 0, 0.1f )] private float m_shiftHelper = 0.05f;               //漂移帮助

    [Header("=== 用作函数静态变量 ===")]
    [SerializeField] private float t_OldRotation;           //记录赛车旋转角
    [SerializeField] private Vector3 t_preFrameVelocity;    //上一帧速度
    
    //赛车最大速度
    public float MaxSpeed { get { return m_maxSpeed; } }

    //赛车速度
    public Vector3 Velocity { get { return m_rigidbody.velocity; } }

    //赛车加速度
    public Vector3 Acceleration { get; private set; }

    //空气阻力
    public Vector3 AirForce{ get; private set; }

    //赛车是否侧滑
    public bool Sliped
    {
        get
        {
            foreach ( var item in m_wheelColliders )
            {
                if ( CheckSidewaysSlip( item, m_slipLimit ) )
                {
                    return true;
                }
            }

            return false;
        }
    }

    //赛车是否着地
    public bool OnGround {
        get
        {
            //若有车轮未着地，则赛车标为未着地
            foreach ( var item in m_wheelColliders )    
            {
                if(!item.isGrounded)
                {
                    return false;
                }
            }

            return true;
        }
    }

    //根据正向摩擦曲线计算最佳扭矩
    public float FigureBestTorque(WheelCollider wheel)
    {
        WheelHit hit;
        wheel.GetGroundHit( out hit );

        float torque = hit.force * wheel.forwardFriction.extremumValue * 0.15f;

        return torque;
    }

    //计算加速度
    private void CalAccel()
    {
        Acceleration = (Velocity - t_preFrameVelocity) / Time.fixedDeltaTime;
        t_preFrameVelocity = Velocity;
    }
    
    //发生碰撞时，检测是否达到碰撞要求
    private void OnCollisionEnter(Collision collision)
    {
        if ( collision.impulse.magnitude > m_collisionImpulseLimit )
        {
            if ( OnCollid != null )
            {
                OnCollid(SpeedFactor);
            }
        }
    }
    
    //根据赛车各零件调整赛车属性
    private void AdjustCarAttribute()
    {
        if ( m_parts == null )
        {
            m_parts = FindObjectOfType<UserCarParts>();
            if ( m_parts == null )
            {
                Debug.Log( "userParts Not exists" );
                return;
            }
        }
        
        m_rigidbody.centerOfMass = m_centorOfMass.transform.localPosition;   //设置赛车重心
        
        //motor零件属性
        m_maxSpeed = (2f * Mathf.PI * m_wheelColliders[0].radius) * m_parts.motor.RPM / 60f;//最大速度(m/s) = 车轮周长 * rpm / 60
        m_maxMotorTorque = m_parts.motor.MotorTorque;    //最大动力扭矩为马达零件扭矩

        //wheel零件属性
        m_maxBrakeTorque = m_parts.wheel.BrakeTorque;    //最大刹车扭矩为车轮零件刹车扭矩
        m_wheelFriction = m_parts.wheel.Friction;        //车轮摩擦力

        //cover零件属性
        m_airDrag = m_parts.cover.AirDrag;               //空气阻力
        m_downForce = m_parts.cover.DownForce;           //下压力
    }

    //赛车速度档控制
    public void CalCurGear()
    {
        if ( GearFactor * m_maxGear > m_curGear && m_curGear <= m_maxGear )
        {
            ++m_curGear;
        }

        if ( GearFactor * m_maxGear < m_curGear && m_curGear > 1 )
        {
            --m_curGear;
        }
    }

    //赛车当前档位因数
    public float GearFactor
    {
        get
        {
            m_gearFactor = Mathf.Sqrt( SpeedFactor );

            return m_gearFactor;
        }
    }
    
    //速度因数
    public float SpeedFactor
    {
        get
        {
            return Velocity.magnitude / MaxSpeed;
        }
    }

    //驾驶状态
    public CarState GetCarState()
    {
        if ( Velocity.magnitude <= 0.1f )
        {
            return CarState.idle;
        }

        if ( Vector3.Angle( Velocity, -transform.forward ) < 30f )
        {
            return CarState.backward;
        }
        else
        {
            return CarState.forward;
        }
    }
    
    //如果超过最高速度，则施加相反方向的速度
    private void CapMaxSpeed()
    {
        if( GetCarState() == CarState.backward )
        {
            float offset = Velocity.magnitude - m_maxReverseSpeed;

            if ( offset > 0 )
            {
                m_rigidbody.AddForce( -Velocity * offset, ForceMode.Acceleration );
            }
        }

        else
        {
            float offset= Velocity.magnitude - MaxSpeed;

            //如果正向行驶超过最大正向速度
            if ( offset > 0 )
            {
                m_rigidbody.AddForce( -Velocity * offset, ForceMode.Acceleration );
            }
        }
        
    }

    //驾驶控制
    public void Drive( float accel, float steer, float brake )
    {
        //各车轮动力扭矩,超过最高速度则为0
        float motorTorque = accel * m_maxMotorTorque * Mathf.Clamp01( m_maxGear - m_curGear * 0.5f );
        float brakeTorque = brake * m_maxBrakeTorque;   //各车轮制动扭矩
        float steerAngle = m_maxSteer * steer * Mathf.Clamp01(1 - SpeedFactor * 0.25f);   //转向角

        //当赛车没有动力时，自动进行减速
        if ( motorTorque == 0 || 
            (GetCarState() == CarState.forward && motorTorque < 0) || 
            GetCarState() == CarState.backward && motorTorque > 0 )  
        {
            brakeTorque += m_maxBrakeTorque * 0.15f;
            motorTorque = 0;
        }

        //四轮动力
        for ( int i = 0; i < 4; i++ )
        {
            m_wheelColliders[i].motorTorque = motorTorque / 4; 
        }

        //四轮刹车
        for ( int i = 0; i < 4; i++ )
        {
            m_wheelColliders[i].brakeTorque = brakeTorque / 4; 
        }

        //前轮转向
        for ( int i = 0; i < 2; i++ )
        {
            m_wheelColliders[i].steerAngle = steerAngle;   
        }

        CalAccel();                  //计算加速度
        CapMaxSpeed();               //超速锁定
        SteerHelper( steer );        //转向帮助
        SyncWheelMeshes();           //同步车轮模型与碰撞器
        AdjustCarAttribute();        //调整赛车属性
        AdjustForce();               //赛车受力调整
        CalCurGear();                //计算当前档位
    }

    //转向帮助
    private void SteerHelper( float steer )
    {
        if( !OnGround ) { return; }

        //Vector3 BackAxisCenter = ( m_wheelColliders[2].transform.position + m_wheelColliders[3].transform.position ) * 0.5f;
        //Vector3 BackAxisForce = transform.right * steer * m_shiftHelper * Velocity.magnitude;

        //m_rigidbody.AddForceAtPosition( BackAxisCenter, BackAxisForce, ForceMode.VelocityChange );
        
        //通过修改速度方向来提升转向灵敏度
        if ( Mathf.Abs( t_OldRotation - transform.eulerAngles.y ) < 10f )
        {
            var turnAdjust = (transform.eulerAngles.y - t_OldRotation) * m_steerHelper;
            Quaternion velRotation = Quaternion.AngleAxis( turnAdjust, Vector3.up );
            m_rigidbody.velocity = velRotation * m_rigidbody.velocity;
        }

        t_OldRotation = transform.eulerAngles.y;
    }

    //检查是否有车轮侧滑
    private bool CheckSidewaysSlip(WheelCollider wc, float slipLimit)
    {
        WheelHit hit;
        wc.GetGroundHit( out hit );

        // 车轮滑动超过滑动上限
        if ( Mathf.Abs( hit.sidewaysSlip ) >= slipLimit )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //赛车受力调整
    private void AdjustForce()
    {
        //调整下压力
        foreach ( var item in m_wheelColliders )
        {
            Vector3 pos;
            Quaternion quat;

            item.GetWorldPose( out pos, out quat );

            m_rigidbody.AddForceAtPosition( - 0.25f * m_downForce * transform.up, pos);
        }
        
        AirForce = -m_airDrag * Velocity.sqrMagnitude* Velocity.normalized;     ////空气阻力正比于速度的平方

        //调整空气阻力
        m_rigidbody.AddForce( AirForce );    
        
        //调整车轮摩擦力
        for(int i = 0; i < m_wheelColliders.Length; i++ )
        {
            var forward = m_wheelColliders[i].forwardFriction;
            forward.stiffness = m_wheelFriction;
            m_wheelColliders[i].forwardFriction = forward;

            var sideways = m_wheelColliders[i].sidewaysFriction;
            sideways.stiffness = m_wheelFriction;
            m_wheelColliders[i].sidewaysFriction = sideways;
        }
        
    }

    //同步车轮模型与碰撞器www
    private void SyncWheelMeshes()
    {
        for( int i = 0; i < m_wheelColliders.Length; ++i )
        {
            Vector3 pos;
            Quaternion quat;


            m_wheelColliders[i].GetWorldPose( out pos, out quat );        
            m_wheelModels[i].transform.SetPositionAndRotation( pos, quat );
        }
    }
}
