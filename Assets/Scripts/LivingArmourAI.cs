using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class LivingArmourAI : MonoBehaviour
{
    NavMeshAgent agent;
    public Camera DirectCam;
    public GameObject player;
    public bool wasFollowingPlayer = false;
    public bool canSeePlayer = false;
    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame  
    void Update()
    {
        
        Vector3 screenPoint = DirectCam.WorldToViewportPoint(player.GetComponent<Transform>().position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        Vector3 playerLastSeen = screenPoint;
        if(player.GetComponent<Renderer>().isVisible)
        {
            agent.SetDestination(player.transform.position);
            wasFollowingPlayer = true;
            canSeePlayer = true;
        }
        if(!onScreen && !player.GetComponent<Renderer>().isVisible)
        {
            canSeePlayer = false;
        }
        if (wasFollowingPlayer && !onScreen)
        {
            agent.SetDestination(playerLastSeen);
            if(agent.transform.position == playerLastSeen)
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
