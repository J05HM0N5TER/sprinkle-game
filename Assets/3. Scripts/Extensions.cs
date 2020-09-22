using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatrixExtensions
{
    public static Quaternion ExtractRotation(this Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    public static Vector3 ExtractPosition(this Matrix4x4 matrix)
    {
        Vector3 position;
        position.x = matrix.m03;
        position.y = matrix.m13;
        position.z = matrix.m23;
        return position;
    }

    public static Vector3 ExtractScale(this Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }
}

public static class Vector3Extensions
{
    /// <summary>
    /// Divide the components of one vector by the components of a second vector
    /// </summary>
    /// <param name="vector">The dividend in the equation</param>
    /// <param name="secondVector">The divisor in the equation</param>
    /// <returns>The result of the equation</returns>
    public static Vector3 ComponentDivide(this Vector3 vector, Vector3 secondVector)
    {
        return new Vector3(vector.x / secondVector.x, vector.y / secondVector.y, vector.z / secondVector.z);
    }

    public static Vector3 OverWrite(this Vector3 vector, System.Numerics.Vector3 newValues)
    {
        return new Vector3(newValues.X, newValues.Y, newValues.Z);
    }
}

public static class NumericsExtensions
{
    /// <summary>
    /// Copies values for Unity Vector3 to .Net Vector3 for serialization
    /// </summary>
    /// <param name="vector">The current vector</param>
    /// <param name="newValues">The Vector3 holding the values to overwrite the current vector</param>
    /// <returns></returns>
    public static System.Numerics.Vector3 OverWrite(this System.Numerics.Vector3 vector, Vector3 newValues)
    {
        return new System.Numerics.Vector3(newValues.x, newValues.y, newValues.z);
    }
    // public static 
}
