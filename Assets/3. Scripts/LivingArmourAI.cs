using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GameSerialization;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using System;
// End of for serialization

public class LivingArmourAI : MonoBehaviour, IXmlSerializable
{
	private NavMeshAgent agent;
	// The agents camera to see if the player is in the direct view
	[Tooltip("The Camera of the AI")]
	public Camera DirectCam;
	[Tooltip("The player Object")]
	private GameObject player;
	[Tooltip("The max distance the ai will wonder around from its current point, decrease for ai to not move to other rooms as much")]
	public float wonderDistance = 10.0f;
	[Tooltip("How long will the ai be searching in the area that it last saw the player")]
	public float timer = 10.0f;
	// Stores what the timer was when the game started
	private float resettimer;

	// How fast the waling speed for the AI is
	public float normalWalkSpeed = 10;
	// The speed that the AI goes at when chasing the player
	public float chaseSpeed = 20;
	// The speed that the AI moves at when serching player last know position
	public float searchSpeed = 5;

	[Tooltip("the area around the last seen point of the player that the ai will search for the player")]
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
	[Tooltip("The positions of sounds that the AI has heard")]
	public List<Vector3> soundSources = new List<Vector3> ();
	[Tooltip ("The detection range of hearing for the AI")]
	public float maxHearingRange = 5;
	private bool lookingforplayer = false;
	// Original wander distance
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
	public Material chase; // = new Color(84, 31, 81, 1);
	public Material investigate; // = new Color(161, 100, 16, 1);
	public Material search; // = new Color(66, 94, 68, 1);
	private Color chaseLight = new Color(84, 31, 81, 1);
	private Color investigateLight = new Color(161, 100, 16, 1);
	private Color searchLight = new Color(66, 94, 68, 1);

	private Vector3 playerLastSeen = Vector3.zero;
	[Header("attacking player values")]
	//attacking player stuff
	public float attackDistance = 3;
	public float attackCoolDown = 1;
	public float stopafterattacktime = 1;
	private float stopafterattacktimereset;
	private float attackcooldownreset;
	public bool canAttackAgain = true;
	public float knockBack = 5000;

	private Animator anim;
	public float lookAroundTimer = 4;
	private float lookAroundTimerReset;

	
	public float detectionTimer = 1;
	private float detectionTimerReset;
	public float crouchDetectionTimer = 2;
	private float crouchDetectionTimerReset;
	private Quaternion lookRotation;
    private Vector3 direction;

	public enum AIStates
	{
		Idle, // stopping and idleing in spot / taking a break
		Wondering, // roaming the map
		Chasing, // chasing the player 
		Searching, // searching around for player at last seen pos / going to sound
		SwapSuit, // player to far so swap suit
		DetectingPlayer // seeing and detection timer stuff
	}
	AIStates CurrentState;
	public bool busyWithState;
	// Start is called before the first frame update
	void Start()
	{
		agent = gameObject.GetComponent<NavMeshAgent>();
		originalWonder = wonderDistance;
		suits = GameObject.FindGameObjectsWithTag("Suit");
		//visorLight = GetComponent<Light>();1
		lightvisor = visorLight.GetComponent<Light>();
		lightvisor.color = searchLight;
		resettimer = timer;
		agent.speed = normalWalkSpeed;
		//visorEmission = search;
		visorEmission.SetColor("_EmissiveColor", searchLight);
		visorEmission.EnableKeyword("_EMISSION");
		visorEmission.color = searchLight;
		agent.autoBraking = true;
		agent.acceleration = 20;

		playerCam = Camera.main;
		attackcooldownreset = attackCoolDown;
		anim = gameObject.GetComponentInChildren<Animator>();
		player = GameObject.FindGameObjectWithTag("Player");

		resetIdleLookTime = idleLookTime;
		CurrentState = AIStates.Wondering;

		anim.gameObject.GetComponent<Animator>().enabled = true;

		lookAroundTimerReset = lookAroundTimer;
		stopafterattacktimereset = stopafterattacktime;
		detectionTimerReset = detectionTimer;
		crouchDetectionTimerReset = crouchDetectionTimer;
		


	}
	

