using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelAdjust : MonoBehaviour {

    public WheelColliderAdjust AdjustFront;
    public WheelColliderAdjust AdjustRear;

    public WheelCollider front;
    public WheelCollider rear;

    public bool StoreData;

    private void Start()
    {
        InitCurve();
    }

    private void FixedUpdate()
    {
        if(StoreData)
        {
            Store();
            StoreData = false;
        }
    }

    private void Store()
    {
        AdjustFront.ForwardFriction.SetData( front.forwardFriction );
        AdjustFront.SidewaysFriction.SetData( front.sidewaysFriction );

        AdjustRear.ForwardFriction.SetData( rear.forwardFriction );
        AdjustRear.SidewaysFriction.SetData( rear.sidewaysFriction );
    }

    private void InitCurve()
    {
        front.forwardFriction = AdjustFront.ForwardFriction.GetData();
        front.sidewaysFriction = AdjustFront.SidewaysFriction.GetData();

        rear.forwardFriction = AdjustRear.ForwardFriction.GetData();
        rear.sidewaysFriction = AdjustRear.SidewaysFriction.GetData();
    }
}
