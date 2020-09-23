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

	// Start is called before the first frame update
	void Start()
	{
		agent = gameObject.GetComponent<NavMeshAgent>();
		originalWonder = wonderDistance;
		Suits = GameObject.FindGameObjectsWithTag("Suit");
	}

	// Update is called once per frame  
	void Update()
	{
		// Player position on the AI camera view
		Vector3 screenPoint = DirectCam.WorldToViewportPoint(player.GetComponent<Transform>().position);
		// Is the player within the view bounds
		playerInScreenBounds = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
		// The position the player was last seen at by the AI (Updated when the player is discovered)
		Vector3 playerLastSeen = Vector3.zero;
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
			
			wasFollowingPlayer = true;
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
		if ((agent.transform.position - playerLastSeen).magnitude < 0.5f)
		{
			
			lookingforplayer = true;
			wasFollowingPlayer = false;
		}
		if (!agent.hasPath || agent.path == null)
		{
			agent.SetDestination(RandomNavSphere(agent.GetComponent<Transform>().position, wonderDistance, -1));
		}
		if(lookingforplayer)
		{
			wonderDistance = lookingDistance;
			//LookForPlayer();
			timer -= Time.deltaTime;
		}
		if(timer <= 0)
		{
			lookingforplayer = false;
			wonderDistance = originalWonder;
		}
		if (!wasFollowingPlayer || !isPlayerVisible)
		{
			foreach (GameObject SoundSource in soundSources)
			{
				if (Vector3.Distance(gameObject.transform.position, SoundSource.transform.position) <= maxHearingRange)
				{
					agent.SetDestination(SoundSource.transform.position);
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
	}
}
