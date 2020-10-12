using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GameSerialization;
// End of for serialization

public class LivingArmourAI : MonoBehaviour, IXmlSerializable
{
	private NavMeshAgent agent;
	// The agents camera to see if the player is in the direct view
	[Tooltip ("The Camera of the AI")]
	public Camera DirectCam;
	[Tooltip ("The player Object")]
	private GameObject player;
	[Tooltip ("The max distance the ai will wonder around from its current point, decrease for ai to not move to other rooms as much")]
	public float wonderDistance = 10.0f;
	[Tooltip ("How long will the ai be searching in the area that it last saw the player")]
	public float timer = 10.0f;
	// Stores what the timer was when the game started
	private float resettimer;

	public float normalWalkSpeed = 10;
	public float chaseSpeed = 20;
	public float searchSpeed = 5;

	[Tooltip ("the area around the last seen point of the player that the ai will search for the player")]
	public float lookingDistance = 1.0f;
	public float idleLookTime = 1.0f;
	private float resetIdleLookTime;
	// Was the AI previously following the player?
	private bool wasFollowingPlayer = false;
	private bool isPlayerVisible = false;
	// Has the player ever been seen by this AI?

	// Is the ray between the player and AI colliding with something
	private bool rayObstructed = true;
	// Is the player in the view area for the AI
	private bool playerInScreenBounds = false;

	//hearing and sound stuff
	//public GameObject[] soundSources;
	public List<Vector3> soundSources = new List<Vector3> ();
	[Tooltip ("The detection range of hearing for the AI")]
	public float maxHearingRange = 5;
	private bool lookingforplayer = false;
	private float originalWonder;

	//living suit jumping stuff
	private GameObject[] suits;
	public float maxDistanceFromPlayer = 50;
	private GameObject closestSuit;
	private GameObject currentSuit;
	private Camera playerCam;
	private bool canSwapSuitAgain = true;
	public float swapTimer = 2;

	//visor colour stuff
	public GameObject visorLight;
	public Material visorEmission;
	private Light lightvisor;
	public Color chase = new Color (84, 31, 81, 1);
	public Color investigate = new Color (161, 100, 16, 1);
	public Color search = new Color (66, 94, 68, 1);
	private Vector3 playerLastSeen = Vector3.zero;
	[Header("attacking player values")]
	//attacking player stuff
	public float attackDistance = 3;
	public float attackCoolDown = 1;
	private float attackcooldownreset;
	public bool canAttackAgain = true;

	private Animator anim;

	// Start is called before the first frame update
	void Start ()
	{
		agent = gameObject.GetComponent<NavMeshAgent> ();
		originalWonder = wonderDistance;
		suits = GameObject.FindGameObjectsWithTag ("Suit");
		//visorLight = GetComponent<Light>();
		lightvisor = visorLight.GetComponent<Light> ();
		lightvisor.color = search;
		resettimer = timer;
		agent.speed = normalWalkSpeed;

		visorEmission.SetColor ("_EmissiveColor", search);
		visorEmission.EnableKeyword ("_EMISSION");
		//visorEmission.color = search;
		agent.autoBraking = true;
		agent.acceleration = 20;

		playerCam = Camera.main;
		attackcooldownreset = attackCoolDown;
		anim = gameObject.GetComponentInChildren<Animator>();
		player = GameObject.FindGameObjectWithTag("Player");

		resetIdleLookTime = idleLookTime;
	}

