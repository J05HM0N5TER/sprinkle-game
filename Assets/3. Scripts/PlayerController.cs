using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	
	private Rigidbody rb;
	[Tooltip("Layer of the ground")]
	public LayerMask groundMask;
	[Tooltip("Speed the player moves")]
	public float speed = 10.0f;
	[Tooltip("How far the player can be from the ground and still jump")]
	public float groundDistance = 0.4f;
	[Tooltip("Height jump")]
	public float JumpHeight = 15;
	[Tooltip("Height of player when crouched")]
	public float crouchHeight;
	private Vector3 defautScale;
	private float standHeight;
	private CapsuleCollider capsule;

	// FIXME: This is dumb, need to work out the math to do in code later
	private Transform moveTransform;

	// Start is called before the first frame update
	void Start()
	{
		rb = gameObject.GetComponent<Rigidbody>();
		capsule = GetComponent<CapsuleCollider>();
		standHeight = capsule.height;
		defautScale = transform.localScale;
	}

	private void FixedUpdate()
	{
		// Movement input
		Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		//rb.AddForce(gameObject.transform.forward * input.y * speed);
		//rb.AddForce(gameObject.transform.right * input.x * speed);

		//Vector3 forwardInput = input.z * transform.forward;
		//Vector3 rightInput = input.x * transform.right;
		//rb.velocity = forwardInput + rightInput;
		//rb.velocity = input.x * transform.forward;

		//Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward * input.x);
		//Vector3 localForward = transform.InverseTransformPoint(input);
		//Vector3 localForward = Transform.Translate(input, Space.Self);

		//float velocityInDirection = Vector3.Dot(rigidbody.velocity, direction);

		Vector3 localForward = transform.InverseTransformVector(input);

		// Working from https://answers.unity.com/questions/518399/simulate-child-parent-relationship.html
		// FIXME: This is not working at all.

		Quaternion rotation = new Quaternion();
		rotation = transform.rotation;
		rotation *= transform.rotation;

		Matrix4x4 test = new Matrix4x4();
		//test.
		Vector3 vel = rotation.eulerAngles;
		vel /= 360;
		rb.velocity = vel;
		//Transform dumbFix = new Transform();

		Matrix4x4 child = new Matrix4x4();
		child.SetTRS(input, Quaternion.identity, Vector3.one);

		Matrix4x4 parentMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

		transform.position = parentMatrix.MultiplyPoint3x4(child.ExtractPosition());

		transform.rotation = (transform.rotation * Quaternion.Inverse(transform.rotation)) * mouse.gameObject.transform.rotation;



		//transform.Translate(new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, 0, Input.GetAxis("Vertical") * speed * Time.deltaTime));

	}

	// Update is called once per frame
	void Update()
	{
		// Movement input

		//rb.velocity = Vector3.Project(input, transform.forward);

		if (Input.GetButton("Jump") && isStanding())
		{
			rb.AddForce(gameObject.transform.up * JumpHeight);
		}
		if (Input.GetButtonDown("Crouch"))
		{
			ToggleCrouch();
		}
	}

	/// <summary>
	/// Toggles between the player standing and crouching
	/// </summary>
	void ToggleCrouch()
	{
		// If the player is standing then make them crouch otherwise make them stand
		if (transform.localScale == defautScale)
		{
			// Change the localScale of the gameObject so that the height is the crouch height
			gameObject.transform.localScale = new Vector3(transform.localScale.x,
				defautScale.y / (standHeight / crouchHeight), transform.localScale.z);

		}
		else if (!Physics.Raycast(transform.position, Vector3.up,
			// Distance is how much difference between current height and stand height
			standHeight - (capsule.radius < crouchHeight ? crouchHeight : capsule.radius)))
		{
			// When the player stands up reset the scale back to what it was at the start
			gameObject.transform.localScale = defautScale;
		}
	}

	/// <summary>
	/// Checks if the player is standing on ground
	/// </summary>
	/// <returns>Is the player on the ground</returns>
	bool isStanding()
	{
		Ray ray = new Ray(transform.position, -transform.up);
		return Physics.Raycast(ray, (capsule.height / 2) + groundDistance);
	}
}
