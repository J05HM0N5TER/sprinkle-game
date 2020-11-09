using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractOnOff : MonoBehaviour
{

	public List<GameObject> turnOnStuff = new List<GameObject>();
	public List<GameObject> turnOffStuff = new List<GameObject>();
	public float maxDistanceToInteract = 2;
	private bool activated = false;
	private CameraControl cameraControl;
	// Start is called before the first frame update
	void Start()
	{
		cameraControl = FindObjectOfType<CameraControl>();

#if UNITY_EDITOR
		if (cameraControl == null)
		{
			Debug.LogError(("Couldn't find cameraControl", this));
		}
#endif
	}

	private void Update()
	{
		if (Input.GetButtonDown("Interact"))
		{
			if (Physics.Raycast(cameraControl.CursorToRay(), out RaycastHit hit, maxDistanceToInteract) && // Raycast check
				hit.collider.gameObject == gameObject)
			{
				// Turn on all the objects
				foreach (var obj in turnOnStuff)
				{
					//obj.SetActive(true);
					foreach (var childcomp in turnOnStuff)
					{
						//childcomp.GetComponents<Behaviour>();
						foreach (Behaviour child in childcomp.GetComponents<Behaviour>())
						{
							child.enabled = true;
						}
					}
				}

				// Turn off all the objects
				foreach (var obj in turnOffStuff)
				{
					//obj.SetActive(false);
					foreach (var childcomp in turnOffStuff)
					{
						//childcomp.GetComponents<Behaviour>();
						foreach (Behaviour child in childcomp.GetComponents<Behaviour>())
						{
							child.enabled = false;
						}
					}
				}
				activated = true;
			}
		}
		if (activated)
		{
			this.enabled = false;
		}
	}
}
