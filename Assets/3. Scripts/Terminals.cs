using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Terminals : MonoBehaviour
{
	[Tooltip("The door that this terminal will open")]
	public GameObject doorToOpen;
	[Tooltip("They keycard that is needed to use the terminal")]
	public PlayerController.Inventory neededKeyCard;
	private GameObject player;
	[Tooltip("the max distance that the player can be from the terminal and still interact with it")]
	public float maxDistanceToInteract;
	public bool brokenTerminal = false;
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
			if (Physics.Raycast(ray, out RaycastHit hit) &&  // Raycast check
				hit.collider.gameObject == gameObject && // Raycast hit this object
				Vector3.Distance(hit.transform.position, gameObject.transform.position) <= maxDistanceToInteract)  // The player is in range 
			{
				// The player has the key card needed
				if ((player.GetComponent<PlayerController>().inventory & neededKeyCard) == neededKeyCard && !brokenTerminal)
				{
					// Unlock script
					doorToOpen.GetComponent<TriggerScript>().locked = false;
					// Unlock Animation
					gameObject.GetComponent<Animator>().SetTrigger("Unlock");
				}
				if(player.GetComponent<PlayerController>().inventory.HasFlag(PlayerController.Inventory.SolderingIron) && brokenTerminal)
				{
					gameObject.GetComponent<Animator>().SetTrigger("FixTerminal");
					brokenTerminal = false;
				}
			}
		}
	}
}
