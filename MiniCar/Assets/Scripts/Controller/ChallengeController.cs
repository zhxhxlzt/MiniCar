using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
/* 关卡控制器：
 * 1，统计赛车行驶圈数
 * 2，统计赛车用时
 * 3，判断胜负
 * */

public enum ChallengeState { begin, underway, paused, failed, succeed}  //闯关状态

public class ChallengeController : MonoBehaviour {

    [Header( "=== 关卡控制器配置属性 ===" )]
    [SerializeField] private float m_timeLimit;      //过关时间限制
    [SerializeField] private float m_timeCount;      //当前用时
    [SerializeField] private int m_countDown;        //读秒
    [SerializeField] private int m_turnLimit;        //过关圈数限制
    [SerializeField] private int m_turnCount;        //当前圈数

    [Header( "=== 控制参数 ===" )]
    public ChallengeState m_challengeState;          //闯关状态

    [Header( "=== 外部控制器 ===" )]
    [SerializeField] private InputHandler m_inputHandler;   //输入控制器
    [SerializeField] private SceneController m_sceneController; //场景控制器
    [SerializeField] private LevelHud m_levelHud;               //关卡HUD
    [SerializeField] private LevelInfoList m_levelInfoList;     //所有关卡信息表
    [SerializeField] private AudioController m_audioController; //音效控制器
    private CheckPointController m_checkPointController;    //检查点控制器

    //公有获取只读数据方法，
    public float TimeLimit { get { return m_timeLimit; } }
    public float TimeCount { get { return m_timeCount; } }
    public int CountDown { get { return m_countDown; } }
    public float TurnLimit { get { return m_turnLimit; } }
    public float TurnCount { get { return m_turnCount; } }

    //初始化参数
    private void Awake()
    {
        m_timeCount = 0;
        m_turnCount = 0;
        m_challengeState = ChallengeState.begin;
    }

    //初始化
    private void Start()
    {
        m_checkPointController = new CheckPointController();
        m_inputHandler = FindObjectOfType<InputHandler>();
        m_sceneController = FindObjectOfType<SceneController>();
        m_levelHud = FindObjectOfType<LevelHud>();

        m_levelInfoList = Resources.Load<UserData>( "UserDatum/UserData" ).levelInfoList;
        
        StartCoroutine( CheckChallengeState() );        //开始协程检测玩家闯关状态
    }

    private void OnDestroy()
    {
        Time.timeScale = 1; //重置时间缩放
    }

    private void FixedUpdate()
    {
        CountTurn();                //检查圈数
        CountTimeUsage();           //计时
    }

    //暂停开关
    public void PauseSwitch()
    {
        if( m_challengeState == ChallengeState.paused )
        {
            Time.timeScale = 1f;
            m_challengeState = ChallengeState.underway;
            m_audioController.MuteSwitch();
        }
        else if( m_challengeState == ChallengeState.underway)
        {
            Time.timeScale = 0f;
            m_challengeState = ChallengeState.paused;
            m_audioController.MuteSwitch();
        }
    }

    //修改闯关状态
    public void SetChallengeState( int state )
    {
        switch(state)
        {
            case 0:
                m_challengeState = ChallengeState.begin;
                break;
            case 1:
                m_challengeState = ChallengeState.underway;
                break;
            case 2:
                m_challengeState = ChallengeState.paused;
                break;
            case 3:
                m_challengeState = ChallengeState.failed;
                break;
            case 4:
                m_challengeState = ChallengeState.succeed;
                break;
        }
    }

    //统计圈数
    private void CountTurn()
    {
        if( m_challengeState == ChallengeState.underway && m_checkPointController.AllPassed())
        {
            m_turnCount++;  //若通过所有检查点，完成圈数加1
            m_checkPointController.ResetAllCheckPoint();    //重置所有检查点，开始记录下一圈
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
        }
    }
    
    //判断游戏状态
    IEnumerator CheckChallengeState()
    {
        while(true)
        {
            //开始闯关状态
            if ( m_challengeState == ChallengeState.begin )
            {
                m_inputHandler.handleCarInput = false;              //在倒计时阶段关闭赛车控制
                m_levelHud.SetCountDown(CountDown);             //开始倒计时协程
                yield return new WaitForSeconds( m_countDown ); //等待倒计时结束
                m_challengeState = ChallengeState.underway;     //进入闯关中状态
            }

            //闯关进行状态
            if ( m_challengeState == ChallengeState.underway )
            {
                m_inputHandler.handleCarInput = true;               //开启赛车控制

                //若闯关用时大于时间限制，则闯关失败
                if ( m_timeCount > m_timeLimit )
                {
                    m_challengeState = ChallengeState.failed;
                }

                //若在规定时间内完成指定圈数，则成功
                if ( m_turnCount == m_turnLimit )
                {
                    m_challengeState = ChallengeState.succeed;
                }
            }

            //闯关暂停状态
            if ( m_challengeState == ChallengeState.paused )
            {
                //m_inputHandler.MoveInput = false;     //在此状态测试赛车功能
                Debug.Log( "游戏暂停中" );
            }
            
            if ( m_challengeState == ChallengeState.failed )
            {
                m_levelHud.SetAlertLabel( "闯关失败！" );

                m_inputHandler.handleCarInput = false;   //禁用输入
                m_audioController.FinishSnapShot(); //设置结束时的音效
                break;
            }

            if ( m_challengeState == ChallengeState.succeed )
            {
                var info = m_levelInfoList.GetLevelInfo( m_sceneController.CurrentSceneName );  //获取当前关卡信息
                
                info.Passed = true;     //设置通过当前关卡
                info.SetTimeUsage( Convert.ToInt32( TimeCount ) );      //设置用时

                m_levelHud.SetAlertLabel( "  闯关成功！\n" +
                                           "本次用时：" + Convert.ToInt32( TimeCount ) + "秒\n" +
                                           "最佳记录: " + Convert.ToInt32( info.TimeUsage ) + "秒" );

                m_inputHandler.handleCarInput = false;   //禁用输入
                m_audioController.FinishSnapShot(); //设置结束时的音效

                break;
            }
            yield return null;
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log( "Saved!" );
        m_levelInfoList.Save();
    }
}
