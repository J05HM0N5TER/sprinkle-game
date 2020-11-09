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
	private PlayerController player;
	[Tooltip("the max distance that the player can be from the terminal and still interact with it")]
	public float maxDistanceToInteract = 2;
	public bool brokenTerminal = false;
	[Tooltip("Spark Particles")]
	public GameObject brokenParticles;
	private ParticleSystem ps;
	public Material screenMaterial;
	public Material brokenMaterial;
	public Material unlockedMaterial;
	public Material lockedMaterial;
	private CameraControl cameraControl;
	// Start is called before the first frame update
	void Start()
	{
		cameraControl = FindObjectOfType<CameraControl>();
		player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
		ps = brokenParticles.GetComponent<ParticleSystem>();
		var em = ps.emission;
		if (brokenTerminal)
		{
			ps.Play();
			em.enabled = true;
		}
		else
		{
			em.enabled = false;
			ps.Stop();

		}

		// UpdateDisplay();
#if UNITY_EDITOR
		if (player == null)
		{
			Debug.LogError("Failed to find player", this);
		}
		if (cameraControl == null)
		{
			Debug.LogError(("Couldn't find cameraControl", this));
		}
#endif
	}

	// Update is called once per frame
	void Update()
	{
		if (screenMaterial == null)
		{
			return;
		}
		screenMaterial.SetColor("_EmissiveColor", new Color(161, 100, 16, 1));
		screenMaterial.SetColor("_EmissionColor", new Color(161, 100, 16, 1));
		screenMaterial.EnableKeyword("_EMISSION");
		try
		{

			if (brokenTerminal)
			{
				screenMaterial.SetColor("_EmissiveColor", new Color(84, 31, 81, 1));
				screenMaterial.EnableKeyword("_EMISSION");
			}
			else if (!PlayerHasKeyCards())
			{
				screenMaterial.SetColor("_EmissiveColor", new Color(161, 100, 16, 1));
				screenMaterial.EnableKeyword("_EMISSION");
			}
			else
			{
				screenMaterial.SetColor("_EmissiveColor", new Color(66, 94, 68, 1));
				screenMaterial.EnableKeyword("_EMISSION");
			}
		}
		catch (System.Exception e)
		{
			Debug.LogError(e.ToString());
			throw;
		}

		// UpdateDisplay();
		if (Input.GetButtonDown("Interact"))
		{
			if (Physics.Raycast(cameraControl.CursorToRay(), out RaycastHit hit, maxDistanceToInteract) && // Raycast check
				hit.collider.gameObject == gameObject) //&& // Raycast hit this object
			//hit.distance <= maxDistanceToInteract)  // The player is in range 
			{
				// The player has the key card needed
				if (PlayerHasKeyCards() && !brokenTerminal)
				{
					// Unlock script
					if (doorToOpen.name == "BlastDoor")
					{
						doorToOpen.GetComponent<BlastDoor>().locked = false;
					}
					else
					{
						doorToOpen.GetComponent<TriggerScript>().locked = false;
					}
					// Unlock Animation
					gameObject.GetComponent<Animator>().SetTrigger("Unlock");
				}
				if (player.inventory.HasFlag(PlayerController.Inventory.SolderingIron) && brokenTerminal)
				{
					brokenTerminal = false;
					var em = ps.emission;
					em.enabled = false;
					ps.Stop();
					gameObject.GetComponent<Animator>().SetTrigger("FixTerminal");
				}
				UpdateDisplay();
			}
		}
	}

	private void UpdateDisplay()
	{
		if (brokenTerminal)
		{
			SetColours(new Color(84, 31, 81, 1));
		}
		else if (!PlayerHasKeyCards())
		{
			SetColours(new Color(161, 100, 16, 1));
		}
		else
		{
			SetColours(new Color(66, 94, 68, 1));
		}
	}

	private bool PlayerHasKeyCards()
	{
		return (player.inventory & neededKeyCard) == neededKeyCard;
	}

	private void SetColours(Color newColor)
	{
		// int id = Shader.PropertyToID("_EmissionColor");
		screenMaterial.SetColor("_EmissiveColor", newColor);
		screenMaterial.EnableKeyword("_EMISSION");
		// screenMaterial.color = newColor;
	}
}
