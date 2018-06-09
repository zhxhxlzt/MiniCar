using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* 1, 接收用户输入
 * 2，提供赛车控制器指令输出
 * */
public class InputHandler : MonoBehaviour {

    public bool handleCarInput = true;   //是否处理移动输入
    public bool handleUIInput = true;     //是否处理UI输入

    [Header("=== 赛车数据 ===")]
    [SerializeField] private float accel = 0f;    //加减速，倒车;  数值范围[-1，1]
    [SerializeField] private float steer = 0f;    //转向;  数值范围[-1, 1]
    [SerializeField] private float brake = 0f;    //刹车;  数值范围[0, 1]

    [Header( "=== UI数据 ===" )]
    [SerializeField] private bool escape = false;   //取消命令


    //外部获取移动参数
    public float Accel { get { return accel; } }
    public float Steer { get { return steer; } }
    public float Brake { get { return brake; } }

    public bool Escape
    {
        get
        {
            bool returnValue = escape;

            escape = false;

            return returnValue;
        }
    }
    
    private void Update()
    {
        HandleMoveInput();
    }
    //处理输入
    private void HandleMoveInput()
    {
        //若为false，自动刹车，不处理输入
        if ( handleCarInput )
        {
            accel = Input.GetAxis( "Vertical" );
            steer = Input.GetAxis( "Horizontal" );
            brake = Input.GetAxis( "Jump" );
        }
        else
        {
            accel = 0;
            steer = 0;
            brake = 1;
        }

        if ( handleUIInput )
        {
            escape = Input.GetKeyUp( KeyCode.Escape );
        }
        else
        {
            escape = false;
        }
        
    }
}
