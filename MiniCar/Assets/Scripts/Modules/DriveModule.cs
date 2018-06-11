using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveModule : MonoBehaviour {

    //公开只读数据
    public float Accel { get; private set; }    //加速数据
    public float Steer { get; private set; }    //转向数据
    public float Brake { get; private set; }    //刹车数据

    //赛车最大速度
    public float MaxSpeed { get { return m_maxSpeed; } }

    //赛车速度
    public Vector3 Velocity { get { return c_rigidbody.velocity; } }

    //赛车行驶方向
    public bool IsReverse
    {
        get
        {
            return Vector3.Angle( Velocity, -transform.forward ) < 10f;     //速度与反方向点乘小于10，则为向后行驶
        }
    }

    //当前动力扭矩
    public float CurMotorTorque
    {
        get
        {
            return m_maxMotorTorque * (m_maxGear - m_curGear);
        }
    }

    //空气阻力
    public Vector3 AirForce
    {
        get
        {
            return -m_airDrag * Velocity.sqrMagnitude * Velocity.normalized;
        }
    }
    
    //当前赛车转角
    public float CurSteerAngle
    {
        get
        {
            return m_maxSteerAngle;
        }
    }

    //当前刹车扭矩
    public float CurBrakeTorque
    {
        get
        {
            return m_maxBrakeTorque;
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
            return Mathf.Clamp01( c_rigidbody.velocity.magnitude / m_maxSpeed );
        }
    }

    //四轮着地
    public bool OnGround
    {
        get
        {
            //若有车轮未着地，则赛车标为未着地
            foreach ( var item in c_wheelColliders )
            {
                if ( !item.isGrounded )
                {
                    return false;
                }
            }

            return true;
        }
    }
    
    //内部组件 : c
    [SerializeField] private WheelCollider[] c_wheelColliders;              //车轮碰撞器
    [SerializeField] private Transform[] c_wheelMeshes;                     //车轮
    [SerializeField] private Rigidbody c_rigidbody;                         //车轮连接的刚体

    //内部数据成员 : m
    [SerializeField] private float m_maxSpeed = 30f;              //最高速度 m/s
    [SerializeField] private float m_maxReverseSpeed = 3f;       //最高倒车速度
    [SerializeField] private float m_maxMotorTorque = 600f;        //最高动力扭矩
    [SerializeField] private float m_maxSteerAngle = 30f;         //最大转向角
    [SerializeField] private float m_maxBrakeTorque = 1500f;        //最大制动扭矩
    [SerializeField] private float m_wheelFriction = 1f;         //车轮摩擦力
    [SerializeField] private float m_airDrag = 0.35f;               //空气阻力
    [SerializeField] private float m_downForce = 500f;             //下压力
    [SerializeField] private Transform m_centorOfMass;      //重心
    [SerializeField] private int m_maxGear = 5;             //最高档位
    [SerializeField] private int m_curGear = 1;             //初始为1
    [SerializeField] private float m_gearFactor;            //档位因数

    //状态判定成员
    [SerializeField] [Range( 0, 1 )] private float m_slipLimit = 0.8f;              //侧滑判定上限
    [SerializeField] [Range( 3, 10 )] private float m_slipVelocityLimit = 5f;       //侧滑时速度下限
    [SerializeField] [Range( 0, 1 )] private float m_steerHelper;                   //转向帮助

    //单函数成员
    private float m_OldRotation;           //记录赛车旋转角
    private Vector3 t_preFrameVelocity;    //上一帧速度

    //测试数据
    private void OnGUI()
    {
        GUILayout.Label( "speed:" + Velocity.magnitude );
    }
    private void Start()
    {
        c_rigidbody = c_wheelColliders[0].attachedRigidbody;        //获取挷定的刚体
    }

    private void FixedUpdate()
    {
        HandleInput();
        DriveMethod();
        CapMaxSpeed();
        SteerHelper();
        AdjustForce();
    }

    private void Update()
    {
        GearChanging();
        SyncWheelMeshes();
    }

    private void HandleInput()
    {
        Accel = Input.GetAxis( "Vertical" );
        Steer = Input.GetAxis( "Horizontal" );
        Brake = Input.GetAxis( "Jump" );
    }

    //赛车速度档控制
    private void GearChanging()
    {
        if ( m_maxGear * GearFactor > m_curGear + 1 && m_curGear <= m_maxGear )
        {
            ++m_curGear;
        }

        if ( m_maxGear * GearFactor < m_curGear && m_curGear > 1 )
        {
            --m_curGear;
        }
    }

    //每个车轮的控制方法
    private void ApplyWheelDrive(WheelCollider wh, float ac = 0, float br = 0, float st = 0)
    {
        wh.motorTorque = ac;
        wh.steerAngle = st;
        wh.brakeTorque = br;
    }

    //驱动车轮方法 in : fixedUpdate()
    private void DriveMethod()
    {
        float ac = Accel * m_maxMotorTorque * (1 - GearFactor * 0.3f);
        float st = Steer * m_maxSteerAngle;
        float br = Brake * m_maxBrakeTorque;
        
        Debug.Log( br );
        for ( int i = 0; i < 2; ++i )
        {
            ApplyWheelDrive( c_wheelColliders[i], ac, br, st );
        }

        for ( int i = 2; i < 4; i++ )
        {
            ApplyWheelDrive( c_wheelColliders[i], ac, br );
        }
    }
    
    //速度限制 in: fixedUpdate()
    private void CapMaxSpeed()
    {
        bool isReverse = IsReverse;
        Vector3 velocity = Velocity;
        float speed = velocity.magnitude;

        if ( isReverse && speed > m_maxReverseSpeed )
        {
            float offset = speed - m_maxReverseSpeed;      //超速差值
            Debug.Log( "反向超速！超速：" + offset );
            c_rigidbody.velocity = c_rigidbody.velocity.normalized * m_maxReverseSpeed;
            
        }
        
        if( !IsReverse && Velocity.magnitude > m_maxSpeed )
        {
            float offset = Velocity.magnitude - m_maxSpeed;
            Debug.Log( "正向超速！超速：" + offset );
            c_rigidbody.velocity = c_rigidbody.velocity.normalized * m_maxSpeed;
        }
    }

    //转向帮助 in: fixedUpdate()
    private void SteerHelper()
    {
        if ( !OnGround ) { return; }

        //通过修改速度方向来提升转向灵敏度
        if ( Mathf.Abs( m_OldRotation - transform.eulerAngles.y ) < 1f )
        {
            var turnAdjust = (transform.eulerAngles.y - m_OldRotation) * m_steerHelper;
            Quaternion velRotation = Quaternion.AngleAxis( turnAdjust, Vector3.up );
            c_rigidbody.velocity = velRotation * c_rigidbody.velocity;
        }

        m_OldRotation = transform.eulerAngles.y;
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

    //外力调整
    private void AdjustForce()
    {
        //调整下压力
        foreach ( var item in c_wheelColliders )
        {
            Vector3 pos;
            Quaternion quat;

            item.GetWorldPose( out pos, out quat );

            c_rigidbody.AddForceAtPosition( -0.25f * m_downForce * transform.up, pos );
        }
        
        //调整空气阻力
        c_rigidbody.AddForce( AirForce );
    }

    private void SyncWheelMeshes()
    {
        for ( int i = 0; i < 4; ++i )
        {
            Vector3 pos;
            Quaternion quat;

            c_wheelColliders[i].GetWorldPose( out pos, out quat );
            c_wheelMeshes[i].transform.SetPositionAndRotation( pos, quat );
        }
    }
}
