using System;
using UnityEngine;

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
	private float YRotation = 0f;
	// ray casting
	[Tooltip("layer of objects that can be picked up")]
	public LayerMask grabLayers;

	[Header("Grab ability settings:")]
	[Range(0.1f, 10)]
	[Tooltip("How far away the object can be before it cant be grabbed")]
	public float grabDistance = 1;
	[Range(0.1f, 5)]
	[Tooltip("How far the object is held from the player once picked up")]
	public float holdDistance = 0.5f;
	// The object that is in the players hand (null if player isn't holding anything)
	[HideInInspector] public Rigidbody heldObject = null;
	// The position on the screen where it detects click at (decimal percentage)
	private Vector2 cursorPosition = new Vector2(0.5f, 0.5f);
	[Tooltip("The name of the game object that is the reticle that the player can see")]
	public string reticleName = "Reticle";
	// The info from the reticle used to calculate where to click
	private RectTransform reticle;
	[Tooltip("The amount of force put into the object held when thrown")]
	[Range(0, 5000)]
	public float throwForce = 5f;

	[Header("Lean settings")]
	[Tooltip("The angle that the camera will be tilted on when the player leans")]
	[Range(-90, 90)]
	public float leanTilt;
	[Tooltip("How much the camera will be moved by when the player leans")]
	[Range(-1, 1)]
	public float leanOffset;
	// The default position of camera
	private Vector3 defaultPos;
	// What is the current lean (eather the one that it is on or the one that is being transitioned to)
	private LeanState currentLean = LeanState.None;
	// The previous state of lean
	private LeanState previousLean = LeanState.None;
	// Array of the position to lerp to, uses the lean state for index
	private Vector3[] leanPos;
	private DateTime leanTransitionStartTime;
	[Tooltip("How long it takes to transition between lean states (in seconds)")]
	public float leanTransitionTime = 1f;
	// What the position of the camera is when a new transition starts
	private Vector3 leanTransitionStartPos;
	// The modifier to the rotation euler x at start of transition
	private float leanTransitionStartRotMod;
	// Array of modifiers to the rotation, uses the lean state for index
	private float[] leanRotMod;
	[Tooltip("How fast the player has to be moving before they can no longer lean")]
	public float maxSpeedWhileLeaning = 0.25f;

	private Camera PlayerCamera;

	[Header("Spring variables")]
	[Tooltip("How \"Snappy\" the holding is")]
	public float springStrength = 50;
	[Tooltip("How much the dragging slows down when it is the correct position")]
	public float damper = 5;
	public float minDistance = 0;
	public float maxDistance = 0.2f;
	[Tooltip("The amount of drag applied to the held object")]
	public float rigidBodyDrag = 5;
	[Tooltip("The amount of angular drag applied to the held object")]
	public float rigidBodyDragAngular = 5;
	// The saved variables from the held object, to be applied when dropped
	float normalDrag = 0;
	float normalADrag = 0.05f;

	[Header("Debug")]
	[Tooltip("The current magnitude of the velocity of the player")]
#pragma warning disable IDE0052 // Remove unread private members
	[SerializeField] private float playerMagnitude = 0;
