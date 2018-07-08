using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

 /// <summary>
 /// 用户输入检测
 /// </summary>
public class InputHandler : MonoBehaviour {

    public static InputHandler Instance { get; private set; }

    private void Awake()
    {
        if( Instance == null ) { Instance = this; }
        else
        {
            Destroy( this );
        }
        
    }

    private void Start()
    {
        //订阅闯关控制
        Debug.Log( "开始订阅" );
        ChallengeController.Instance.OnChallengePrepare += DisableCarInput;
        ChallengeController.Instance.OnChallengeStart += EnableCarInput;
        ChallengeController.Instance.OnChallengePaused += DisableCarInput;
        ChallengeController.Instance.OnChallengeFailed += DisableCarInput;
        ChallengeController.Instance.OnChallengeSucceed += DisableCarInput;
    }

    private void FixedUpdate()
    {
        HandleInput();  //处理赛车相关输入
    }

    private bool m_acceptCarInput = true;    //是否处理移动输入
    private bool  m_acceptUIInput = true;    //是否处理UI输入

    [Header("=== 赛车数据 ===")]
    [SerializeField] private float m_accel = 0f;    //加减速，倒车;  数值范围[-1，1]
    [SerializeField] private float m_steer = 0f;    //转向;  数值范围[-1, 1]
    [SerializeField] private float m_brake = 0f;    //刹车;  数值范围[0, 1]

    [Header( "=== UI数据 ===" )]
    [SerializeField] private bool m_escape = false;   //取消命令

    //外部获取移动参数
    public event Action<float, float, float> OnDriveInputChange;
    public event Action OnEscape;
    
    //赛车输入控制
    public bool AcceptCarInput
    {
        get { return m_acceptCarInput; }
        set
        {
            m_acceptCarInput = value;

            if( !m_acceptCarInput )
            {
                BeforeCarInputInterruption();   
            }
        }
    }

    //UI输入控制
    public bool AcceptUIInput
    {
        get { return m_acceptUIInput; }
        set
        {
            m_acceptUIInput = value;
            if( !m_acceptUIInput )
            {
                BeforeUIInputInterruption();
            }
        }
    }

    //获取赛车输入
    public float Accel { get { return m_accel; } }
    public float Steer { get { return m_steer; } }
    public float Brake { get { return m_brake; } }

    //获取返回数据与发布事件
    public bool Escape
    {
        get { return m_escape; }
        private set
        {
            m_escape = value;
            if( OnEscape != null && m_escape)
            {
                OnEscape();
            }
        }
    }

    //当不接受输入时，为UI设置最后一次状态
    private void BeforeUIInputInterruption()
    {
        Escape = false;
    }

    //当不接受输入时，为赛车设置最后一次状态
    private void BeforeCarInputInterruption()
    {
        OnDriveInputChange( 0, 0, 1 );
    }

    //处理输入
    private void HandleInput()
    {
        //若为ture，处理输入
        if ( m_acceptCarInput )
        {
            m_accel = Input.GetAxis( "Vertical" );
            m_steer = Input.GetAxis( "Horizontal" );
            m_brake = Input.GetAxis( "Jump" );
            if( OnDriveInputChange != null)
            {
                OnDriveInputChange( m_accel, m_steer, m_brake );
            }
        }

        if ( m_acceptUIInput )
        {
            Escape = Input.GetKeyUp( KeyCode.Escape );
        }
    }

    private void EnableCarInput()
    {
        AcceptCarInput = true;
    }

    private void DisableCarInput()
    {
        AcceptCarInput = false;
    }

}
