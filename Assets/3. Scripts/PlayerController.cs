using System;
using System.Collections;
using System.Collections.Generic;
// Start of for serialization
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
// End of for serialization
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IXmlSerializable
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
	private Vector3 defaultScale;
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
	public UInt16 medSyringes = 0;
	[Tooltip("How many Battery Packs the player has in their inventory")]
	public UInt16 batteryPacks = 0;

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
		if (transform.localScale == defaultScale)
		{
			isCrouching = true;
			// Change the localScale of the gameObject so that the height is the crouch height
			gameObject.transform.localScale = new Vector3(transform.localScale.x,
				defaultScale.y / (standHeight / crouchHeight), transform.localScale.z);
			Vector3 pos = transform.position;
			pos.y -= ((standHeight - crouchHeight) / 2) /*+ 0.01f*/ ;
			transform.position = pos;

		}
		else if (!Physics.Raycast(transform.position, Vector3.up,
				// Distance is how much difference between current height and stand height
				standHeight - (capsule.radius < crouchHeight ? crouchHeight : capsule.radius)))
		{
			isCrouching = false;
			// When the player stands up reset the scale back to what it was at the start
			gameObject.transform.localScale = defaultScale;
		}

		// If the player is holding something, make sure that the scale is not effected
		//if (cameraControl.heldObject)
		//{
		//	cameraControl.heldObject.localScale = cameraControl.heldObject.localScale.ComponentDivide(cameraControl.heldObject.lossyScale);
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

	// Stuff below is for serialization
	private PlayerController()
	{

	}

	// Xml Serialization Infrastructure

	public void WriteXml(XmlWriter writer)
	{
		XmlSerializer vector3Writer = new XmlSerializer(typeof(System.Numerics.Vector3));

		writer.WriteStartElement(nameof(speed));
		writer.WriteValue(speed);
		writer.WriteEndElement();

		writer.WriteStartElement(nameof(groundDistance));
		writer.WriteValue(groundDistance);
		writer.WriteEndElement();

		writer.WriteStartElement(nameof(jumpForce));
		writer.WriteValue(jumpForce);
		writer.WriteEndElement();

		writer.WriteStartElement(nameof(crouchHeight));
		writer.WriteValue(crouchHeight);
		writer.WriteEndElement();

		writer.WriteStartElement(nameof(defaultScale));
		vector3Writer.Serialize(writer, Convert.New(defaultScale));
		writer.WriteEndElement();

		writer.WriteStartElement(nameof(standHeight));
		writer.WriteValue(standHeight);
		writer.WriteEndElement();

		writer.WriteStartElement(nameof(isCrouching));
		writer.WriteValue(isCrouching);
		writer.WriteEndElement();

		writer.WriteStartElement(nameof(crouchSpeedModifier));
		writer.WriteValue(crouchSpeedModifier);
		writer.WriteEndElement();

		writer.WriteStartElement(nameof(inventory));
		writer.WriteValue( /* (int) */ inventory.ToString());
		writer.WriteEndElement();

		writer.WriteStartElement(nameof(medSyringes));
		writer.WriteValue(medSyringes);
		writer.WriteEndElement();

		writer.WriteStartElement(nameof(batteryPacks));
		writer.WriteValue(batteryPacks);
		writer.WriteEndElement();
	}

	public void ReadXml(XmlReader reader)
	{
		XmlSerializer vector3Reader = new XmlSerializer(typeof(System.Numerics.Vector3));
		// reader.ReadStartElement();
		speed = reader.ReadElementContentAsFloat();
		// reader.ReadStartElement();
		groundDistance = reader.ReadElementContentAsFloat();
		jumpForce = reader.ReadElementContentAsFloat();
		crouchHeight = reader.ReadElementContentAsFloat();
		reader.ReadStartElement();
		Convert.Copy((System.Numerics.Vector3) vector3Reader.Deserialize(reader), defaultScale);
		reader.ReadEndElement();
		// defaultScale.CopyFrom((System.Numerics.Vector3)reader.ReadElementContentAs(typeof(System.Numerics.Vector3), null));
		standHeight = reader.ReadElementContentAsFloat();
		isCrouching = reader.ReadElementContentAsBoolean();
		crouchSpeedModifier = reader.ReadElementContentAsFloat();
		// inventory = (Inventory)reader.ReadElementContentAs(typeof(Inventory),
		// null);
		// reader.ReadStartElement();
		inventory = (Inventory) Enum.Parse(typeof(Inventory), reader.ReadElementContentAsString());
		// inventory = (Inventory)reader.ReadElementContentAsInt();
		medSyringes = (ushort) reader.ReadElementContentAsInt();
		batteryPacks = (ushort) reader.ReadElementContentAsInt();
	}

	public XmlSchema GetSchema()
	{
		return (null);
	}
}
