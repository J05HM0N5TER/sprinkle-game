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
	private float timerReset;
	[Tooltip("the area around the last seen point of the player that the ai will search for the player")]
	public float lookngDistance = 1.0f;
	// Was the AI previously following the player?
	private bool wasFollowingPlayer = false;
	private bool isPlayerVisable = false;
	// Has the player ever been seen by this AI?
	private bool playerHasBeenSeen = false;
	// Is the ray between the player and AI colliding with something
    private bool rayObstructed = true;
	// Is the player in the view area for the AI
    private bool playerInScreenBounds = false;
	bool GoneToLastPoint;

	//hearing and sound stuff
	//public GameObject[] soundSources;
	public List <GameObject> soundSources = new List<GameObject>();
	[Tooltip("The detection range of hearing for the AI")]
	public float maxHearingRange = 5;
	private bool lookingforplayer = false;
	private float originalWonder;

	// Start is called before the first frame update
	void Start()
	{
		agent = gameObject.GetComponent<NavMeshAgent>();
		originalWonder = wonderDistance;
		timerReset = timer;
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
        isPlayerVisable = playerInScreenBounds && !rayObstructed;
		// If the player is currently seen
		if (isPlayerVisable)
		{
			playerLastSeen = player.transform.position;
			// Set the AI to go towards the player
			agent.SetDestination(playerLastSeen);
			playerHasBeenSeen = true;
			wasFollowingPlayer = true;
		}
		/*if (wasFollowingPlayer && !isPlayerVisable)
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
			//	agent.SetDestination(RandomNavSphere(agent.GetComponent<Transform>().position, lookngDistance, -1));
			//}
		}
		*/
		if ((agent.transform.position - playerLastSeen).magnitude < 0.5f)
		{
			playerHasBeenSeen = false;
			lookingforplayer = true;
			wasFollowingPlayer = false;
		}
		if (!agent.hasPath || agent.path == null)
		{
			agent.SetDestination(RandomNavSphere(agent.GetComponent<Transform>().position, wonderDistance, -1));
		}
		if(lookingforplayer)
		{
			wonderDistance = lookngDistance;
			//LookForPlayer();
			timer -= Time.deltaTime;
		}
		if(timer <= 0)
		{
			lookingforplayer = false;
			wonderDistance = originalWonder;
		}
		if (!wasFollowingPlayer || !isPlayerVisable)
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
