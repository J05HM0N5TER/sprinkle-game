using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityMathExtensions
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

    /// <summary>
    /// Divide the components of one vector by the components of a second vector
    /// </summary>
    /// <param name="vector">The dividend in the equation</param>
    /// <param name="secondVector">The divisor in the equation</param>
    /// <returns>The result of the equation</returns>
    public static Vector3 ComponentDivide(this Vector3 vector, Vector3 secondVector)
    {
        vector = new Vector3(vector.x / secondVector.x, vector.y / secondVector.y, vector.z / secondVector.z);
        return vector;
    }

    public static Vector3 CopyFrom(this Vector3 vector, System.Numerics.Vector3 newValues)
    {
        vector = new Vector3(newValues.X, newValues.Y, newValues.Z);
        return vector;
    }
}

public static class NumericsExtensions
{
    /// <summary>
    /// Copies values for Unity Vector3 to .Net Vector3 for serialization
    /// </summary>
    /// <param name="vector">The current vector</param>
    /// <param name="newValues">The Vector3 holding the values to overwrite the current vector</param>
    /// <returns>The current value with its new values in it</returns>
    public static System.Numerics.Vector3 CopyFrom(this System.Numerics.Vector3 vector, Vector3 newValues)
    {
        vector = new System.Numerics.Vector3(newValues.x, newValues.y, newValues.z);
        return vector;
    }

    /// <summary>
    /// Copies values for Unity Quaternion to .Net Quaternion for serialization
    /// </summary>
    /// <param name="vector">The current Quaternion</param>
    /// <param name="newValues">The Quaternion holding the values to overwrite the current Quaternion</param>
    /// <returns>The current value with its new values in it</returns>
    public static System.Numerics.Quaternion CopyFrom(this System.Numerics.Quaternion quaternion, UnityEngine.Quaternion newValues)
    {
        quaternion = new System.Numerics.Quaternion(newValues.x, newValues.y, newValues.z, newValues.w);
        return quaternion; 
    }


    public static System.Numerics.Matrix4x4 CopyFrom(this System.Numerics.Matrix4x4 matrix4X4, UnityEngine.Matrix4x4 newValues)
    {
        // This was annoying to do
        matrix4X4 = new System.Numerics.Matrix4x4(
            newValues.m00, newValues.m01, newValues.m02, newValues.m03,
            newValues.m10, newValues.m11, newValues.m12, newValues.m13,
            newValues.m20, newValues.m21, newValues.m22, newValues.m23,
            newValues.m30, newValues.m31, newValues.m32, newValues.m33);
        return matrix4X4;
    }
}
