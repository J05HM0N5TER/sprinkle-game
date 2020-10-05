using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class LivingArmourAI : MonoBehaviour
{
	private NavMeshAgent agent;
	// The agents camera to see if the player is in the direct view
	[Tooltip ("The Camera of the AI")]
	public Camera DirectCam;
	[Tooltip("The player Object")]
	public GameObject player;
	[Tooltip("The max distance the ai will wonder around from its current point, decrease for ai to not move to other rooms as much")]
	public float wonderDistance = 10.0f;
	[Tooltip("How long will the ai be searching in the area that it last saw the player")]
	public float timer = 10.0f;
	// Stores what the timer was when the game started
	private float resettimer;

	public float normalWalkSpeed = 10;
	public float chaseSpeed = 20;
	public float searchSpeed = 5;
	
	[Tooltip("the area around the last seen point of the player that the ai will search for the player")]
	public float lookingDistance = 1.0f;
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
	public List <GameObject> soundSources = new List<GameObject>();
	[Tooltip("The detection range of hearing for the AI")]
	public float maxHearingRange = 5;
	private bool lookingforplayer = false;
	private float originalWonder;

	//living suit jumping stuff
	private GameObject[] Suits;
	public float maxDistanceFromPlayer;


	//visor colour stuff
	public GameObject visorLight;
	private Light lightvisor;
	public Color chase =  new Color(84, 31, 81, 1);
	public Color investigate =  new Color(161, 100,16, 1);
	public Color search =  new Color(66, 94, 68, 1);
	private Vector3 playerLastSeen = Vector3.zero;

	// Start is called before the first frame update
	void Start()
	{
		agent = gameObject.GetComponent<NavMeshAgent>();
		originalWonder = wonderDistance;
		Suits = GameObject.FindGameObjectsWithTag("Suit");
		//visorLight = GetComponent<Light>();
		lightvisor = visorLight.GetComponent<Light>();
		lightvisor.color = search;
		resettimer = timer;
		agent.speed = normalWalkSpeed;
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
		rayObstructed = Physics.Linecast(/*startPos, endPos,*/ agent.transform.position, player.transform.position, out RaycastHit hitinfo, ~(1<<10) );
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
			agent.SetDestination(playerLastSeen);
			lightvisor.color = chase;
			wasFollowingPlayer = true;
			agent.speed = chaseSpeed;
		}
		/*if (wasFollowingPlayer && !isPlayerVisible)
		{
			// If there is a last seen position go search there
			//if (playerHasBeenSeen)
			//{
			//	agent.SetDestination(playerLastSeen);
			//}
			//LookForPlayer();
			// If the AI has reached the last known position then search again
			if ((agent.transform.position - playerLastSeen).magnitude < 0.5f)
			{
				playerHasBeenSeen = false;
				lookingforplayer = true;
				wasFollowingPlayer = false;
			}
			//while(GoneToLastPoint)
			//{
			//	agent.SetDestination(RandomNavSphere(agent.GetComponent<Transform>().position, lookingDistance, -1));
			//}
		}
		*/
		// TODO: Check that the plaeyr can't be seen, this it for when the AI catches the player
		if ((agent.transform.position - playerLastSeen).magnitude < 0.5f)
		{
			// Debug.Log("Changing to look for player", this);
			lookingforplayer = true;
			wasFollowingPlayer = false;
		}
		if (!agent.hasPath || agent.path == null)
		{
			// for debug purpose this does work
			agent.SetDestination(RandomNavSphere(agent.GetComponent<Transform>().position, wonderDistance, -1));
		}
		// Searching around previous known position
		if(lookingforplayer)
		{
			// Debug.LogError("Looking for player", this);
			
			wonderDistance = lookingDistance;
			//LookForPlayer();
			lightvisor.color = investigate;
			timer -= Time.deltaTime;
			agent.speed = searchSpeed;
		}
		// reseting wonder / looking for player further
		if(timer <= 0)
		{
			// Debug.Log("Timer Up", this);
			// this isnt working
			lookingforplayer = false;
			wonderDistance = originalWonder;
			lightvisor.color = search;
			timer = resettimer;
			agent.speed = normalWalkSpeed;
		}
		// sound reactions
		if (!wasFollowingPlayer || !isPlayerVisible)
		{
			foreach (GameObject SoundSource in soundSources)
			{
				if (Vector3.Distance(gameObject.transform.position, SoundSource.transform.position) <= maxHearingRange)
				{
					agent.SetDestination(SoundSource.transform.position);
					lightvisor.color = investigate;
				}
				if ((agent.transform.position - SoundSource.transform.position).magnitude < 0.5f)
				{
					agent.stoppingDistance = 0.5f;
				}
			}
		}
		/// <summary>
		/// all this for jumping to different suits
		/// </summary>
		
		//TODO: add in jumping to other suits, this code bellow is the basics of it but not fully wokring
		
            
            
		// print("Is seen, trying to find new point...");
		// if (appearsNearPlayer)
		// {
		// 	foreach (GameObject Lurkerpoint in lurkerPoints)
		// 	{
		// 		// Is the spawn point being seen?
		// 		if (!IsVisableToPlayer(Lurkerpoint.transform.position))
		// 		{
		// 			// Find closest valid spawn point
		// 			float distance = UnityEngine.Vector3.Distance(player.GetComponent<Transform>().position, Lurkerpoint.GetComponent<Transform>().position);
		// 			// If their is no points then set this as the current one
		// 			if (closestLurkerPoint == null)
		// 			{
		// 				closestLurkerPoint = Lurkerpoint;
		// 			}
		// 			// If the point is not the current point
		// 			else if (transform != currentLurkingPoint)
		// 			{
		// 				closestLurkerPoint = Lurkerpoint;
		// 			}
		// 		}
		// 		unseenTimer = resetUnseenTimer;
		// 	}
		// }
		// if(!appearsNearPlayer)
		// {
		// 	closestLurkerPoint.transform.position = lurkerWaitingPoint.transform.position;
		// }
		// //actually moving the bloody thing
		// if (closestLurkerPoint != null)
		// {
		// 	//print("Closest point is " + closestLurkerPoint.name);
		// 	spookyTimer -= Time.deltaTime;
		// 	if (spookyTimer <= 0.0f)
		// 	{
		// 		gameObject.GetComponent<Transform>().position = closestLurkerPoint.transform.position;
		// 		gameObject.GetComponent<Transform>().rotation = closestLurkerPoint.transform.rotation;
		// 		currentLurkingPoint = closestLurkerPoint;
		// 		spookyTimer = resettimer;
		// 	}
		// }
		// else
		// {
		// 	Debug.LogError("Couldn't find valid point");
		// }
	
		/// <summary>
		/// 
		/// </summary>
		
	}
	public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
	{
		Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

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
	private void ifSoundInRange()
	{
		Physics.OverlapSphere(gameObject.transform.position, 30);
	}
	private IEnumerator LookForPlayer()
	{
		yield return StartCoroutine("resetLookingForPlayer");
	}
	private IEnumerator resetLookingForPlayer()
	{
		yield return new WaitForSeconds(timer);
		wonderDistance = originalWonder;
		lightvisor.color = search;
	}
}
