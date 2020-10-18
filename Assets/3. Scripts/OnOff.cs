using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOff : MonoBehaviour
{
    public List<GameObject> turnOnStuff = new List<GameObject>();
    public List<GameObject> turnOffStuff = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        // If the player entered the trigger
        if (other.tag == "Player")
        {
            foreach (var obj in turnOnStuff)
            {
                obj.SetActive(true);
            }
            foreach (var obj in turnOffStuff)
            {
                obj.SetActive(false);
            }
        }
    }
}
