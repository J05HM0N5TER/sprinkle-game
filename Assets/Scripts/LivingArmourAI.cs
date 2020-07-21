using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class LivingArmourAI : MonoBehaviour
{
	NavMeshAgent agent;
	// The agents camera to see if the player is in the direct view
	public Camera DirectCam;
	public GameObject player;
	public bool wasFollowingPlayer = false;
	public bool canSeePlayer = false;
	// Has the player ever been seen by this AI?
	bool playerHasBeenSeen = false;
	// Start is called before the first frame update
	void Start()
	{
		agent = gameObject.GetComponent<NavMeshAgent>();
	}

	// Update is called once per frame  
	void Update()
	{
		// Player position on the AI camera view
		Vector3 screenPoint = DirectCam.WorldToViewportPoint(player.GetComponent<Transform>().position);
		// Is the player within the view bounds
		bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
		// The position the player was last seen at by the AI (Updated when the player is discovered)
		Vector3 playerLastSeen = Vector3.zero;
        // Is the player within screen bounds and nothing is obstructing view
        Vector3 startPos = transform.position + ((player.transform.position - transform.position).normalized * 0.5f); 
        Vector3 endPos = player.transform.position + ((transform.transform.position - player.transform.position).normalized * 0.5f); 
		bool isPlayerVisable = onScreen && !Physics.Linecast(startPos, endPos,/*agent.transform.position, player.transform.position,*/ /*0b100000000*/(1 << 9)/*9*/);
		// If the player is currently seen
		if (isPlayerVisable)
		{
			playerLastSeen = player.transform.position;
			// Set the AI to go towards the player
			agent.SetDestination(playerLastSeen);
			wasFollowingPlayer = true;
			canSeePlayer = true;
			playerHasBeenSeen = true;
		}
		else
		{
			canSeePlayer = false;
		}

		if (wasFollowingPlayer && !isPlayerVisable)
		{
			if (playerHasBeenSeen)
				agent.SetDestination(playerLastSeen);
			if ((agent.transform.position - playerLastSeen).magnitude < 0.5f)
			{
				wasFollowingPlayer = false;
			}
		}

		if (!agent.hasPath)
		{
			agent.SetDestination(RandomNavSphere(agent.GetComponent<Transform>().position, 50f, -1));
		}


	}
	public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
	{
		Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

		randomDirection += origin;

		NavMeshHit navHit;

		NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

		return navHit.position;
	}
}
