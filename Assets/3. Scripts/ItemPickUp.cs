using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [Tooltip("Max distance away that the object can be interacted with")]
    public float maxDistanceToInteract;
    private GameObject player;
    [Tooltip("The item that you want to give the player when picking this thing up")]
    public PlayerController.Inventory item;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact"))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit, maxDistanceToInteract) &&  // Raycast check
				hit.collider.gameObject == gameObject)
            {
                if(item == PlayerController.Inventory.MedicalSyringe)
                {
                    player.GetComponent<PlayerController>().medSyringes ++;
                }
                player.GetComponent<PlayerController>().inventory |= item; 
                Destroy(gameObject);
            }
        }
    }
}
