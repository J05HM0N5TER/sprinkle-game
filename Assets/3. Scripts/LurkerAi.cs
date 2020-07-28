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
    public bool wasSeen;
    public GameObject closestLurkerPoint;
    public float closestDistance;

    // Start is called before the first frame update
    void Start()
    {
        lurkerPoints = GameObject.FindGameObjectsWithTag("Respawn");
        foreach (GameObject Lurkerpoint in lurkerPoints)
        {
            print(Lurkerpoint.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if looked at disable / invis
        if (gameObject.GetComponent<Renderer>().isVisible)
        {
            //gameObject.SetActive(false);
            wasSeen = true;
        }
        else
        {
            //gameObject.SetActive(true);
        }
        if(wasSeen)
        {
            foreach (GameObject Lurkerpoint in lurkerPoints)
            {
                bool availableLurkPoint;
                if (Lurkerpoint.GetComponent<LurkerPoint>().isVisable)
                {
                    availableLurkPoint = false;
                }
                if (!Lurkerpoint.GetComponent<LurkerPoint>().isVisable)
                {
                    availableLurkPoint = true;
                    float distance = UnityEngine.Vector3.Distance(player.GetComponent<Transform>().position, Lurkerpoint.GetComponent<Transform>().position);
                    if(closestLurkerPoint == null)
                    {
                        closestLurkerPoint = Lurkerpoint;
                        closestDistance = distance;
                    }
                    if(distance < closestDistance)
                    {
                        closestLurkerPoint = Lurkerpoint;
                        closestDistance = distance;
                    }
                    
                        //Physics.Linecast(player.GetComponent<Transform>().position, Lurkerpoint.transform.position)
                }
                
            }
            gameObject.GetComponent<Transform>().position = closestLurkerPoint.transform.position;
            wasSeen = false;
        }
        



    }
}
