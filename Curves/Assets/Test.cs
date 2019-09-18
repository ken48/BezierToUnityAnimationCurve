using UnityEngine;

public enum ConversionDirection
{
    BezierToAnimationCurve,
    AnimationCurveToBezier,
}

public class Test : MonoBehaviour
{
    [SerializeField]
    ConversionDirection _conversionDirection;
    [SerializeField]
    Vector2 p0;
    [SerializeField]
    Vector2 p1;
    [SerializeField]
    Vector2 p2;
    [SerializeField]
    Vector2 p3;
    [SerializeField]
    AnimationCurve _curve;
}
