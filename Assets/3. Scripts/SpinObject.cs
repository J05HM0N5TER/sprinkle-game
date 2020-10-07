using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
	// Start is called before the first frame update
	[Tooltip("How much should it rotate by each second per axis")]
	public Vector3 spinAmount = new Vector3(0, 0, 1);

	// Update is called once per frame
	private void FixedUpdate()
	{
		Vector3 rot = transform.rotation.eulerAngles;

		// Get the amount that it should spin this frame
		Vector3 spinThisFrame = spinAmount * Time.deltaTime;
		// Modify the rotation by the amount that it should be rotating
		rot += spinThisFrame;
		// Assign now rotation
		transform.rotation = Quaternion.Euler(rot);
	}
}
