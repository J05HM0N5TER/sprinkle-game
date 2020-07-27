using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LurkerAi : MonoBehaviour
{
    //public List<GameObject> lurkerPoints = new List<GameObject>();
    public GameObject[] lurkerPoints;
    public GameObject player;
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
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