	// Update is called once per frame  
	void Update()
	{
		// Player position on the AI camera view
		Vector3 screenPoint = DirectCam.WorldToViewportPoint(player.GetComponent<Transform>().position);
		// Is the player within the view bounds
		playerInScreenBounds = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
		// The position the player was last seen at by the AI (Updated when the player is discovered)
		// Is the player within screen bounds and nothing is obstructing view
		rayObstructed = Physics.Linecast( /*startPos, endPos,*/ agent.transform.position, player.transform.position, out RaycastHit hitinfo, ~(1 << 10));
		// Print out what the ray hit
		// Debug view
		isPlayerVisible = playerInScreenBounds && !rayObstructed;
		// TODO: Check that the player can't be seen, this it for when the AI catches the player
	
		if((agent.transform.position - playerLastSeen).magnitude < attackDistance && isPlayerVisible && canAttackAgain)
		{
			player.GetComponent<PlayerController>().health -= 1;
			if(player.GetComponent<PlayerController>().health <= 0)
			{
				GameObject.Find("PauseManager").GetComponent<PauseMenu>().PauseGame();
			}
			anim.SetBool("attack", true);
			var dir = (player.transform.position - transform.position).normalized;
			player.GetComponent<Rigidbody>().AddForce(knockBack * dir);
			//anim.SetBool("attack", false);
			canAttackAgain = false;
			CurrentState = AIStates.Idle;

			//attackcooldown();
		}
		if (!canAttackAgain)
		{
			attackCoolDown -= Time.deltaTime;
			if (attackCoolDown <= 0)
			{
				
				canAttackAgain = true;
				attackCoolDown = attackcooldownreset;
			}
		}
		

		//TODO: add in information sharing

		
		if(agent.velocity.magnitude >= 0.1f)
		{
			anim.SetBool("walking", true);
		}
		anim.speed = agent.speed / 2;

		
		if (((gameObject.transform.position - player.transform.position).magnitude > maxDistanceFromPlayer) && !isPlayerVisible && suits.Length >= 1)
		{
			CurrentState = AIStates.SwapSuit;
		}
		
		if(CurrentState == AIStates.Idle)
		{
			agent.isStopped = true;
			if(CurrentState == AIStates.Idle)
			{
				stopafterattacktime -= Time.deltaTime;
				if(stopafterattacktime <= 0)
				{
					stopafterattacktime = stopafterattacktimereset;
					anim.SetBool("attack", false);
					agent.isStopped = false;
					CurrentState = AIStates.Searching;
				}
			}
		}
		if(CurrentState == AIStates.Wondering)
		{
			
			anim.SetBool("walking", true);
			if (agent.remainingDistance <= 1)
			{
				// for debug purpose this does work
				agent.SetDestination (RandomNavSphere (agent.GetComponent<Transform> ().position, wonderDistance, -1));
			}
			if(wonderDistance != originalWonder)
			{
				wonderDistance = originalWonder;
				agent.speed = normalWalkSpeed;
				
			}
			lightvisor.color = searchLight;
			//visorEmission = search;
			visorEmission.SetColor ("_EmissiveColor", searchLight);
			visorEmission.EnableKeyword ("_EMISSION");

			// Change state
			if(isPlayerVisible)
			{
				CurrentState = AIStates.DetectingPlayer;
			}
			else if((!wasFollowingPlayer || !isPlayerVisible) && soundSources.Count >= 1)
			{
				CurrentState = AIStates.Searching;
			}
		}
		if(CurrentState == AIStates.Chasing)
		{
			
			anim.SetBool("running", true);
			anim.SetBool("walking", false);
			if(isPlayerVisible)
			{
				playerLastSeen = player.transform.position;
			}
			
			// Set the AI to go towards the player
			agent.SetDestination (playerLastSeen);
			wasFollowingPlayer = true;
			agent.speed = chaseSpeed;
			lightvisor.color = chaseLight;
			//visorEmission = chase;
			visorEmission.SetColor ("_EmissiveColor", chaseLight);
			visorEmission.EnableKeyword ("_EMISSION");
			if((agent.transform.position - playerLastSeen).magnitude < 1)
			{
				CurrentState = AIStates.Searching;
				anim.SetBool("walking", true);
				anim.SetBool("running", false);
			}
		}
		if(CurrentState == AIStates.Searching)
		{
			
			anim.SetBool("walking", true);
			if(soundSources.Count == 0)
			{
				// if (agent.remainingDistance <= 0.5)
				// {
				// 	// for debug purpose this does work
				// 	agent.SetDestination (RandomNavSphere (agent.GetComponent<Transform> ().position, wonderDistance, NavMesh.AllAreas));
				// }
				agent.speed = searchSpeed;
				wonderDistance = lookingDistance;
				//LookForPlayer();
				lightvisor.color = investigateLight;
				timer -= Time.deltaTime;
				//visorEmission = investigate;
				anim.SetBool("walking", true);
				if(agent.remainingDistance <= 0.5)
				{
					//stand and do looking animation //TODO: add look around animation
					lookAroundTimer -= Time.deltaTime;
					anim.SetBool("searching", true);
					anim.SetBool("walking", false);
					if(lookAroundTimer <= 0)
					{
						anim.SetBool("searching", false);
						anim.SetBool("walking", true);
						agent.SetDestination (RandomNavSphere (agent.GetComponent<Transform> ().position, wonderDistance, NavMesh.AllAreas));
						lookAroundTimer = lookAroundTimerReset;
						
					}
				}
				

				visorEmission.SetColor ("_EmissiveColor", investigateLight);
				visorEmission.EnableKeyword ("_EMISSION");
			}
			else
			{
				foreach (Vector3 soundSource in soundSources)
				{
					if (Vector3.Distance (gameObject.transform.position, soundSource) <= maxHearingRange)
					{
						agent.SetDestination (soundSource);
						lookingforplayer = true;
						lightvisor.color = investigateLight;
						//visorEmission = investigate;
						visorEmission.SetColor ("_EmissiveColor", investigateLight);
						visorEmission.EnableKeyword ("_EMISSION");
						agent.speed = normalWalkSpeed;
						anim.SetBool("searching", false);
						anim.SetBool("walking", true);
					}
					if ((agent.transform.position - soundSource).magnitude < 0.5f)
					{
						//agent.SetDestination (RandomNavSphere (agent.GetComponent<Transform> ().position, wonderDistance, -1));
						// CurrentState = AIStates.Searching;
						soundSources.Remove(soundSource);
					}
				}
			}
			
			if (timer <= 0 && !isPlayerVisible)
			{
				lookingforplayer = false;
				//lightvisor.color = search;
				timer = resettimer;
				CurrentState = AIStates.Wondering;
				anim.SetBool("searching", false);
				// Debug.Log("Timer Up", this);
			}
			if(isPlayerVisible)
			{
				CurrentState = AIStates.Chasing;
			}
		}
		//TODO: make sure animations stop on the living suits
		if(CurrentState == AIStates.SwapSuit)
		{
			
			suits = GameObject.FindGameObjectsWithTag ("Suit");
			currentSuit = gameObject;
			foreach (GameObject suit in suits)
			{
				// Is the spawn point being seen?
				if (!IsVisableToPlayer(suit.transform.position))
				{
					// Find closest valid spawn point
					float distance = UnityEngine.Vector3.Distance(player.GetComponent<Transform>().position, suit.GetComponent<Transform>().position);

					// If their is no points then set this as the current one
					if (closestSuit == null && closestSuit != currentSuit)
					{
						closestSuit = suit;
					}
					// If the point is not the current point
					else if ((transform != currentSuit) && (distance < (closestSuit.transform.position - player.transform.position).magnitude))
					{
						closestSuit = suit;
					}
				}
			}
			//! : check if this is still bugged, if range is too small then the suit will turn itself off, if bug is persistant add timer maybe.
			if ((closestSuit != currentSuit) && canSwapSuitAgain)
			{
				print("changing suit " + closestSuit.name);
				closestSuit.GetComponent<LivingArmourAI>().enabled = true;
				currentSuit = closestSuit;
				this.enabled = false;
				canSwapSuitAgain = false;
				SwapSuit();
			}
			else
			{
				Debug.LogError("Couldn't find valid point");
			}
		}
		if(CurrentState == AIStates.DetectingPlayer)
		{
			
			if(isPlayerVisible)
			{
				agent.isStopped = true;
				direction = (player.transform.position - transform.position).normalized;
				//set the current looking rotation the direction the target is at
				lookRotation = Quaternion.LookRotation(direction);
				lightvisor.color = investigateLight;
				//visorEmission = investigate;
				visorEmission.SetColor ("_EmissiveColor", investigateLight);
				visorEmission.EnableKeyword ("_EMISSION");
				transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);
				anim.SetBool("walking", false);
				anim.SetBool("idle", true);
				if(player.GetComponent<PlayerController>().isCrouching)
				{
					crouchDetectionTimer -= Time.deltaTime;
				}
				else
				{
					detectionTimer -= Time.deltaTime;
				}
				if(detectionTimer <= 0 || crouchDetectionTimer <= 0)
				{
					agent.isStopped = false;
					CurrentState = AIStates.Chasing;
					detectionTimer = detectionTimerReset;
					crouchDetectionTimer = crouchDetectionTimerReset;
				}
				
			}
		}

