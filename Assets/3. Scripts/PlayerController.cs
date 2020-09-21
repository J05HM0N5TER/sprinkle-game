using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
	[Flags] public enum Inventory : byte
	{
		none = 0,
		Astrogeology = 1 << 0,
		Astrobotany = 1 << 1,
		AstroMicrobiology = 1 << 2,
		EscapePodPasscard = 1 << 3,
		ChemicalSpray = 1 << 4,
		SolderingIron = 1 << 5
	}


	private Rigidbody rb;
	[Tooltip("Speed the player moves")]
	public float speed = 10.0f;
	[Tooltip("How far the player can be from the ground and still jump")]
	public float groundDistance = 0.4f;
	[Tooltip("Height jump")]
	public float jumpForce = 300;

	[Header("Crouch variables")]
	[Tooltip("Height of player when crouched")]
	public float crouchHeight = 0.5f;
	// The scale of the player at launch
	private Vector3 defautScale;
	// The default height of the player
	private float standHeight;
	// The collider for the player
	private CapsuleCollider capsule;
	// Is the player currently couching?
	private bool isCrouching = false;
	[Tooltip("The effect that crouching has on speed, this is a percentage impact (0.5 make it so crouching make the player half speed)")]
	public float crouchSpeedModifier = 0.5f;


	[Header("Debug values")]
	[Tooltip("What the player has in their inventory")]
	public Inventory inventory;
	[Tooltip("How many Med-Syringes the player has in their inventory")]
	public int medSyringes = 0;
	[Tooltip("How many Battery Packs the player has in their inventory")]
	public int batteryPacks = 0;

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
		// Constant input
		Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		if (input.magnitude > 0.9f)
			input.Normalize();

		// Modify speed based on if the player is crouching
		float currentSpeed = isCrouching ? speed * crouchSpeedModifier : speed;

		// Move the player using the input, keep the downwards velocity for when they fall.
		Vector3 vel = new Vector3(0, rb.velocity.y, 0);
		vel += gameObject.transform.forward * input.z * currentSpeed;
		vel += gameObject.transform.right * input.x * currentSpeed;
		rb.velocity = vel;
	}

	// Update is called once per frame
	void Update()
	{
		// Impulse input
		if (Input.GetButtonDown("Jump") && IsStanding())
		{
			Debug.Log("Jump pressed", this);
			rb.AddForce(gameObject.transform.up * jumpForce);
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
			isCrouching = true;
			// Change the localScale of the gameObject so that the height is the crouch height
			gameObject.transform.localScale = new Vector3(transform.localScale.x,
				defautScale.y / (standHeight / crouchHeight), transform.localScale.z);
			Vector3 pos = transform.position;
			pos.y -= ((standHeight - crouchHeight) / 2) /*+ 0.01f*/;
			transform.position = pos;

		}
		else if (!Physics.Raycast(transform.position, Vector3.up,
			// Distance is how much difference between current height and stand height
			standHeight - (capsule.radius < crouchHeight ? crouchHeight : capsule.radius)))
		{
			isCrouching = false;
			// When the player stands up reset the scale back to what it was at the start
			gameObject.transform.localScale = defautScale;
		}

		// If the player is holding something, make sure that the scale is not effected
		//if (cameraControl.heldObject)
		//{
		//	cameraControl.heldObject.localScale = cameraControl.heldObject.localScale.ComponentDevide(cameraControl.heldObject.lossyScale);
		//}
		//Debug.Log($"Held objects transform is {heldObject.localScale} and lossyscale is {heldObject.lossyScale}", heldObject);
	}

	/// <summary>
	/// Checks if the player is standing on ground
	/// </summary>
	/// <returns>Is the player on the ground</returns>
	public bool IsStanding()
	{
		Ray ray = new Ray(transform.position, -transform.up);
		return Physics.Raycast(ray, ((capsule.height * transform.localScale.y) / 2) + groundDistance, ~LayerMask.GetMask("Player"));
	}
}