#pragma warning restore IDE0052 // Remove unread private members
	// The spring that controls the held object
	private SpringJoint grabSpring;

	private PlayerController player;
	private Rigidbody playerRigidbody;

	public GameObject torch;
	private bool torchActive = false;

	// Start is called before the first frame update
	void Start()
	{
		leanTransitionStartTime = DateTime.Now.AddSeconds(-leanTransitionTime);

		Cursor.lockState = CursorLockMode.Locked;
		defaultPos = transform.localPosition;
		leanPos = new Vector3[]
		{
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
			0f, -leanTilt,
			leanTilt
		};

		PlayerCamera = GetComponent<Camera>();
		player = gameObject.transform.parent.GetComponent<PlayerController>();
		playerRigidbody = player.GetComponent<Rigidbody>();

		reticle = GameObject.Find(reticleName).GetComponent<RectTransform>();
		cursorPosition = new Vector2(reticle.position.x / Screen.width, reticle.position.y / Screen.height);

		// Check that everything was retrieved successfully
#if UNITY_EDITOR
		if (PlayerCamera == null)
		{
			Debug.LogWarning("Can't find Camera", this);
		}
		if (player == null)
		{
			Debug.LogWarning("Can't find PlayerController", this);
		}
		if (playerRigidbody == null)
		{
			Debug.LogWarning("Cant find player RigidBody", this);
		}
#endif
		torch.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		// Get input
		float mouseX = Input.GetAxis("Mouse X") * mouseSen * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSen * Time.deltaTime;

		// Vertical mouse movement goes on camera transform
		YRotation -= mouseY;
		YRotation = Mathf.Clamp(YRotation, -90f, 90f);
		transform.localRotation = Quaternion.Euler(YRotation, 0, 0);

		// Horizontal mouse movement goes on player body transform
		PlayerBody.Rotate(Vector3.up * mouseX);

		// Picking up objects
		if (!heldObject && Input.GetButtonDown("Interact"))
		{
			GrabObject();
		}
		// Dropping held object
		else if (heldObject && Input.GetButtonUp("Interact"))
		{
			DropObject();
		}
		// Throwing the object
		else if (heldObject && Input.GetButtonDown("Throw object"))
		{
			ThrowObject();
		}

		if (heldObject != null)
		{
			// Only in editor update reticle position every frame
#if UNITY_EDITOR
			cursorPosition = new Vector2(reticle.position.x / Screen.width, reticle.position.y / Screen.height);
#endif
			// Adjust the held object spring to in front of the player
			grabSpring.connectedAnchor = PlayerCamera.ViewportToWorldPoint(new Vector3(cursorPosition.x, cursorPosition.y, holdDistance));

			heldObject.rotation = Quaternion.Euler(
				heldObject.rotation.eulerAngles.x, 
				heldObject.rotation.eulerAngles.y + mouseX, 
				heldObject.rotation.eulerAngles.z);
		}
		if (Input.GetButtonDown("Torch") && !torchActive)
		{
			torch.SetActive(true);
			torchActive = true;
		}
		if (Input.GetButtonDown("Torch") && torchActive)
		{
			torch.SetActive(false);
			torchActive = false;
		}
		Lean();
	}

	/// <summary>
	/// Grabs the object in front of the player
	/// </summary>
	public void GrabObject()
	{
		if (Physics.Raycast(PlayerCamera.ViewportPointToRay(new Vector3(cursorPosition.x, cursorPosition.y, 0.5f)), out RaycastHit RayOut, grabDistance, grabLayers))
		{
			if (RayOut.rigidbody != null)
			{
				//RayOut.collider.isTrigger = false;
				heldObject = RayOut.rigidbody;
				// Creates a spring to hold the object by
				grabSpring = RayOut.transform.gameObject.AddComponent<SpringJoint>();
				// Turn off the auto configuration, this is so you can specify a position instead of a game object
				grabSpring.autoConfigureConnectedAnchor = false;
				// Set the variables that has been set manually
				grabSpring.spring = springStrength;
				grabSpring.damper = damper;
				grabSpring.minDistance = minDistance;
				grabSpring.maxDistance = maxDistance;
				// Set the anchor to nothing, this will be updated with correct position later
				grabSpring.anchor = Vector3.zero;

				normalDrag = RayOut.rigidbody.drag;
				normalADrag = RayOut.rigidbody.angularDrag;
				heldObject.drag = rigidBodyDrag;
				heldObject.angularDrag = rigidBodyDragAngular;

				// Stop the object to rotating
				heldObject.constraints = heldObject.constraints | RigidbodyConstraints.FreezeRotation;

				//heldObject.gameObject.transform.position = PlayerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, holdDistance));
			}
		}
	}

	/// <summary>
	/// Drops the currently held object
	/// </summary>
	public void DropObject()
	{
		heldObject.WakeUp();

		// Reset values saved from before
		heldObject.drag = normalDrag;
		heldObject.angularDrag = normalADrag;

		// Allow the object to rotate
		heldObject.constraints = heldObject.constraints ^ RigidbodyConstraints.FreezeRotation;

		// Destroy the spring
		Destroy(grabSpring);

		// Set it to not holding anything
		heldObject = null;
	}

	public void ThrowObject()
	{
		// Copy the object becuase DropObject removes it from heldobject
		Rigidbody throwObject = heldObject;
		DropObject();
		// Add the throw force to the object
		throwObject.AddForce(transform.forward * throwForce);
	}

	/// <summary>
	/// Moves the camera when the player leans
	/// </summary>
	private void Lean()
	{
		//Lean Input
		bool isLeaningLeft = Input.GetButton("Lean Left");
		bool isLeaningRight = Input.GetButton("Lean Right");

		// Used to see if the target lean state changed this frame
		LeanState leanStateAtStartOfFrame = currentLean;

		playerMagnitude = playerRigidbody.velocity.magnitude;

		// NOTE: At peak of jump velocity is 0 so for a quick second it allows them to lean
		// If the player is moving or jumping
		if (playerRigidbody.velocity.magnitude > maxSpeedWhileLeaning)
		{
			currentLean = LeanState.None;
		}
		// If both directions or none are pressed
		else if (isLeaningLeft && isLeaningRight)
		{
			// Don't do anything
		}
		// If none are being pressed
		else if (!isLeaningLeft && !isLeaningRight)
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

		// If the target lean changed this frame
		if (currentLean != leanStateAtStartOfFrame)
		{
			//Debug.Log($"Current lean changed from {leanStateAtStartOfFrame} to {currentLean}", this);
			// What was the rotation modifier when the lean state changed? 
			// If the transition was complete
			if (leanStateAtStartOfFrame == previousLean)
			{
				leanTransitionStartRotMod = leanRotMod[(int) previousLean];
			}
			// If the player changed mid transition
			else
			{
				float transitionPercent = (float) (DateTime.Now - leanTransitionStartTime).TotalSeconds / leanTransitionTime;
				leanTransitionStartRotMod = Mathf.Lerp(
					leanTransitionStartRotMod,
					leanRotMod[(int) currentLean],
					transitionPercent);
			}
			leanTransitionStartPos = transform.localPosition;
			leanTransitionStartTime = DateTime.Now;
		}

		bool leanPosBlocked = Physics.Linecast(transform.TransformPoint(transform.localPosition),
			transform.TransformPoint(leanPos[(int) currentLean]), ~LayerMask.GetMask("Player"));

		// If a transition is needed
		if (!leanPosBlocked && (Vector3.Distance(transform.localPosition, leanTransitionStartPos) > 0.05f || previousLean != currentLean))
		{
			// Position
			//Debug.Log($"Transitioning between {previousLean} and {currentLean}", this);
			float transitionPercent = (float) (DateTime.Now - leanTransitionStartTime).TotalSeconds / leanTransitionTime;
			transform.localPosition = Vector3.Lerp(
				leanTransitionStartPos, // The state transitioning from
				leanPos[(int) currentLean], // The state transitioning to
				transitionPercent); // How far though the transition it is

			// Rotation
			float currentRotMod = Mathf.Lerp(leanTransitionStartRotMod, leanRotMod[(int) currentLean], transitionPercent);
			transform.localRotation = Quaternion.Euler(
				transform.localRotation.eulerAngles.x,
				transform.localRotation.eulerAngles.y,
				transform.localRotation.eulerAngles.z + currentRotMod);

			if (transitionPercent >= 1f)
			{
				previousLean = currentLean;
			}
		}
		// If a transition is not needed
		else if (!leanPosBlocked)
		{
			// Position
			transform.localPosition = leanPos[(int) currentLean];
			// Rotation
			transform.localRotation = Quaternion.Euler(
				transform.localRotation.eulerAngles.x,
				transform.localRotation.eulerAngles.y,
				transform.localRotation.eulerAngles.z + leanRotMod[(int) currentLean]);
		}
	}
}
