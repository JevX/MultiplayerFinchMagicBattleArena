using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtility{

    public static float Lerp(float Current, float Target, float DeltaTime, float InterpSpeed)
    {
        if (InterpSpeed <= 0.0f) return Target;
        float Alpha = Mathf.Clamp(DeltaTime * InterpSpeed, 0.0f, 1.0f);
        return Target * Alpha + Current * (1.0f - Alpha);
    }

    public static float DistancePointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        return Vector3.Magnitude(ProjectPointLine(point, lineStart, lineEnd) - point);
    }

    public static Vector3 ProjectPointLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 rhs = point - lineStart;
        Vector3 vector2 = lineEnd - lineStart;
        float magnitude = vector2.magnitude;
        Vector3 lhs = vector2;
        if (magnitude > 1E-06f)
        {
            lhs = (Vector3)(lhs / magnitude);
        }
        float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, magnitude);
        return (lineStart + ((Vector3)(lhs * num2)));
    }
}