	// Update is called once per frame  
	void Update ()
	{
		// Player position on the AI camera view
		Vector3 screenPoint = DirectCam.WorldToViewportPoint (player.GetComponent<Transform> ().position);
		// Is the player within the view bounds
		playerInScreenBounds = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
		// The position the player was last seen at by the AI (Updated when the player is discovered)
		// Is the player within screen bounds and nothing is obstructing view
		rayObstructed = Physics.Linecast ( /*startPos, endPos,*/ agent.transform.position, player.transform.position, out RaycastHit hitinfo, ~(1 << 10));
		// Print out what the ray hit
		//if (rayObstructed)
		//	print("Ray hit: " + hitinfo.collider.name + " at: " + hitinfo.point.x + ", " + hitinfo.point.y);
		// Debug view
		isPlayerVisible = playerInScreenBounds && !rayObstructed;
		// If the player is currently seen
		if (isPlayerVisible)
		{
			playerLastSeen = player.transform.position;
			// Set the AI to go towards the player
			agent.SetDestination (playerLastSeen);
			lightvisor.color = chase;
			wasFollowingPlayer = true;
			agent.speed = chaseSpeed;
			visorEmission.SetColor ("_EmissiveColor", chase);
			visorEmission.EnableKeyword ("_EMISSION");
			
			
		}
		// TODO: Check that the player can't be seen, this it for when the AI catches the player
		if ((agent.transform.position - playerLastSeen).magnitude < 0.5f && !isPlayerVisible)
		{
			// Debug.Log("Changing to look for player", this);
			lookingforplayer = true;
			wasFollowingPlayer = false;
		}
		else if((agent.transform.position - playerLastSeen).magnitude < attackDistance && isPlayerVisible && canAttackAgain)
		{
			player.GetComponent<PlayerController>().health -= 1;
			canAttackAgain = false;
			//attackcooldown();
		}
		if(!canAttackAgain)
		{
			attackCoolDown -= Time.deltaTime;
			if(attackCoolDown <= 0)
			{
				canAttackAgain = true;
				attackCoolDown = attackcooldownreset;
			}
		}
		if (!agent.hasPath || agent.path == null)
		{
			// for debug purpose this does work
			agent.SetDestination (RandomNavSphere (agent.GetComponent<Transform> ().position, wonderDistance, -1));
		}
		// Searching around previous known position
		if (lookingforplayer)
		{
			// Debug.LogError("Looking for player", this);
			// if((agent.remainingDistance <= 0.5) &&  (idleLookTime > 0))
			// {
			// 	idleLookTime -= Time.deltaTime;
			// 	agent.isStopped = true;

			// }
			// else
			//{
				wonderDistance = lookingDistance;
				//LookForPlayer();
				lightvisor.color = investigate;
				timer -= Time.deltaTime;
				agent.speed = searchSpeed;
				visorEmission.SetColor ("_EmissiveColor", investigate);
				visorEmission.EnableKeyword ("_EMISSION");
				// if(idleLookTime <= 0)
				// {
				// 	idleLookTime = resetIdleLookTime;
				// }
				// agent.isStopped = false;
				// agent.SetDestination (RandomNavSphere (agent.GetComponent<Transform> ().position, wonderDistance, -1));
			//}
			//timer -= Time.deltaTime;
			
		}
		// reseting wonder / looking for player further
		if (timer <= 0 && !isPlayerVisible)
		{
			// Debug.Log("Timer Up", this);

			lookingforplayer = false;
			wonderDistance = originalWonder;
			lightvisor.color = search;
			timer = resettimer;
			agent.speed = normalWalkSpeed;
			visorEmission.SetColor ("_EmissiveColor", search);
			visorEmission.EnableKeyword ("_EMISSION");
		}
		// sound reactions
		if (!wasFollowingPlayer || !isPlayerVisible)
		{
			foreach (Vector3 SoundSource in soundSources)
			{
				if (Vector3.Distance (gameObject.transform.position, SoundSource) <= maxHearingRange)
				{
					agent.SetDestination (SoundSource);
					lookingforplayer = true;
					//lightvisor.color = investigate;
					// visorEmission.SetColor ("_EmissiveColor", investigate);
					// visorEmission.EnableKeyword ("_EMISSION");
				}
				if ((agent.transform.position - SoundSource).magnitude < 0.5f)
				{
					agent.SetDestination (RandomNavSphere (agent.GetComponent<Transform> ().position, wonderDistance, -1));
				}
			}
		}

		//TODO: add in information sharing

		print ("Is seen, trying to find new point...");
		if (((gameObject.transform.position - player.transform.position).magnitude > maxDistanceFromPlayer) && !isPlayerVisible)
		{
			suits = GameObject.FindGameObjectsWithTag ("Suit");
			currentSuit = gameObject;
			foreach (GameObject suit in suits)
			{
				// Is the spawn point being seen?
				if (!IsVisableToPlayer (suit.transform.position))
				{
					// Find closest valid spawn point
					float distance = UnityEngine.Vector3.Distance (player.GetComponent<Transform> ().position, suit.GetComponent<Transform> ().position);

					// If their is no points then set this as the current one
					if (closestSuit == null && closestSuit != currentSuit)
					{
						closestSuit = suit;
					}
					// If the point is not the current point
					else if ((transform != currentSuit) && (distance < (closestSuit.transform.position - player.transform.position).magnitude) )
					{
						closestSuit = suit;
					}
				}
			}
			//! : check if this is still bugged, if range is too small then the suit will turn itself off, if bug is persistant add timer maybe.
			if ((closestSuit != currentSuit) && canSwapSuitAgain)
			{
				print("changing suit " + closestSuit.name);
				closestSuit.GetComponent<LivingArmourAI> ().enabled = true;
				currentSuit = closestSuit;
				this.enabled = false;
				canSwapSuitAgain = false;
				SwapSuit();
			}
			else
			{
				Debug.LogError ("Couldn't find valid point");
			}
		}
		if(agent.velocity.magnitude >= 0.1f)
		{
			anim.SetBool("walking", true);
		}
	}
	public static Vector3 RandomNavSphere (Vector3 origin, float distance, int layermask)
	{
		Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

		randomDirection += origin;

		NavMesh.SamplePosition (randomDirection, out NavMeshHit navHit, distance, layermask);

		return navHit.position;
	}
	private void OnDrawGizmos ()
	{
		if (agent)
		{
			if (rayObstructed)
				Gizmos.color = Color.red;
			else
				Gizmos.color = Color.blue;
			Gizmos.DrawLine (agent.transform.position, player.transform.position);

			Gizmos.DrawWireSphere (agent.transform.position, wonderDistance);
		}
	}
	private void ifSoundInRange ()
	{
		Physics.OverlapSphere (gameObject.transform.position, 30);
	}
	private IEnumerator SwapSuit ()
	{
		yield return StartCoroutine ("SwappingSuitTimer");
	}
	private IEnumerator SwappingSuitTimer ()
	{
		// 
		// wonderDistance = originalWonder;
		// lightvisor.color = search;
		yield return new WaitForSeconds (swapTimer);
		canSwapSuitAgain = true;
		
	}
	private bool IsVisableToPlayer (UnityEngine.Vector3 position)
	{
		// lurker in player view
		UnityEngine.Vector3 screenPoint = playerCam.WorldToViewportPoint (position);
		// Is the player within the view bounds
		bool InScreenBounds = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
		// Is the player within screen bounds and nothing is obstructing view
		bool rayObstructed = Physics.Linecast ( /*startPos, endPos,*/ position, player.transform.position,
			out RaycastHit hitinfo, ~((1 << 9) | (1 << 10))); // ignore layer 9 and 10
		// Print out what the ray hit
		//if (rayObstructed)
		//    print("Ray hit: " + hitinfo.collider.name + " at: " + hitinfo.point.x + ", " + hitinfo.point.y);
		// Debug view
		return InScreenBounds && !rayObstructed;
	}
	//attacking player cooldown
	private IEnumerator attackcooldown ()
	{
		yield return StartCoroutine ("attackingcooldown");
	}
	private IEnumerator attackingcooldown ()
	{
		yield return new WaitForSeconds (attackCoolDown);
		canAttackAgain = true;
	}

