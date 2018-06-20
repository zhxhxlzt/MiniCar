using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 检查点
/// 1.检测赛车是否通过此检查点
/// 2.方向错误时调用HUD警告方向错误
/// </summary>
public class CheckPoint : MonoBehaviour {

    [Header( "=== 外部控制器 ===" )]
    [SerializeField] private LevelHud m_levelHud;   //关卡HUD

    [Header( "=== 当前检查点状态 ===" )]
    [SerializeField] private string identifyTag = "CarCollider";
    [SerializeField] private bool passed = false;      //检查赛车是否通过

    //是否通过当前关卡
    public bool Passed { get { return passed; } }

    //初始化外部控制器
    private void Start()
    {
        m_levelHud = FindObjectOfType<LevelHud>();
    }

    //警告错误方向
    private void AlertWrongDirection()
    {
        m_levelHud.SetWrongDirection( 3 );
    }
    
    //当赛车进入trigger时执行
    private void OnTriggerEnter(Collider other)
    {
        
        //只有速度方向与跑道方向一致，才能认为通过检查点
        bool CorrectDirection = Vector3.Dot( other.GetComponentInParent<Rigidbody>().velocity, transform.forward ) > 0;

        if( !CorrectDirection )
        {
            AlertWrongDirection();
        }

        //若当前检查点已通过，则直接返回
        if ( passed )
        {
            return;
        }

        //只有当赛车正确通过时，才将当前检查点设为已通过
        if ( other.CompareTag( identifyTag ) && CorrectDirection )
        {
            Debug.Log( "赛车已通过！" );
            passed = true;
        }
    }

    //若当前检查点为TurnFlag，则会在赛车离开此检查点后将passed置为false，
    //通过判断“是否通过全部检查点”可以确定行驶一圈后又回到了TurnFlag检查点
    private void OnTriggerExit(Collider other)
    {
        if( name == "TurnFlag" && other.CompareTag( identifyTag ) )
        {
            passed = false;
            Debug.Log( "赛车已离开TurnFlag!" );
        }
    }

    //公有重设方法
    public void ResetPassState()
    {
        passed = false;     
    }

    private void Reset()
    {
        if(GetComponent<BoxCollider>() == null)
        {
            Debug.Log( name + "没有box collider!" );
        }

        GetComponent<BoxCollider>().isTrigger = true;
    }
}
