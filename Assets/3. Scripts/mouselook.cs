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
	private float Xrotation = 0f;
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

	[Header("Lean variables")]
	[Tooltip("The angle that the camera will be tilted on when the player leans")]
	public float leanTilt;
	[Tooltip("How much the camera will be moved by when the player leans")]
	public Vector3 leanOffset;
	// The default position of camera
	private Vector3 defaultCameraPos;
	private Quaternion defaultCameraRot;

	// Start is called before the first frame update
	void Start()
	{
		UnityEngine.Cursor.lockState = CursorLockMode.Locked;
		defaultCameraPos = camera.transform.localPosition;
		defaultCameraRot = camera.transform.localRotation;
	}

	// Update is called once per frame
	void Update()
	{
		float mousex = Input.GetAxis("Mouse X") * mouseSen * Time.deltaTime;
		float mousey = Input.GetAxis("Mouse Y") * mouseSen * Time.deltaTime;

		Xrotation -= mousey;
		Xrotation = Mathf.Clamp(Xrotation, -90f, 90f);
		transform.localRotation = Quaternion.Euler(Xrotation, 0, 0);
		PlayerBody.Rotate(Vector3.up * mousex);

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
		if (Input.GetButtonDown("Lean Right"))
		{
			ToggleLean(LeanState.Right);
		}
		if (Input.GetButtonDown("Lean Left"))
		{
			ToggleLean(LeanState.Left);
		}
		if (Input.GetButtonUp("Lean Left") || Input.GetButtonUp("Lean Right"))
		{
			ToggleLean(LeanState.None);
		}
	}

	private void ToggleLean(LeanState leanState)
	{
		Quaternion newLocalRot = transform.rotation;
		Debug.Log("Toggles lean", this);
		switch (leanState)
		{
			case LeanState.None:
				transform.localPosition = defaultCameraPos;
				break;
			case LeanState.Left:
				this.transform.localPosition = defaultCameraPos - leanOffset;
				newLocalRot.eulerAngles = new Vector3(newLocalRot.x + leanTilt, newLocalRot.y + leanTilt, newLocalRot.z + leanTilt);
				break;
			case LeanState.Right:
				this.transform.localPosition = defaultCameraPos + leanOffset;
				//newLocalRot.eulerAngles = new Vector3(newLocalRot.x - leanTilt, newLocalRot.y - leanTilt, newLocalRot.z - leanTilt);
				newLocalRot.SetLookRotation(new Vector3(0, 0, leanTilt));
				break;
			default:
				Debug.Log("Invalid lean state", this);
				break;
		}

		this.transform.localRotation = newLocalRot;
	}
}
