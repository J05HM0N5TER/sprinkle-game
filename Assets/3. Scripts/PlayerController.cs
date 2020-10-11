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
		SolderingIron = 1 << 5,
		Lantern = 1 << 6
	}

	private Rigidbody rb;
	[Range(1, 15)]
	[Tooltip("Speed the player moves")]
	public float speed = 10.0f;
	[Range(0.04f, 0.1f)]
	[Tooltip("How far the player can be from the ground and still jump")]
	public float groundDistance = 0.04f;
	[Range(150, 500)]
	[Tooltip("The amount of force that goes into jumping (Jump height)")]
	public float jumpForce = 300;

	[Header("Crouch variables")]
	[Range(0.1f, 1)]
	[Tooltip("Height of player when crouched")]
	public float crouchHeight = 0.5f;
	// The scale of the player at launch
	private Vector3 defaultScale;
	// The default height of the player
	private float standHeight;
	[Range(0.1f, 1)]
	[Tooltip("How long it takes to transition between crouching and standing (in seconds)")]
	public float crouchTransitionTime = 0.5f;
	// The collider for the player
	private CapsuleCollider capsule;
	// Is the player currently couching?
	private bool isCrouching = false;
	[Range(0, 10)]
	[Tooltip("The effect that crouching has on speed, this is a percentage impact (0.5 make it so crouching make the player half speed)")]
	public float crouchSpeed = 5f;
	[Range(50, 300)]
	[Tooltip("The amount of force that goes into jumping while crouching (Jump height for crouching)")]
	public float couchJumpForce = 150;

	[Header("Debug values")]
	[Tooltip("What the player has in their inventory")]
	public Inventory inventory;
	[Tooltip("How many Med-Syringes the player has in their inventory")]
	public int medSyringes = 0;
	[Tooltip("How many Battery Packs the player has in their inventory")]
	public int batteryPacks = 0;

	private bool inCrouchTransition;
	private DateTime crouchTransitionStart;

	//attacking player stuff
	public float health = 2;
	
	// Start is called before the first frame update
	void Start()
	{
		rb = gameObject.GetComponent<Rigidbody>();
		capsule = GetComponent<CapsuleCollider>();
		standHeight = capsule.height;
		defaultScale = transform.localScale;
	}

	private void FixedUpdate()
	{
		// Constant input
		Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		if (input.magnitude > 0.9f)
			input.Normalize();

		// Modify speed based on if the player is crouching
		float currentSpeed = isCrouching ? crouchSpeed : speed;

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

			rb.AddForce(gameObject.transform.up * (isCrouching ? couchJumpForce : jumpForce));
		}
		Crouch();
		if(health <= 0)
		{
			//GameObject.FindObjectOfType<GameManager>();
			GameObject.Find("PauseManager").GetComponent<PauseMenu>().PauseGame();
		}
	}

	/// <summary>
	/// Deals with all the logic to do with crouching
	/// </summary>
	void Crouch()
	{

		// Check if a transition needs to start
		if (!inCrouchTransition && Input.GetButtonDown("Crouch"))
		{
			isCrouching = !isCrouching;
			inCrouchTransition = true;
			crouchTransitionStart = DateTime.Now;
		}

		// If the player is standing then make them crouch otherwise make them stand
		if (inCrouchTransition && isCrouching)
		{
			float currentYScale = Mathf.Lerp(defaultScale.y, crouchHeight,
				(float) (DateTime.Now - crouchTransitionStart).TotalSeconds / crouchTransitionTime);

			isCrouching = true;
			// Change the localScale of the gameObject so that the height is the crouch height
			// gameObject.transform.localScale = new Vector3(transform.localScale.x,
			// 	defaultScale.y / (standHeight / crouchHeight), transform.localScale.z);
			Debug.Log($"{currentYScale} {currentYScale}");

			gameObject.transform.localScale = new Vector3(
				transform.localScale.x,
				currentYScale,
				transform.localScale.z
			);
			Vector3 pos = transform.position;
			pos.y -= ((standHeight - crouchHeight) / (crouchTransitionTime / Time.deltaTime) );
			transform.position = pos;
		}
		// Standing up
		else if (inCrouchTransition && !isCrouching)
		{
			// Check that there is nothing above them
			if (!Physics.Raycast(transform.position, Vector3.up,
					// Distance is how much difference between current height and stand height
					standHeight - (capsule.radius < crouchHeight ? crouchHeight : capsule.radius)))
			{
				float currentYScale = Mathf.Lerp(crouchHeight, defaultScale.y,
					(float) (DateTime.Now - crouchTransitionStart).TotalSeconds / crouchTransitionTime);
				// When the player stands up reset the scale back to what it was at the start
				// gameObject.transform.localScale = defaultScale;
				gameObject.transform.localScale = new Vector3(
					transform.localScale.x,
					currentYScale,
					transform.localScale.z
				);
			}
			else
			{
				isCrouching = true;
				inCrouchTransition = false;
			}
		}
		// float temp = transform.localScale.y;

		// Check if a transition needs to stop
		if (inCrouchTransition && (DateTime.Now - crouchTransitionStart).TotalSeconds > crouchTransitionTime)
		{
			Debug.Log($"Transion time is {(DateTime.Now - crouchTransitionStart).TotalSeconds} so stopping transition");
			inCrouchTransition = false;
			crouchTransitionStart = DateTime.Now;
		}

	}

	/// <summary>
	/// Checks if the player is standing on ground
	/// </summary>
	/// <returns>Is the player on the ground</returns>
	public bool IsStanding()
	{
		Ray ray = new Ray(transform.position, -transform.up);
		// For some reason when crouched the value for the distace to ground has to be a lot more
		float distance = ((capsule.height * transform.localScale.y) / 2) + (groundDistance * (isCrouching ? 5 : 1));
		return Physics.Raycast(ray, distance, ~LayerMask.GetMask("Player"));
	}
}
