using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPoint : MonoBehaviour {

    [Header( "=== UI组件 ===" )]
    public Text Result;

    [Header("=== 当前检查点状态 ===")]
    [SerializeField]private bool passed = false;      //检查赛车是否通过

    public bool Passed { get { return passed; } }

    //警告错误方向
    private void AlertWrongDirection()
    {
        Result.text = "方向错误！";
        StartCoroutine( BlinkResult() );
    }

    //闪烁UI组件提示
    IEnumerator BlinkResult()
    {
        for( int i = 0; i < 5; i++ )
        {
            yield return new WaitForSeconds( 0.5f );    //每隔0.5秒闪烁一次
            Result.gameObject.SetActive( true );
            yield return new WaitForSeconds( 0.5f );
            Result.gameObject.SetActive( false );
        }
    }

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

        //只有当赛车通过时，才将当前检查点设为已通过
        if ( other.CompareTag( "Player" ) && CorrectDirection )
        {
            Debug.Log( "赛车已通过！" );
            passed = true;
        }
    }

    //若当前检查点为TurnFlag，则会在赛车离开此检查点后将passed置为false，
    //通过判断“是否通过全部检查点”可以确定行驶一圈后又回到了TurnFlag检查点
    private void OnTriggerExit(Collider other)
    {
        if( name == "TurnFlag" && other.CompareTag( "Player" ) )
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
