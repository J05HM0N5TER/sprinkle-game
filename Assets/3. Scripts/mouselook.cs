using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Mouselook : MonoBehaviour
{
	private enum LeanState : byte
	{
		None = 0,
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

	// Start is called before the first frame update
	void Start()
	{
		UnityEngine.Cursor.lockState = CursorLockMode.Locked;
		defaultPos = transform.localPosition;
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


		//Lean
		bool isLeaningLeft = Input.GetButton("Lean Left");
		bool isLeaningRight = Input.GetButton("Lean Right");

		// If both directions or none are pressed
		if ((isLeaningLeft && isLeaningRight) || !(isLeaningLeft || isLeaningRight))
		{
			ToggleLean(LeanState.None);
		}
		else if (isLeaningLeft)
		{ 
			ToggleLean(LeanState.Left);
		}
		else if (isLeaningRight)
		{
			ToggleLean(LeanState.Right);
		}
	}

	/// <summary>
	/// Moves the camera when the player leans
	/// </summary>
	/// <param name="leanState">The current lean of the player (left, right or none)</param>
	private void ToggleLean(LeanState leanState)
	{
		Debug.Log("Toggles lean", this);
		switch (leanState)
		{
			case LeanState.None:
				transform.localPosition = defaultPos;
				break;
			case LeanState.Left:
				// Set position of camera
				this.transform.localPosition = new Vector3(
					defaultPos.x - leanOffset,
					defaultPos.y,
					defaultPos.z);

				// Set rotation of camera
				transform.localRotation = Quaternion.Euler(
					transform.localRotation.eulerAngles.x , 
					transform.localRotation.eulerAngles.y, 
					transform.localRotation.eulerAngles.z - leanTilt);
				break;
			case LeanState.Right:
				// Set position of camera
				this.transform.localPosition = new Vector3(
					defaultPos.x + leanOffset,
					defaultPos.y,
					defaultPos.z);

				// Set rotation of camera
				transform.localRotation = Quaternion.Euler(
					transform.localRotation.eulerAngles.x, 
					transform.localRotation.eulerAngles.y, 
					transform.localRotation.eulerAngles.z + leanTilt);
				break;
			default:
				Debug.Log("Invalid lean state", this);
				break;
		}
	}
}
