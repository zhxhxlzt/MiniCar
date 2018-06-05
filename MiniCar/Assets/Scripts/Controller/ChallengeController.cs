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
    [SerializeField] private LevelInfoList m_levelInfoList;     //所有关卡信息表
    private CheckPointController m_checkPointController;    //检查点控制器


    [Header( "=== UI组件 ===" )]
    public Text CountDownLabel;     //倒计时组件
    public Text TimeCountLabel;     //用时统计组件
    public Text TurnCountLabel;     //圈数组件
    public Text ResultLabel;        //结果显示组件

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

    //初始化控制器
    private void Start()
    {
        m_checkPointController = new CheckPointController();
        m_inputHandler = FindObjectOfType<InputHandler>();
        m_sceneController = FindObjectOfType<SceneController>();
        m_levelInfoList = Resources.Load<UserData>( "UserDatum/UserData" ).levelInfoList;

        CountDownLabel = GameObject.Find( "CountDown" ).GetComponent<Text>();
        TimeCountLabel = GameObject.Find( "TimeCount" ).GetComponent<Text>();
        TurnCountLabel = GameObject.Find( "TurnCount" ).GetComponent<Text>();
        ResultLabel = GameObject.Find( "Result" ).GetComponent<Text>();
        
        SetTimeCountLabel();                            //设置初始时间
        StartCoroutine( CheckChallengeState() );        //开始协程检测玩家闯关状态
    }

    private void FixedUpdate()
    {
        SetTimeCountLabel();        //在固定循环中调用时间统计
        CountTurn();                //检查圈数
        //Debug.Log( "场景名为：" + m_sceneController.CurrentSceneName );
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

    //暂停与恢复功能
    public void PauseAndResume()
    {
        if( m_challengeState == ChallengeState.underway )
        {
            Time.timeScale = 0f;
            m_challengeState = ChallengeState.paused;
        }
        else if( m_challengeState == ChallengeState.paused )
        {
            Time.timeScale = 1f;
            m_challengeState = ChallengeState.underway;
        }
    }

    //统计圈数
    private void CountTurn()
    {
        SetTurnCountLabel();
        if( m_checkPointController.AllPassed() )
        {
            m_turnCount++;
            m_checkPointController.ResetAllCheckPoint();
            Debug.Log( "已完成" + m_turnCount + "圈" );
        }
    }

    //用时统计
    private void CountTimeUsage()
    {
        if ( m_challengeState == ChallengeState.underway )
        {
            m_timeCount += Time.fixedDeltaTime;
        }
    }

    //设置用时UI
    private void SetTimeCountLabel()
    {
        CountTimeUsage();
        TimeCountLabel.text = "耗时：" + Convert.ToInt32(TimeCount).ToString() + "秒/" + TimeLimit.ToString() + "秒";
    }
    
    //设置圈数UI
    private void SetTurnCountLabel()
    {
        TurnCountLabel.text = "已完成：" + TurnCount.ToString() + "/" + TurnLimit.ToString() + "圈";
    }

    //设置闯关状态
    private void SetResultLabel(string info)
    {
        ResultLabel.enabled = true;
        ResultLabel.text = info;
    }

    //判断游戏状态
    IEnumerator CheckChallengeState()
    {
        while(true)
        {
            //开始闯关状态
            if ( m_challengeState == ChallengeState.begin )
            {
                m_inputHandler.MoveInput = false;              //在倒计时阶段关闭赛车控制
                StartCoroutine( BeignCountDown() );             //开始倒计时协程
                yield return new WaitForSeconds( m_countDown ); //等待倒计时结束
                m_challengeState = ChallengeState.underway;     //进入闯关中状态
            }

            //闯关进行状态
            if ( m_challengeState == ChallengeState.underway )
            {
                m_inputHandler.MoveInput = true;               //开启赛车控制
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
                m_inputHandler.MoveInput = false;
            }
            
            if ( m_challengeState == ChallengeState.failed )
            {
                SetResultLabel( "闯关失败！你真是个菜鸡！" );
                m_inputHandler.MoveInput = false;
                break;
            }

            if ( m_challengeState == ChallengeState.succeed )
            {
                SetResultLabel( "闯关成功！" );
                m_levelInfoList.GetLevelInfo( m_sceneController.CurrentSceneName ).Passed = true;
                m_inputHandler.MoveInput = false;
                break;
            }
            yield return null;
        }
    }

    //开始倒计时
    IEnumerator BeignCountDown()
    {
        CountDownLabel.gameObject.SetActive( true );
        int countDownTime = m_countDown;
        while(countDownTime > 0)
        {
            CountDownLabel.text = countDownTime.ToString();
            countDownTime--;
            yield return new WaitForSeconds( 1f );
        }

        CountDownLabel.text = "Go!";

        yield return new WaitForSeconds( 1f );

        CountDownLabel.gameObject.SetActive( false );
    }
}
