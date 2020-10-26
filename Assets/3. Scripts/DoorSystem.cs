using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSystem : MonoBehaviour
{
    private List<GameObject> thingsInDoorway = new List<GameObject>();
    public GameObject door;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(thingsInDoorway.Count > 0)
        {
            door.GetComponent<TriggerScript>().locked = true;
        }
        if(thingsInDoorway.Count <= 0)
        {
            door.GetComponent<TriggerScript>().locked = false;
        }
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(!(other.tag == "Door"))
        {
            thingsInDoorway.Add(other.gameObject);
        }
    }
    
    private void OnTriggerExit(Collider other) 
    {
        if(!(other.tag == "Door"))
        {
            thingsInDoorway.Remove(other.gameObject);
        }
    }
}
