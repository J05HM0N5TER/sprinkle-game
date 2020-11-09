using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SludgeDPS : MonoBehaviour
{
    private GameObject player;
    public float DPS = 2.0f;
    private bool playerInDPSArea = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInDPSArea)
        {
             player.GetComponent<PlayerController>().health -= DPS * Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other == player)
        {
            playerInDPSArea = true;
            //player.GetComponent<PlayerController>().health -= DPS;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other == player)
        {
              playerInDPSArea = false;
        }
        
    }
}
