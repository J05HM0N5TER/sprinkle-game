using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;
using GameSerialization;

public class LurkerAi : MonoBehaviour, IXmlSerializable
{
	//public List<GameObject> lurkerPoints = new List<GameObject>();
	private GameObject[] lurkerPoints;
	[Tooltip("Player Object")]
	public GameObject player;
	private GameObject currentLurkingPoint; // point that the AI is currently at
	private GameObject closestLurkerPoint; // the closest point to the player that the AI is not at
	[Tooltip("Time that the AI stays visible when looked at until it disappears/moves")]
	public float spookyTimer = 0.5f;
	float resettimer;
	[Tooltip("Camera attached to player")]
	public Camera playerCam;
	[Tooltip("Time until the AI moves from spot without being seen as player hasn't looked at it in a while")]
	public float unseenTimer = 20.0f;
	private float resetUnseenTimer;
	[Tooltip("The chance of the ai appearing, 1 = 100%, 0 = 0%")]
	public float chanceOfAppearing = 1.0f;
	private bool appearsNearPlayer = false;
	[Tooltip("The place that the lurker goes when it is not supposed to be seen")]
	public GameObject lurkerWaitingPoint;
	// Start is called before the first frame update
	void Start()
	{
		lurkerPoints = GameObject.FindGameObjectsWithTag("Respawn");
		foreach (GameObject Lurkerpoint in lurkerPoints)
		{
			//print("Lurker point: " + Lurkerpoint.transform.position);
		}
		resetUnseenTimer = unseenTimer;
		resettimer = spookyTimer;
	}

	// Update is called once per frame
	void Update()
	{
		unseenTimer -= Time.deltaTime;
		// Is the lurker being seen?
		if (IsVisableToPlayer(transform.position) || unseenTimer <= 0.0f)// gameObject.GetComponent<Renderer>().isVisible)
		{
			unseenTimer = resetUnseenTimer;
			if (Random.value <= chanceOfAppearing)
			{
				appearsNearPlayer = true;
			}
			else
			{
				appearsNearPlayer = false;
			}
			
			print("Is seen, trying to find new point...");
			if (appearsNearPlayer)
			{
				foreach (GameObject Lurkerpoint in lurkerPoints)
				{
					// Is the spawn point being seen?
					if (!IsVisableToPlayer(Lurkerpoint.transform.position))
					{
						// Find closest valid spawn point
						float distance = UnityEngine.Vector3.Distance(player.GetComponent<Transform>().position, Lurkerpoint.GetComponent<Transform>().position);
						// If their is no points then set this as the current one
						if (closestLurkerPoint == null)
						{
							closestLurkerPoint = Lurkerpoint;
						}
						// If the point is not the current point
						else if (transform != currentLurkingPoint)
						{
							closestLurkerPoint = Lurkerpoint;
						}
					}
					unseenTimer = resetUnseenTimer;
				}
			}
			if(!appearsNearPlayer)
			{
				// FIXME: Check that there is a closestLurkerPoint set before modifying
				closestLurkerPoint.transform.position = lurkerWaitingPoint.transform.position;
			}
			//actually moving the bloody thing
			if (closestLurkerPoint != null)
			{
				//print("Closest point is " + closestLurkerPoint.name);
				spookyTimer -= Time.deltaTime;
				if (spookyTimer <= 0.0f)
				{
					gameObject.GetComponent<Transform>().position = closestLurkerPoint.transform.position;
					gameObject.GetComponent<Transform>().rotation = closestLurkerPoint.transform.rotation;
					currentLurkingPoint = closestLurkerPoint;
					spookyTimer = resettimer;
				}
			}
			else
			{
				Debug.LogError("Couldn't find valid point");
			}
		}
	}

	private bool IsVisableToPlayer(UnityEngine.Vector3 position)
	{
		// lurker in player view
		UnityEngine.Vector3 screenPoint = playerCam.WorldToViewportPoint(position);
		// Is the player within the view bounds
		bool InScreenBounds = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
		// Is the player within screen bounds and nothing is obstructing view
		bool rayObstructed = Physics.Linecast(/*startPos, endPos,*/ position, player.transform.position,
			out RaycastHit hitinfo, ~((1 << 9) | (1 << 10))); // ignore layer 9 and 10
															  // Print out what the ray hit
		//if (rayObstructed)
		//    print("Ray hit: " + hitinfo.collider.name + " at: " + hitinfo.point.x + ", " + hitinfo.point.y);
		// Debug view
		return InScreenBounds && !rayObstructed;
	}

	public XmlSchema GetSchema()
	{
		return (null);
	}

	public void WriteXml(XmlWriter writer)
	{
		writer.WriteStartElement(nameof(currentLurkingPoint));
		// Write the ID of the currentLurkerPoint if there is one, if not then write
		// 0 so that on read it knows that there was nothing set
		writer.WriteValue(currentLurkingPoint?.transform.GetInstanceID() ?? 0);
		writer.WriteEndElement();
		writer.WriteStartElement(nameof(spookyTimer));
		writer.WriteValue(spookyTimer);
		writer.WriteEndElement();
		writer.WriteStartElement(nameof(resettimer));
		writer.WriteValue(resettimer);
		writer.WriteEndElement();
		writer.WriteStartElement(nameof(unseenTimer));
		writer.WriteValue(unseenTimer);
		writer.WriteEndElement();
	}

	public void ReadXml(XmlReader reader)
	{
		SerializationController controller = FindObjectOfType<SerializationController>();
		
		currentLurkingPoint = controller.InstanceIDToTransform(reader.ReadElementContentAsInt())?.gameObject;
		spookyTimer = reader.ReadElementContentAsFloat();
		resettimer = reader.ReadElementContentAsFloat();
		unseenTimer = reader.ReadElementContentAsFloat();
	}
}
