using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOff : MonoBehaviour
{
    public List<GameObject> turnOnStuff;
    public List<GameObject> turnOffStuff;
    // Start is called before the first frame update
    void Start()
    {
        if((turnOnStuff != null))
        {
            foreach(var obj in turnOnStuff)
            {
                obj.SetActive(false);
            }
        }
        
        if((turnOffStuff != null))
        {
            foreach(var obj in turnOffStuff)
            {
                obj.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other == GameObject.FindGameObjectWithTag("Player") && (turnOffStuff != null) && (turnOnStuff != null))
        {
            if((turnOnStuff != null))
            {
                foreach(var obj in turnOnStuff)
                {
                    obj.SetActive(true);
                }
            }
            
            if((turnOffStuff != null))
            {
                foreach(var obj in turnOffStuff)
                {
                    obj.SetActive(false);
                }
            }
            
        }
    }
}
