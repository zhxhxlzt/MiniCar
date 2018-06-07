using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelHud : MonoBehaviour {

    [Header( "=== 外部控制器 ===" )]
    [SerializeField] private CarController m_carCtrl;               //赛车控制器
    [SerializeField] private ChallengeController m_challengeCtrl;   //闯关控制器

    [Header( "=== UI组件 ===" )]
    [SerializeField] private Text CountDownLabel;    //倒计时Text
    [SerializeField] private Text TimeCountLabel;    //计时Text
    [SerializeField] private Text TurnCountLabel;    //计圈Text
    [SerializeField] private Text SpeedLabel;        //速度Text
    [SerializeField] private Text ResultLabel;       //结果Text

    private void Start()
    {
        m_carCtrl = FindObjectOfType<CarController>();
        m_challengeCtrl = FindObjectOfType<ChallengeController>();

        FindHUD();
    }

    private void OnGUI()
    {
        ShowHUD();
    }

    //查找UI组件
    private void FindHUD()
    {
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
    private void ShowHUD()
    {
        SetTurnCountLabel();
        SetTimeCountLabel();
        SetSpeedLabel();
    }

    //设置结果信息
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
        TurnCountLabel.text = "已完成：" + m_challengeCtrl.TurnCount.ToString() + "/" + m_challengeCtrl.TurnLimit.ToString() + "圈";
    }

    //设置用时UI
    private void SetTimeCountLabel()
    {
        TimeCountLabel.text = "耗时：" + Convert.ToInt32( m_challengeCtrl.TimeCount ).ToString() + "秒/" + m_challengeCtrl.TimeLimit.ToString() + "秒";
    }

    //设置速度UI
    private void SetSpeedLabel()
    {
        SpeedLabel.text = "当前速度：" + Convert.ToInt32( m_carCtrl.Velocity.magnitude * 3.6f).ToString() + "km/h" ;
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
}
