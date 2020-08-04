using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class LurkerAi : MonoBehaviour
{
    //public List<GameObject> lurkerPoints = new List<GameObject>();
    public GameObject[] lurkerPoints;
    public GameObject player;
    GameObject currentLurkingPoint;
    private GameObject closestLurkerPoint;
    private float closestDistance;
    private bool debugIsVisable;
    private float spookyTimer = 0.5f;
    public Camera playerCam;

    public float unseenTimer = 20.0f;
    private float resetUnseenTimer;
    // Start is called before the first frame update
    void Start()
    {
        lurkerPoints = GameObject.FindGameObjectsWithTag("Respawn");
        foreach (GameObject Lurkerpoint in lurkerPoints)
        {
            print("Lurker point: " + Lurkerpoint.transform.position);
        }
        resetUnseenTimer = unseenTimer;
    }

    // Update is called once per frame
    void Update()
    {
        unseenTimer -= Time.deltaTime;
        // Is the lurker being seen?
        if (IsVisableToPlayer(transform.position) || unseenTimer <= 0.0f)// gameObject.GetComponent<Renderer>().isVisible)
        {
            debugIsVisable = true;
            print("Is seen, trying to find new point...");
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
                        closestDistance = distance;
                    }
                    // If the point is not the current point
                    else if (transform != currentLurkingPoint)
                    { 
                        closestLurkerPoint = Lurkerpoint;
                        closestDistance = distance;
                    } 

                    //Physics.Linecast(player.GetComponent<Transform>().position, Lurkerpoint.transform.position)
                }
                unseenTimer = resetUnseenTimer;

            }
            if (closestLurkerPoint != null)
            {
                print("Closest point is " + closestLurkerPoint.name);
                spookyTimer -= Time.deltaTime;
                if (spookyTimer <= 0.0f)
                {
                    gameObject.GetComponent<Transform>().position = closestLurkerPoint.transform.position;
                    currentLurkingPoint = closestLurkerPoint;
                    spookyTimer = 0.5f;
                }
                
            }
            else
            {
                Debug.LogError("Couldn't find valid point");
            }
        }
        else
        {
            debugIsVisable = false;
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
        if (rayObstructed)
            print("Ray hit: " + hitinfo.collider.name + " at: " + hitinfo.point.x + ", " + hitinfo.point.y);
        // Debug view
        return InScreenBounds && !rayObstructed;
    }
}
