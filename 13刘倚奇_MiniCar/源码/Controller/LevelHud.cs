using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelHud : MonoBehaviour {
    
    [Header( "=== UI组件 ===" )]
    [SerializeField] private CanvasGroup SystemMenu; //暂停菜单
    [SerializeField] private Text CountDownLabel;    //倒计时Text
    [SerializeField] private Text TimeCountLabel;    //计时Text
    [SerializeField] private Text TurnCountLabel;    //计圈Text
    [SerializeField] private Text SpeedLabel;        //速度Text
    [SerializeField] private Text ResultLabel;       //结果Text
    [SerializeField] private Text AlertLabel;        //警告Text
    [SerializeField] private Button MenuButton;      //菜单button
    [SerializeField] private Button RestartButton;   //重新开始
    [SerializeField] private Button ReturnButton;    //返回

    [Header( "=== 控制参数 ===" )]
    [SerializeField] private bool m_showMenu;

    private void Start()
    {
        FindHUD();
        SetTimeCountLabel();    //设置初始计时
        SetTurnCountLabel();    //设置初始圈数

        InputHandler.Instance.OnEscape += ShowMenu;                             //订阅用户输入事件
        ChallengeController.Instance.OnTurnFinished += SetTurnCountLabel;       //完成一圈时，设置圈数label
        ChallengeController.Instance.OnTimeCountChanged += SetTimeCountLabel;   //当计时进行时，设置时间
        SubscribeChallenge();                                                   //订阅闯关事件
        RestartButton.onClick.AddListener( OnRestartButtonClick );              //为重新开始按钮添加监听方法
        ReturnButton.onClick.AddListener( OnReturnButtonClick );                //为返回按钮添加监听方法
        MenuButton.onClick.AddListener( ShowMenu );                             //为菜单按钮添加监听方法
    }

    private void Update()
    {
        SetSpeedLabel();
    }

    //查找UI组件
    private void FindHUD()
    {
        SystemMenu = FindUIComponent<CanvasGroup>( "SystemMenu" );
        CountDownLabel = FindUIComponent<Text>( "CountDown" );
        TimeCountLabel = FindUIComponent<Text>( "TimeCount" );
        TurnCountLabel = FindUIComponent<Text>( "TurnCount" );
        SpeedLabel = FindUIComponent<Text>( "Speed" );
        ResultLabel = FindUIComponent<Text>( "Result" );
        AlertLabel = FindUIComponent<Text>( "Alert" );
        MenuButton = FindUIComponent<Button>( "MenuButton" );
        RestartButton = FindUIComponent<Button>( "Restart" );
        ReturnButton = FindUIComponent<Button>( "Return" );
    }

    //查找UI组件
    private T FindUIComponent<T>(string name)
    {
        return GameObject.Find( name ).GetComponent<T>();
    }

    public void ShowMenu()
    {
        m_showMenu = !m_showMenu;

        if ( m_showMenu )
        {
            Debug.Log( "ESC" );
            if ( SystemMenu.alpha == 0 )
            {
                SystemMenu.alpha = 1;
                SystemMenu.blocksRaycasts = true;
            }
            else
            {
                SystemMenu.alpha = 0;
                SystemMenu.blocksRaycasts = false;
            }

            m_showMenu = !m_showMenu;
        }
    }
    
    //设置结果信息
    public void SetAlertLabel(string info)
    {
        AlertLabel.enabled = true;
        AlertLabel.text = info;
    }

    //警告错误方向
    public void SetWrongDirection(int times)
    {
        SetAlertLabel( "方向错误！" );
        StopCoroutine( BlinkResult( 3 ) );
        StartCoroutine( BlinkResult( times ) );
    }

    //设置倒计时
    public void SetCountDown()
    {
        int time = ChallengeController.Instance.CountDown;
        if( CountDownLabel == null)
        {
            CountDownLabel = FindUIComponent<Text>( "CountDown" );
        }

        StartCoroutine( BeignCountDown( time ) );
    }

    public void SetSuccess()
    {
        ResultLabel.text = "闯关成功！";
    }

    public void SetFailure()
    {
        ResultLabel.text = "闯关失败！";
    }

    //设置圈数UI
    public void SetTurnCountLabel()
    {
        TurnCountLabel.text = "已完成：" + 
                              ChallengeController.Instance.TurnCount.ToString() + 
                              "/" + 
                              ChallengeController.Instance.TurnLimit.ToString() + 
                              "圈";
    }

    //设置用时UI
    public void SetTimeCountLabel()
    {
        TimeCountLabel.text = "耗时：" + 
                              Convert.ToInt32( ChallengeController.Instance.TimeCount ).ToString() + 
                              "秒/" + 
                              ChallengeController.Instance.TimeLimit.ToString() + 
                              "秒";
    }

    //设置速度UI
    private void SetSpeedLabel()
    {
        SpeedLabel.text = "当前速度：" + 
                          Convert.ToInt32( CarController.Instance.Velocity.magnitude * 3.6f ).ToString() + 
                          "km/h" ;
    }

    //闪烁UI组件
    IEnumerator BlinkResult(int times)
    {
        for ( int i = 0; i < times; i++ )
        {
            AlertLabel.enabled = true;
            yield return new WaitForSeconds( 0.5f );    //每隔0.5秒闪烁一次
            AlertLabel.enabled = false;
            yield return new WaitForSeconds( 0.5f );
        }
    }
    
    //开始倒计时
    IEnumerator BeignCountDown(int time)
    {
        CountDownLabel.enabled = true;

        //当time大于0时，每秒减1
        while ( time > 0 )
        {
            CountDownLabel.text = time.ToString();
            time--;
            yield return new WaitForSeconds( 1f );
        }

        CountDownLabel.text = "出发！";    //倒计时结束时显示出发

        yield return new WaitForSeconds( 1f );

        CountDownLabel.enabled = false;
    }

    private void SubscribeChallenge()
    {
        ChallengeController.Instance.OnChallengePrepare += SetCountDown;
        ChallengeController.Instance.OnChallengeSucceed += SetSuccess;
        ChallengeController.Instance.OnChallengeFailed += SetFailure;
    }

    private void OnReturnButtonClick()
    {
        Director.Instance.LoadScene( "MainScene" );
    }

    private void OnRestartButtonClick()
    {
        Director.Instance.LoadScene( Director.Instance.CurrentSceneName );
    }
}
