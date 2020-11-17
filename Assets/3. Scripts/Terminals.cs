using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
public class Terminals : MonoBehaviour
{
	[Tooltip("The door that this terminal will open")]
	public GameObject doorToOpen;
	[Tooltip("They keycard that is needed to use the terminal")]
	public PlayerController.Inventory neededKeyCard;
	private PlayerController playerinv;
	[Tooltip("the max distance that the player can be from the terminal and still interact with it")]
	public float maxDistanceToInteract = 2;
	[Tooltip("Does this terminal require fixing before use?")]
	public bool brokenTerminal = false;
	[Tooltip("How bright the emmision is")]
	[Range(0.001f, 0.005f)]
	public float emissionIntencity = 0.001f;
	[Tooltip("Spark Particles")]
	public ParticleSystem brokenParticles;
	[Tooltip("The material that is active on the display when the terminal is broken")]
	public Material brokenMaterial;
	[Tooltip("The material that is active on the display when the terminal is unlocked")]
	public Material unlockedMaterial;
	[Tooltip("The material that is active on the display when the terminal is locked")]

	public Material lockedMaterial;
	public Material screenMaterial;
	private CameraControl cameraControl;
	private TextMeshPro screentext;
	public float textFadeOutTime = 2;
	[Header("Debug")]
	[Tooltip("Manually updates the material on the display, this is to show changes made in inspector without player interacting with it (this is a button)")]
	public bool manuallyUpdateDisplay = false;

	// Start is called before the first frame update
	void Start()
	{
		cameraControl = FindObjectOfType<CameraControl>();

		playerinv = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
		var em = brokenParticles.emission;
		if (brokenTerminal)
		{
			brokenParticles.Play();
			em.enabled = true;
		}
		else
		{
			em.enabled = false;
			brokenParticles.Stop();
		}
		screentext = gameObject.GetComponentInChildren<TextMeshPro>();
		UpdateDisplay();

		// screenMaterial.EnableKeyword("_EMISSION");

	}

	// Update is called once per frame
	void Update()
	{
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
					//gameObject.GetComponent<Animator>().SetTrigger("Unlock");
					screentext.text = "Unlocked";
					textFadeout();
				}
				if (playerinv.inventory.HasFlag(PlayerController.Inventory.SolderingIron) && brokenTerminal)
				{
					brokenTerminal = false;
					// var em = brokenParticles.emission;
					// em.enabled = false;
					// brokenParticles.Stop();
					//gameObject.GetComponent<Animator>().SetTrigger("FixTerminal");
				}
				if (!PlayerHasKeyCards() && !brokenTerminal)
				{
					screentext.text = neededKeyCard.ToString();
					textFadeout();
				}

				UpdateDisplay();
			}
		}
		if (manuallyUpdateDisplay)
		{
			UpdateDisplay();
			manuallyUpdateDisplay = false;
		}
	}

	/// <summary>
	/// Updates the terminal display, only needs to be called when the state changes
	/// </summary>
	private void UpdateDisplay()
	{
		// Get the array of all the materials on the renderer

		ParticleSystem.EmissionModule emission = brokenParticles.emission;

		//if borked
		if (brokenTerminal)
		{
			SetScreenEmissive(brokenMaterial);
			emission.enabled = true;
			brokenParticles.Play();
		}

		else
		{
			// If locked
			BlastDoor blastDoor = doorToOpen.GetComponent<BlastDoor>();
			TriggerScript triggerScript = doorToOpen.GetComponent<TriggerScript>();
			if ((blastDoor && blastDoor.locked) || (triggerScript && triggerScript.locked))
			{
				SetScreenEmissive(lockedMaterial);
				emission.enabled = false;
				brokenParticles.Stop();
			}
			// If unlocked
			else
			{
				SetScreenEmissive(unlockedMaterial);
				emission.enabled = false;
				brokenParticles.Stop();
			}
		}

		// ren.SetPropertyBlock(propertyBlock, 1);
	}

	/// <summary>
	/// Set the emmision colour of the screen
	/// </summary>
	/// <param name="newmat">The material to get the emission colour from</param>
	private void SetScreenEmissive(Material newmat)
	{
		Renderer ren = gameObject.GetComponent<Renderer>();
		MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
		screenMaterial.EnableKeyword("_EMISSION");
		propertyBlock.SetColor("_EmissiveColor", newmat.GetColor("_EmissiveColor") * emissionIntencity);
		ren.SetPropertyBlock(propertyBlock, 1);
	}

	private bool PlayerHasKeyCards()
	{
		return (playerinv.inventory & neededKeyCard) == neededKeyCard;
	}
	private void textFadeout()
	{
		Debug.Log("textfadeout called");
		StartCoroutine("fade");
	}
	private IEnumerator fade()
	{
		yield return new WaitForSeconds(textFadeOutTime);
		screentext.text = "";
	}
}
