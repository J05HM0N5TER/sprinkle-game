using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractOnOff : MonoBehaviour
{
    
    public List<GameObject> turnOnStuff;
    public List<GameObject> turnOffStuff;
    public float maxDistanceToInteract = 2;
    // Start is called before the first frame update
    void Start()
    {
        if(turnOnStuff != null)
        {
            foreach(var obj in turnOnStuff)
            {
                obj.SetActive(false);
            }
        }
        
        if(turnOffStuff != null)
        {
            foreach(var obj in turnOffStuff)
            {
                obj.SetActive(true);
            }
        }
    }
    private void Update() 
    {
        if (Input.GetButtonDown("Interact"))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit, maxDistanceToInteract) &&  // Raycast check
				hit.collider.gameObject == gameObject)
            {
                if(turnOnStuff != null)
                {
                    foreach(var obj in turnOnStuff)
                    {
                        obj.SetActive(true);
                    }
                }
                
                if(turnOffStuff != null)
                {
                    foreach(var obj in turnOffStuff)
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }
    }
}
