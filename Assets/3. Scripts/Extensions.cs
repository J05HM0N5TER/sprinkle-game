using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnityMathExtensions
{
	public static Quaternion ExtractRotation(this UnityEngine.Matrix4x4 matrix)
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

	public static Vector3 ExtractPosition(this UnityEngine.Matrix4x4 matrix)
	{
		Vector3 position;
		position.x = matrix.m03;
		position.y = matrix.m13;
		position.z = matrix.m23;
		return position;
	}

	public static Vector3 ExtractScale(this UnityEngine.Matrix4x4 matrix)
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
	public static Vector3 ComponentDivide(this UnityEngine.Vector3 vector, UnityEngine.Vector3 secondVector)
	{
		vector = new Vector3(vector.x / secondVector.x, vector.y / secondVector.y, vector.z / secondVector.z);
		return vector;
	}

}

public static class Convert
{
	public static UnityEngine.Transform Copy(GameSave.ObjectData from, UnityEngine.Transform to)
	{
		to.name = from.name;
		to.position = Convert.Copy(from.position, to.position);
		to.rotation = Convert.Copy(from.rotation, to.rotation);
		to.localScale = Convert.Copy(from.localScale, to.localScale);
		return to;
	}
	public static GameSave.ObjectData Copy(UnityEngine.Transform from, GameSave.ObjectData to)
	{
		to.name = from.name;
		to.position = Convert.Copy(from.position, to.position);
		to.rotation = Convert.Copy(from.rotation, to.rotation);
		to.localScale = Convert.Copy(from.localScale, to.localScale);
		return to;
	}
	public static GameSave.ObjectData New(UnityEngine.Transform from)
	{
		GameSave.ObjectData newObjectData = new GameSave.ObjectData();
		Copy(from, newObjectData);
		return newObjectData;
	}

	public static Vector3 Copy(System.Numerics.Vector3 from, UnityEngine.Vector3 to)
	{
		to.x = from.X;
		to.y = from.Y;
		to.z = from.Z;
		return to;
	}

	public static Vector3 New(System.Numerics.Vector3 from)
	{
		UnityEngine.Vector3 to = new Vector3();
		Copy(from, to);
		return to;
	}

	/// <summary>
	/// Copies values for Unity Vector3 to .Net Vector3 for serialization
	/// </summary>
	/// <param name="to">The current vector</param>
	/// <param name="from">The Vector3 holding the values to overwrite the current vector</param>
	/// <returns>The current value with its new values in it</returns>
	public static System.Numerics.Vector3 Copy(UnityEngine.Vector3 from, System.Numerics.Vector3 to)
	{
		to.X = from.x;
		to.Y = from.y;
		to.Z = from.z;
		return to;
	}

	public static System.Numerics.Vector3 New(UnityEngine.Vector3 from)
	{
		System.Numerics.Vector3 to = new System.Numerics.Vector3();
		Copy(from, to);
		return to;
	}

	/// <summary>
	/// Copies values for Unity Quaternion to .Net Quaternion for serialization
	/// </summary>
	/// <param name="vector">The current Quaternion</param>
	/// <param name="from">The Quaternion holding the values to overwrite the current Quaternion</param>
	/// <returns>The current value with its new values in it</returns>
	public static System.Numerics.Quaternion Copy(UnityEngine.Quaternion from, System.Numerics.Quaternion to)
	{
		to.X = from.x;
		to.Y = from.y;
		to.Z = from.z;
		return to;
	}
	public static System.Numerics.Quaternion New(UnityEngine.Quaternion from)
	{
		System.Numerics.Quaternion to = new System.Numerics.Quaternion();
		Copy(from, to);
		return to;
	}

	public static UnityEngine.Quaternion Copy(System.Numerics.Quaternion from, UnityEngine.Quaternion to)
	{
		to.x = from.X;
		to.y = from.Y;
		to.z = from.Z;
		return to;
	}

	public static UnityEngine.Quaternion New(System.Numerics.Quaternion from)
	{
		UnityEngine.Quaternion to = new UnityEngine.Quaternion();
		Copy(from, to);
		return to;
	}

	public static System.Numerics.Matrix4x4 Copy(UnityEngine.Matrix4x4 from, System.Numerics.Matrix4x4 to)
	{
		to.M11 = from.m00;
		to.M12 = from.m01;
		to.M13 = from.m02;
		to.M14 = from.m03;
		to.M21 = from.m10;
		to.M22 = from.m11;
		to.M23 = from.m12;
		to.M24 = from.m13;
		to.M31 = from.m20;
		to.M32 = from.m21;
		to.M33 = from.m22;
		to.M34 = from.m23;
		to.M41 = from.m30;
		to.M42 = from.m31;
		to.M43 = from.m32;
		to.M44 = from.m33;
		return to;
	}
}
