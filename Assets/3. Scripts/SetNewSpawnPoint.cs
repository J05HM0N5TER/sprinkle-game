using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNewSpawnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject newSpawnPoint;
    public GameObject deathscript;
    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            deathscript.GetComponent<DeathScript>().respawnPoint = newSpawnPoint;
        }
    }
}
