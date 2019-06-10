using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CStates/Parameters")]
public class CStateParamtetersSO : ScriptableObject
{
    public CStateParameters idleP;
    public CStateParameters walkP;
    public CStateParameters jumpsquatP;
    public CStateParameters jumpingP;
    public CStateParameters landingP;
    public CStateParameters hitFreezeP;
    public CStateParameters hitStunP;
    public CStateParameters hitLandP;
    public CStateParameters hitLandedP;
    public CStateParameters standUpP;
    public CStateParameters shieldP;
    public CStateParameters dieP;
    public CStateParameters deadP;
}

[System.Serializable]
public class CStateParameters
{
    public string animationName = "idle";
    public int duration = 0;

    public List<Parameter> parameters;
}

[System.Serializable]
public class Parameter
{
    public string statName;
    public float value;
    public string stringValue;

    public float Value { get { return value; } }
    public int IntValue { get { return (int)value; } }
    public bool BoolValue { get { return value == 1; } }
    public string StringValue { get { return stringValue; } }
}