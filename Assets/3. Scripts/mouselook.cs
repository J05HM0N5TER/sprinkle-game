using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class mouselook : MonoBehaviour
{
	public float mouseSen = 100f;

	public Transform PlayerBody;
	private float Xrotation = 0f;
	// ray casting
	public LayerMask grabLayers;
	RaycastHit hit;

	[Header("Grab ability settings:")]
	[Range(0.1f, 10)]
	public float grabDistance = 1;
	[Range(0.1f, 5)]
	public float holdDistance = 0.5f;
	private Transform heldObject = null;
	[Header("Controls")]
	MouseButton grabButton = MouseButton.LeftMouse;

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

		// ray casting
		if (!heldObject && Input.GetMouseButtonDown((int)grabButton) &&
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
		else if (heldObject && Input.GetMouseButtonUp((int)grabButton))
		{
			print("Dropped " + heldObject.name);
            heldObject.parent = null;
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
            heldObject = null;
        }
	}
}