		Debug.Log(CurrentState.ToString());
		Debug.Log(" can attack again: " + canAttackAgain + ", time until next attack: " + attackCoolDown + ", bool of animaition attack: "+ anim.GetBool("attack"));
	}
	public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
	{
		Vector3 randomDirection = UnityEngine.Random.onUnitSphere * distance;

		randomDirection += origin;

		NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, distance, layermask);

		return navHit.position;
	}
	private void OnDrawGizmos()
	{
		if (agent)
		{
			if (rayObstructed)
				Gizmos.color = Color.red;
			else
				Gizmos.color = Color.blue;
			Gizmos.DrawLine(agent.transform.position, player.transform.position);

			Gizmos.DrawWireSphere(agent.transform.position, wonderDistance);
		}
	}
	private void IfSoundInRange ()
	{
		Physics.OverlapSphere(gameObject.transform.position, 30);
	}
	private IEnumerator SwapSuit()
	{
		yield return StartCoroutine("SwappingSuitTimer");
	}
	private IEnumerator SwappingSuitTimer()
	{
		// 
		// wonderDistance = originalWonder;
		// lightvisor.color = search;
		yield return new WaitForSeconds(swapTimer);
		canSwapSuitAgain = true;

	}
	private bool IsVisableToPlayer(UnityEngine.Vector3 position)
	{
		// lurker in player view
		UnityEngine.Vector3 screenPoint = playerCam.WorldToViewportPoint(position);
		// Is the player within the view bounds
		bool InScreenBounds = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
		// Is the player within screen bounds and nothing is obstructing view
		bool rayObstructed = Physics.Linecast( /*startPos, endPos,*/ position, player.transform.position,
			out RaycastHit hitinfo, ~((1 << 9) | (1 << 10))); // ignore layer 9 and 10
		// Print out what the ray hit
		// Debug view
		return InScreenBounds && !rayObstructed;
	}
	//attacking player cooldown
	private IEnumerator Attackcooldown ()
	{
		yield return StartCoroutine("attackingcooldown");
	}
	private IEnumerator Attackingcooldown ()
	{
		yield return new WaitForSeconds(attackCoolDown);
		canAttackAgain = true;
	}
	// saving stuff no touchy touchy
	public void WriteXml(XmlWriter writer)
	{
		XmlSerializer vector3xml = new XmlSerializer(typeof(System.Numerics.Vector3));
		XmlSerializer vector3ListWriter = new XmlSerializer(typeof(List<System.Numerics.Vector3>));

		// XmlSerializer color3xml = new XmlSerializer(typeof(Color));

		writer.WriteStartElement(nameof(isPlayerVisible)); // bool
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
	}
	public void ReadXml(XmlReader reader)
	{
		XmlSerializer vector3xml = new XmlSerializer(typeof(System.Numerics.Vector3));
		XmlSerializer vector3ListWriter = new XmlSerializer(typeof(List<System.Numerics.Vector3>));
		isPlayerVisible = reader.ReadElementContentAsBoolean();
		reader.ReadStartElement();
		Convert.Copy((System.Numerics.Vector3) vector3xml.Deserialize(reader), playerLastSeen);
		reader.ReadEndElement();
		timer = reader.ReadElementContentAsFloat();
		canAttackAgain = reader.ReadElementContentAsBoolean();
		canSwapSuitAgain = reader.ReadElementContentAsBoolean();
		List<System.Numerics.Vector3> soundSourcesRead = new List<System.Numerics.Vector3>();
		reader.ReadStartElement();
		soundSourcesRead = (List<System.Numerics.Vector3>) vector3ListWriter.Deserialize(reader);
		reader.ReadEndElement();
		foreach (var item in soundSourcesRead)
		{
			soundSources.Add(Convert.New(item));
		}

		reader.ReadStartElement();
		Convert.Copy((System.Numerics.Vector3) vector3xml.Deserialize(reader), agent.destination);
		reader.ReadEndElement();

	}
	public XmlSchema GetSchema()
	{
		return (null);
	}
}
