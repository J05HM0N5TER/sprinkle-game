using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class mouselook : MonoBehaviour
{
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

	// Start is called before the first frame update
	void Start()
	{
		UnityEngine.Cursor.lockState = CursorLockMode.Locked;
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
	}
}
