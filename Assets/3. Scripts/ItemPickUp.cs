using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
	[Tooltip("Max distance away that the object can be interacted with")]
	public float maxDistanceToInteract;
	private GameObject player;
	CameraControl cameraControl;
	[Tooltip("The item that you want to give the player when picking this thing up")]
	public PlayerController.Inventory item;

	// Start is called before the first frame update
	void Start()
	{
		cameraControl = FindObjectOfType<CameraControl>();
		player = GameObject.FindGameObjectWithTag("Player");

#if UNITY_EDITOR
		if (cameraControl == null)
		{
			Debug.LogError(("Couldn't find cameraControl", this));

		}
#endif
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetButtonDown("Interact"))
		{
			if (Physics.Raycast(cameraControl.CursorToRay(), out RaycastHit hit, maxDistanceToInteract) && // Raycast check
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