	public void WriteXml(XmlWriter writer)
	{
		XmlSerializer vector3xml = new XmlSerializer(typeof(System.Numerics.Vector3));
		XmlSerializer vector3ListWriter = new XmlSerializer(typeof(List<Vector3>));
		
		// XmlSerializer color3xml = new XmlSerializer(typeof(Color));

		writer.WriteStartElement(nameof(isPlayerVisible));// bool
		writer.WriteValue(isPlayerVisible);
		writer.WriteEndElement();
		writer.WriteStartElement(nameof(playerLastSeen)); // vector3
		vector3xml.Serialize(writer, Convert.New(playerLastSeen));
		writer.WriteEndElement();
		writer.WriteStartElement(nameof(timer)); //float
		writer.WriteValue(timer);
		writer.WriteEndElement();
		writer.WriteStartElement(nameof(canAttackAgain)); //bool
		writer.WriteValue(canAttackAgain);
		writer.WriteEndElement();
		writer.WriteStartElement(nameof(canSwapSuitAgain)); //bool
		writer.WriteValue(canSwapSuitAgain);
		writer.WriteEndElement();
		List<System.Numerics.Vector3> soundSourcesWrite = new List<System.Numerics.Vector3>();
		foreach (var item in soundSources)
		{
			soundSourcesWrite.Add(Convert.New(item));
		}
		writer.WriteStartElement(nameof(soundSourcesWrite)); // list of vector3's
		vector3ListWriter.Serialize(writer, soundSourcesWrite);
		writer.WriteEndElement();
		writer.WriteStartElement(nameof(agent.destination)); // vector3
		vector3xml.Serialize(writer, Convert.New(agent.destination));
		writer.WriteEndElement();
		

		
		// is player visable // done
		//player last seen // done
		//colour of current visor // ????
		//timer // done
		//can attack again //done
		//canspawnsuit // done
		//list of sound sources // done
		// suits current path // done 
		
	}
	public void ReadXml(XmlReader reader)
	{
		XmlSerializer vector3xml = new XmlSerializer(typeof(System.Numerics.Vector3));
		XmlSerializer vector3ListWriter = new XmlSerializer(typeof(List<Vector3>));
		isPlayerVisible = reader.ReadElementContentAsBoolean();
		Convert.Copy((System.Numerics.Vector3) vector3xml.Deserialize(reader), playerLastSeen);
		timer = reader.ReadElementContentAsFloat();
		canAttackAgain = reader.ReadElementContentAsBoolean();
		canSwapSuitAgain = reader.ReadElementContentAsBoolean();
		List<System.Numerics.Vector3> soundSourcesRead = new List<System.Numerics.Vector3>();
		soundSourcesRead = (List<System.Numerics.Vector3>) vector3ListWriter.Deserialize(reader);
		foreach (var item in soundSourcesRead)
		{
			soundSources.Add(Convert.New(item));
		}
		Convert.Copy((System.Numerics.Vector3) vector3xml.Deserialize(reader), agent.destination);

	}
	public XmlSchema GetSchema()
	{
		return (null);
	}
}