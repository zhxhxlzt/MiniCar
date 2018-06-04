using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 1, 接收用户输入
 * 2，提供赛车控制器指令输出
 * */
public class InputHandler : MonoBehaviour {

    public bool MoveInput = true;   //是否处理移动输入

    [SerializeField] private float accel = 0f;    //加减速，倒车;  数值范围[-1，1]
    [SerializeField] private float steer = 0f;    //转向;  数值范围[-1, 1]
    [SerializeField] private float brake = 0f;    //刹车;  数值范围[0, 1]

    //外部获取移动参数
    public float Accel { get { return accel; } }
    public float Steer { get { return steer; } }
    public float Brake { get { return brake; } }

    private void FixedUpdate()
    {
        HandleMoveInput();
    }

    private void HandleMoveInput()
    {
        //若为false， 不处理输入
        if ( !MoveInput )
        {
            accel = 0f;
            steer = 0f;
            brake = 1f;
            return;
        }

        accel = Input.GetAxis( "Vertical" );
        steer = Input.GetAxis( "Horizontal" );
        brake = Input.GetAxis( "Jump" );
    }
    
}
