using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControl : MonoBehaviour
{
	private enum LeanState : byte
	{
		None,
		Left,
		Right
	}

	[Tooltip("Mouse Sensitivity")]
	public float mouseSen = 100f;
	[Tooltip("Player Object")]
	public Transform PlayerBody;
	private float Yrotation = 0f;
	// ray casting
	[Tooltip("layer of objects that can be picked up")]
	public LayerMask grabLayers;
	RaycastHit hit;

	[Header("Grab ability settings:")]
	[Range(0.1f, 10)]
	[Tooltip("How far away the object can be before it cant be grabbed")]
	public float grabDistance = 1;
	[Range(0.1f, 5)]
	[Tooltip("How far the object is held from the player once picked up")]
	public float holdDistance = 0.5f;
	private Transform heldObject = null;

	[Header("Lean settings")]
	[Tooltip("The angle that the camera will be tilted on when the player leans")]
	[Range(-90, 90)]
	public float leanTilt;
	[Tooltip("How much the camera will be moved by when the player leans")]
	[Range(-1, 1)]
	public float leanOffset;
	// The default position of camera
	private Vector3 defaultPos;
	private LeanState currentLean = LeanState.None;
	private LeanState previousLean = LeanState.None;
	// Array of the position to lerp to, uses the lean state for index
	private Vector3[] leanPos;
	private DateTime leanTransitionStartTime;
	[Tooltip("How long it takes to transition between lean states (in seconds)")]
	public float leanTransitionTime = 1f;
	private Vector3 leanTransitionStartPos;
	// The modifier to the rotation euler x at start of transition
	private float leanTransitionStartRotMod;
	// Array of modifiers to the rotation, uses the lean state for index
	private float[] leanRotMod;

	// Start is called before the first frame update
	void Start()
	{
		leanTransitionStartTime = DateTime.Now.AddSeconds(-leanTransitionTime);

		UnityEngine.Cursor.lockState = CursorLockMode.Locked;
		defaultPos = transform.localPosition;
		leanPos = new Vector3[]{
			defaultPos,
			new Vector3(
					defaultPos.x - leanOffset,
					defaultPos.y,
					defaultPos.z),
			new Vector3(
					defaultPos.x + leanOffset,
					defaultPos.y,
					defaultPos.z)
		};
		leanRotMod = new float[]
		{
			0f,
			-leanTilt,
			leanTilt
		};
	}

	// Update is called once per frame
	void Update()
	{
		// Get input
		float mouseX = Input.GetAxis("Mouse X") * mouseSen * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSen * Time.deltaTime;

		// Vertical mouse movement goes on camera transform
		Yrotation -= mouseY;
		Yrotation = Mathf.Clamp(Yrotation, -90f, 90f);
		transform.localRotation = Quaternion.Euler(Yrotation, 0, 0);

		// Horizontal mouse movement goes on player body transform
		PlayerBody.Rotate(Vector3.up * mouseX);

		// Picking up objects
		if (!heldObject && Input.GetButtonDown("Interact") &&
			Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, grabDistance, grabLayers))
		{
			print("Grabbed " + hit.collider.name);
			heldObject = hit.collider.transform;
			Rigidbody rigidbody = heldObject.GetComponent<Rigidbody>();
			if (rigidbody)
			{
				heldObject.position = transform.position + (transform.forward * holdDistance);
				heldObject.parent = transform;
				rigidbody.isKinematic = true;
			}
			else
			{
				Debug.LogError(hit.collider.name + " was grabbed and has no rigidbody");
				heldObject = null;
			}
		}
		// Dropping held object
		else if (heldObject && Input.GetButtonUp("Interact"))
		{
			print("Dropped " + heldObject.name);
			heldObject.parent = null;
			heldObject.GetComponent<Rigidbody>().isKinematic = false;
			heldObject = null;
		}

		Lean();
	}

	/// <summary>
	/// Moves the camera when the player leans
	/// </summary>
	private void Lean()
	{

		//Lean
		bool isLeaningLeft = Input.GetButton("Lean Left");
		bool isLeaningRight = Input.GetButton("Lean Right");

		// Used to see if the target lean state changed this frame
		LeanState leanStateAtStartOfFrame = currentLean;

		// If both directions or none are pressed
		if ((isLeaningLeft && isLeaningRight) || (!isLeaningLeft && !isLeaningRight))
		{
			currentLean = LeanState.None;
		}
		else if (isLeaningLeft)
		{
			currentLean = LeanState.Left;
		}
		else if (isLeaningRight)
		{
			currentLean = LeanState.Right;
		}

		if (currentLean != leanStateAtStartOfFrame)
		{
			Debug.Log($"Current lean changed from {leanStateAtStartOfFrame} to {currentLean}", this);
			// What was the rotation modifier when the lean state changed? 
			// If the transition was complete
			if (leanStateAtStartOfFrame == previousLean)
			{
				leanTransitionStartRotMod = leanRotMod[(int)previousLean];
			}
			// If the player changed mid transition
			else
			{
				float transitionPercent = (float)(DateTime.Now - leanTransitionStartTime).TotalSeconds / leanTransitionTime;
				leanTransitionStartRotMod = Mathf.Lerp(
					leanRotMod[(int)leanStateAtStartOfFrame],
					leanRotMod[(int)currentLean],
					transitionPercent);
			}
			leanTransitionStartPos = transform.localPosition;
			leanTransitionStartTime = DateTime.Now;
		}

		if (Vector3.Distance(transform.localPosition, leanTransitionStartPos) > 0.05f || previousLean != currentLean)
		{
			// Position
			//Debug.Log($"Transitioning between {previousLean} and {currentLean}", this);
			float transitionPercent = (float)(DateTime.Now - leanTransitionStartTime).TotalSeconds / leanTransitionTime;
			transform.localPosition = Vector3.Lerp(
				leanTransitionStartPos, // The state transitioning from
				leanPos[(int)currentLean], // The state transitioning to
				transitionPercent); // How far though the transition it is

			if (transitionPercent > 0.5f && transitionPercent < 1)
			{
				float a = 5;
			}

			//// Rotation
			//Quaternion targetRot = Quaternion.Euler(
			//			transform.localRotation.eulerAngles.x,
			//			transform.localRotation.eulerAngles.y,
			//			transform.localRotation.eulerAngles.z + leanRotMod[(int)currentLean]);

			//Quaternion startTransitionRot = Quaternion.Euler(
			//			transform.localRotation.eulerAngles.x,
			//			transform.localRotation.eulerAngles.y,
			//			transform.localRotation.eulerAngles.z + leanTransitionStartRotMod);

			////transform.localRotation = targetRot;
			//transform.localRotation = Quaternion.Slerp(startTransitionRot, targetRot, transitionPercent);

			float currentRotMod = Mathf.Lerp(leanTransitionStartRotMod, leanRotMod[(int)currentLean], transitionPercent);
			transform.localRotation = Quaternion.Euler(
						transform.localRotation.eulerAngles.x,
						transform.localRotation.eulerAngles.y,
						transform.localRotation.eulerAngles.z + currentRotMod);

			if (transitionPercent >= 1f)
			{
				previousLean = currentLean;
			}
		}
		else
		{
			transform.localPosition = leanPos[(int)currentLean];
		}
		//Debug.Log("Toggles lean", this);

	}
}
