using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	public float JumpHeight = 15;

	// Crouch variables
	[Tooltip("Height of player when crouched")]
	public float crouchHeight;
	// The scale of the player at launch
	private Vector3 defautScale;
	// The default height of the player
	private float standHeight;
	// The collider for the player
	private CapsuleCollider capsule;

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

		// Move the player using the input, keep the downwards velocity for when they fall.
		Vector3 vel = new Vector3(0, rb.velocity.y, 0);
		vel += gameObject.transform.forward * input.z * speed;
		vel += gameObject.transform.right * input.x * speed;
		rb.velocity = vel;
	}

	// Update is called once per frame
	void Update()
	{
		// Impulse input
		if (Input.GetButton("Jump") && isStanding())
		{
			rb.AddForce(gameObject.transform.up * JumpHeight);
		}
		if (Input.GetButtonDown("Crouch"))
		{
			ToggleCrouch();
		}

		inventory = Inventory.Astrobotany | Inventory.Astrogeology;

		Inventory check = Inventory.Astrobotany | Inventory.Astrogeology;
		if ((inventory | check ) == check)
		{

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
