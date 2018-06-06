using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelHud : MonoBehaviour {

    [Header( "=== 外部控制器 ===" )]
    [SerializeField] private CarController m_carController;
    [SerializeField] private ChallengeController m_challengeController;

    [Header( "=== UI组件 ===" )]
    [SerializeField] private Button PauseButton;      //暂停按钮
    [SerializeField] private Button ReturnButton;     //返回主界面按钮
    [SerializeField] private Text CountDownLabel;    //倒计时Text
    [SerializeField] private Text TimeCountLabel;    //计时Text
    [SerializeField] private Text TurnCountLabel;    //计圈Text
    [SerializeField] private Text SpeedLabel;        //速度Text
    [SerializeField] private Text ResultLabel;       //结果Text

    private void Start()
    {
        m_carController = FindObjectOfType<CarController>();
        m_challengeController = FindObjectOfType<ChallengeController>();

        FindAllUIComponent();
    }

    private void OnGUI()
    {
        ShowHud();
    }

    private void FindAllUIComponent()
    {
        PauseButton = FindUIComponent<Button>( "Pause" );
        ReturnButton = FindUIComponent<Button>( "Return" );
        CountDownLabel = FindUIComponent<Text>( "CountDown" );
        TimeCountLabel = FindUIComponent<Text>( "TimeCount" );
        TurnCountLabel = FindUIComponent<Text>( "TurnCount" );
        SpeedLabel = FindUIComponent<Text>( "Speed" );
        ResultLabel = FindUIComponent<Text>( "Result" );
    }

    //查找UI组件
    private T FindUIComponent<T>(string name)
    {
        return GameObject.Find( name ).GetComponent<T>();
    }

    //显示闯关HUD
    private void ShowHud()
    {
        SetTurnCountLabel();
        SetTimeCountLabel();
        SetSpeedLabel();
    }

    //设置闯关状态
    public void SetResultLabel(string info)
    {
        ResultLabel.enabled = true;
        ResultLabel.text = info;
    }

    //警告错误方向
    public void SetWrongDirection(int times)
    {
        SetResultLabel( "方向错误！" );
        StopCoroutine("BlinkResult");
        StartCoroutine( BlinkResult( times ) );
    }

    //设置倒计时
    public void SetCountDown(int time)
    {
        if( CountDownLabel == null)
        {
            CountDownLabel = FindUIComponent<Text>( "CountDown" );
        }

        StartCoroutine( BeignCountDown( time ) );
    }

    //设置圈数UI
    private void SetTurnCountLabel()
    {
        TurnCountLabel.text = "已完成：" + m_challengeController.TurnCount.ToString() + "/" + m_challengeController.TurnLimit.ToString() + "圈";
    }

    //设置用时UI
    private void SetTimeCountLabel()
    {
        TimeCountLabel.text = "耗时：" + Convert.ToInt32( m_challengeController.TimeCount ).ToString() + "秒/" + m_challengeController.TimeLimit.ToString() + "秒";
    }

    //设置速度UI
    private void SetSpeedLabel()
    {
        SpeedLabel.text = "当前速度：" + Convert.ToInt32( m_carController.Velocity.magnitude * 3.6f).ToString() + "km/h" ;
    }

    //闪烁UI组件
    IEnumerator BlinkResult(int times)
    {
        for ( int i = 0; i < times; i++ )
        {
            ResultLabel.enabled = true;
            yield return new WaitForSeconds( 0.5f );    //每隔0.5秒闪烁一次
            ResultLabel.enabled = false;
            yield return new WaitForSeconds( 0.5f );
        }
    }
    
    //开始倒计时
    IEnumerator BeignCountDown(int time)
    {
        Debug.Log( "开始计时" );
        CountDownLabel.enabled = true;
        while ( time > 0 )
        {
            CountDownLabel.text = time.ToString();
            time--;
            yield return new WaitForSeconds( 1f );
        }

        CountDownLabel.text = "Go!";

        yield return new WaitForSeconds( 1f );

        CountDownLabel.enabled = false;
    }
}
