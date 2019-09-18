using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Test))]
public class EditorTest : Editor
{
    SerializedProperty conversionDirection;
    SerializedProperty p0;
    SerializedProperty p1;
    SerializedProperty p2;
    SerializedProperty p3;
    SerializedProperty curve;

    void OnEnable()
    {
        conversionDirection = serializedObject.FindProperty("_conversionDirection");
        p0 = serializedObject.FindProperty ("p0");
        p1 = serializedObject.FindProperty ("p1");
        p2 = serializedObject.FindProperty ("p2");
        p3 = serializedObject.FindProperty ("p3");
        curve = serializedObject.FindProperty ("_curve");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update ();

        if (conversionDirection.enumValueIndex == 0)
        {
            // Bezier -> AnimationCurve
            Vector2 start = p0.vector2Value;
            Vector2 finish = p3.vector2Value;

            void CalcCurveSlope(Vector2 p0, Vector2 p1, out float tan, out float weight)
            {
                Vector2 d = p1 - p0;
                float dxSign = Mathf.Sign(d.x);
                float dxAbs = dxSign * d.x;
                if (Mathf.Approximately(dxAbs, 0f))
                {
                    // Adding Mathf.Epsilon does not work
                    dxAbs += 0.000001f;
                    d.x = dxSign * dxAbs;
                }
                tan = d.y / d.x;
                weight = dxAbs;
            }

            CalcCurveSlope(p0.vector2Value, p1.vector2Value, out float outTan0, out float outWeight0);
            CalcCurveSlope(p2.vector2Value, p3.vector2Value, out float inTan1, out float inWeight1);

            var kf0 = new Keyframe(start.x, start.y, 0f, outTan0, 0f, outWeight0);
            var kf1 = new Keyframe(finish.x, finish.y, inTan1, 0, inWeight1, 0f);

            curve.animationCurveValue = new AnimationCurve(kf0, kf1);
        }
        else
        {
            // AnimationCurve -> Bezier
            if (curve.animationCurveValue.length == 0)
                curve.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);

            Keyframe kf0 = curve.animationCurveValue.keys[0];
            Keyframe kf1 = curve.animationCurveValue.keys[1];

            p0.vector2Value = new Vector2(kf0.time, kf0.value);

            float p1x = kf0.outWeight;
            p1.vector2Value = new Vector2(p1x, p1x * kf0.outTangent);

            float p2x = p3.vector2Value.x - kf1.inWeight;
            float p2y = p3.vector2Value.y - kf1.inWeight * kf1.inTangent;
            p2.vector2Value = new Vector2(p2x, p2y);

            p3.vector2Value = new Vector2(kf1.time, kf1.value);
        }

        EditorGUILayout.PropertyField(conversionDirection);
        EditorGUILayout.PropertyField(p0);
        EditorGUILayout.PropertyField(p1);
        EditorGUILayout.PropertyField(p2);
        EditorGUILayout.PropertyField(p3);
        EditorGUILayout.PropertyField(curve);

        serializedObject.ApplyModifiedProperties();
    }
}
