using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 车轮参数调整
/// </summary>
public class WheelAdjust : MonoBehaviour {

    public WheelColliderAdjust AdjustFront;
    public WheelColliderAdjust AdjustRear;

    public WheelCollider adjustFront;
    public WheelCollider adjustRear;

    public WheelCollider[] target;

    public GameObject[] wheels;
    public Text[] values;
    
    private void Start()
    {
        InitCurve();
    }

    private void FixedUpdate()
    {
        Store();
        Apply();
        ApplyDisplay();
    }

    private void Store()
    {
        AdjustFront.ForwardFriction.SetData( adjustFront.forwardFriction );
        AdjustFront.SidewaysFriction.SetData( adjustFront.sidewaysFriction );

        AdjustRear.ForwardFriction.SetData( adjustRear.forwardFriction );
        AdjustRear.SidewaysFriction.SetData( adjustRear.sidewaysFriction );
    }

    private void Apply()
    {
        for(int i = 0; i < 2; i++ )
        {
            target[i].forwardFriction = adjustFront.forwardFriction;
            target[i].sidewaysFriction = adjustFront.sidewaysFriction;
        }
            
        for(int i = 2; i < 4; i++ )
        {
            target[i].forwardFriction = adjustRear.forwardFriction;
            target[i].sidewaysFriction = adjustRear.sidewaysFriction;
        }
    }

    private void ApplyDisplay()
    {
        for( int i = 0; i < 4; i++ )
        {
            DisplayValue( wheels[i], target[i] );
        }
    }

    private void DisplayValue( GameObject wheel, WheelCollider wc )
    {
        WheelHit hit;
        wc.GetGroundHit( out hit );
        
        values = wheel.GetComponentsInChildren<Text>();
        values[0].text = hit.forwardSlip.ToString();
        values[1].text = hit.sidewaysSlip.ToString();
    }

    private void InitCurve()
    {
        adjustFront.forwardFriction = AdjustFront.ForwardFriction.GetData();
        adjustFront.sidewaysFriction = AdjustFront.SidewaysFriction.GetData();

        adjustRear.forwardFriction = AdjustRear.ForwardFriction.GetData();
        adjustRear.sidewaysFriction = AdjustRear.SidewaysFriction.GetData();
    }
}
