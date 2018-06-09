using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 摩擦曲线
/// </summary>
[System.Serializable]
public struct Friction
{
    public float ExtremumSlip;
    public float ExtremumValue;
    public float AsymptoteSlip;
    public float AsymptoteValue;
    public float Stiffness;

    public void SetData(WheelFrictionCurve frictionCurve)
    {
        ExtremumSlip = frictionCurve.extremumSlip;
        ExtremumValue = frictionCurve.extremumValue;
        AsymptoteSlip = frictionCurve.asymptoteSlip;
        AsymptoteValue = frictionCurve.asymptoteValue;
        //Stiffness = frictionCurve.stiffness;
    }

    public WheelFrictionCurve GetData()
    {
        return new WheelFrictionCurve
        {
            extremumSlip = ExtremumSlip,
            extremumValue = ExtremumValue,
            asymptoteSlip = AsymptoteSlip,
            asymptoteValue = AsymptoteValue,
        };
    }
}

[CreateAssetMenu(fileName = "WheelColliderAdjust")]
public class WheelColliderAdjust : ScriptableObject {
    public Friction ForwardFriction;
    public Friction SidewaysFriction;
}
