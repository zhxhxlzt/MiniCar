using System.Collections;
using UnityEngine;
using System;

/* 关卡控制器：
 * 1，统计赛车行驶圈数
 * 2，统计赛车用时
 * 3，判断胜负
 * */
[Serializable]
public enum ChallengeState { prepare, start, underway, paused, failed, succeed}  //闯关状态

public class ChallengeController : MonoBehaviour {

    public static ChallengeController Instance { get; private set; }

    [Header( "=== 关卡控制器配置属性 ===" )]
    [SerializeField] private float m_timeLimit;      //过关时间限制
    [SerializeField] private float m_timeCount;      //当前用时
    [SerializeField] private int m_countDown;        //读秒
    [SerializeField] private int m_turnLimit;        //过关圈数限制
    [SerializeField] private int m_turnCount;        //当前圈数
    [SerializeField] private ChallengeState m_challengeState;          //闯关状态

    [Header( "=== 外部组件 ===" )]
    [SerializeField] private LevelInfoList m_levelInfoList;     //所有关卡信息表
    private LevelInfo m_curInfo;    //当前关卡信息
    private CheckPointController m_checkPointController;        //检查点控制器

    //事件
    public event Action OnChallengePrepare;     //准备阶段调用
    public event Action OnChallengeStart;       //开始阶段调用
    public event Action OnChallengeGoingOn;     //进行阶段调用
    public event Action OnChallengeSucceed;     //成功阶段调用
    public event Action OnChallengePaused;      //暂停阶段调用
    public event Action OnChallengeFailed;      //失败阶段调用
    public event Action OnTurnFinished;         //当完成一圈时
    public event Action OnTimeCountChanged;     //当计时进行时

    //公有获取只读数据方法，
    public float TimeLimit { get { return m_timeLimit; } }
    public float TimeCount { get { return m_timeCount; } }
    public float TurnLimit { get { return m_turnLimit; } }
    public float TurnCount { get { return m_turnCount; } }
    public int   CountDown { get { return m_countDown; } }

    //初始化参数
    private void Awake()
    {
        if( Instance == null ) { Instance = this; }
        else
        {
            Destroy( this );
        }

        m_timeCount = 0;
        m_turnCount = 0;
        m_countDown = 5;
        m_challengeState = ChallengeState.start;

        InitLevelLimit();   //初始化关卡限制
    }

    //初始化
    private void Start()
    {
        m_checkPointController = new CheckPointController();

        StartCoroutine( CheckChallengeState() );        //开始协程安排闯关状态
    }

    //初始化关卡限制
    private void InitLevelLimit()
    {
        m_levelInfoList = Resources.Load<UserData>( "UserDatum/UserData" ).levelInfoList;
        m_challengeState = ChallengeState.prepare;
        m_curInfo = m_levelInfoList.FindLevelInfo( Director.Instance.CurrentSceneName );
        m_timeLimit = m_curInfo.PassTimeLimit;
        m_turnLimit = m_curInfo.PassTurnLimit;
    }

    //暂停开关
    public void PauseSwitch()
    {
        //当暂停时，进入暂停状态
        if( m_challengeState == ChallengeState.underway )
        {
            m_challengeState = ChallengeState.paused;
            return;
        }

        //当恢复时，进入开始状态
        if( m_challengeState == ChallengeState.paused )
        {
            m_challengeState = ChallengeState.start;
            return;
        }
    }

    //修改闯关状态
    public void SetChallengeState( ChallengeState state )
    {
        m_challengeState = state;
        //switch(state)
        //{
        //    case 0:
        //        m_challengeState = ChallengeState.start;
        //        break;
        //    case 1:
        //        m_challengeState = ChallengeState.underway;
        //        break;
        //    case 2:
        //        m_challengeState = ChallengeState.paused;
        //        break;
        //    case 3:
        //        m_challengeState = ChallengeState.failed;
        //        break;
        //    case 4:
        //        m_challengeState = ChallengeState.succeed;
        //        break;
        //}
    }

    //统计圈数
    private void CountTurn()
    {
        if( m_checkPointController.AllPassed() )
        {
            m_turnCount++;  //若通过所有检查点，完成圈数加1
            m_checkPointController.ResetAllCheckPoint();    //重置所有检查点，开始记录下一圈

            if( OnTurnFinished != null )
            {
                OnTurnFinished();   //完成一圈时，发布此事件
            }
            
            Debug.Log( "已完成" + m_turnCount + "圈" );
        }
    }

    //用时统计
    private void CountTimeUsage()
    {
        //只有在闯关进行中状态才会累计时间
        if ( m_challengeState == ChallengeState.underway )
        {
            m_timeCount += Time.fixedDeltaTime;
            if( OnTimeCountChanged != null )
            {
                OnTimeCountChanged();
            }
        }
    }
    
    //判断游戏状态
    IEnumerator CheckChallengeState()
    {
        yield return null;
        Debug.Log( "开始协程" );
        //开始闯关
        if( m_challengeState == ChallengeState.prepare )
        {
            if ( OnChallengePrepare != null )
            {
                OnChallengePrepare();
            }

            yield return new WaitForSeconds( m_countDown );     //等待倒计时结束
            m_challengeState = ChallengeState.start;
        }

        Start:

        if( m_challengeState == ChallengeState.start )
        {
            if ( OnChallengeStart != null )
            {
                OnChallengeStart(); //恢复赛车控制，恢复音量
            }
            m_challengeState = ChallengeState.underway;
        }
        
        //进入闯关循环
        while( true )
        {
            if( m_challengeState == ChallengeState.underway )
            {
                CountTimeUsage();           //检查用时
                CountTurn();                //检查圈数

                if ( OnChallengeGoingOn != null )
                {
                    OnChallengeGoingOn();   //UI信息
                }

                //若闯关用时大于时间限制，则闯关失败
                if ( m_timeCount > m_timeLimit )
                {
                    m_challengeState = ChallengeState.failed;
                    break;
                }

                //若在规定时间内完成指定圈数，则成功
                if ( m_turnCount == m_turnLimit )
                {
                    m_challengeState = ChallengeState.succeed;
                    break;
                }
            }

            else if ( m_challengeState == ChallengeState.paused )
            {
                if( OnChallengePaused != null ) { OnChallengePaused(); }    //静音，关闭赛车控制

                Debug.Log( "游戏暂停中" );
            }

            //在闯关循环进行中，从暂停中继续开始。
            else if ( m_challengeState == ChallengeState.start )
            {
                if( OnChallengeStart != null ) { OnChallengeStart(); }
                goto Start; //跳转到新开始状态
            }

            yield return new WaitForFixedUpdate();
        }

        //闯关失败
        if ( m_challengeState == ChallengeState.failed )
        {
            if ( OnChallengeFailed != null ) { OnChallengeFailed(); }   //失败时调用
        }

        //闯关成功
        if ( m_challengeState == ChallengeState.succeed )
        {
            if ( OnChallengeSucceed != null ) { OnChallengeSucceed(); }     //成功时调用
            
            if ( m_curInfo != null)
            {
                Debug.Log( "闯关成功，保存数据！" );
                m_curInfo.SetPass();
                m_curInfo.TimeUsage = TimeCount;
            }
        }
    }
}
