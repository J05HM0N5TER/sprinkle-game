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
	public bool brokenTerminal = false;
	[Tooltip("Spark Particles")]
	public GameObject brokenParticles;
	private ParticleSystem ps;
	public Material screenMaterial;
	public Material brokenMaterial;
	public Material unlockedMaterial;
	public Material lockedMaterial;
	public Material[] mats;
	private CameraControl cameraControl;
	private TextMeshPro screentext;
	public float textFadeOutTime = 2;

	// Start is called before the first frame update
	void Start()
	{
		cameraControl = FindObjectOfType<CameraControl>();

		playerinv = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
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
		screentext = gameObject.GetComponentInChildren<TextMeshPro>();

	}

	// Update is called once per frame
	void Update()
	{
		if (screenMaterial == null)
		{
			return;
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
					//gameObject.GetComponent<Animator>().SetTrigger("Unlock");
					screentext.text = "Unlocked";
					textFadeout();
				}
				if (playerinv.inventory.HasFlag(PlayerController.Inventory.SolderingIron) && brokenTerminal)
				{
					brokenTerminal = false;
					var em = ps.emission;
					em.enabled = false;
					ps.Stop();
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
		UpdateDisplay();
	}

	private void UpdateDisplay()
	{
		// Get the array of all the materials on the renderer
		Renderer ren = gameObject.GetComponent<Renderer>();
		// Material[] copiedMaterials = new Material[ren.materials.Length];

		mats = ren.materials;
		// Profiler.BeginSample("UpdateDisplay");
		// Profiler
		//if borked
		if (brokenTerminal)
		{
			// You have to destoy any that you replace becuase GC doesn't take care
			// of it "It is your responsibility to destroy the materials when the
			// game object is being destroyed"
			// https://docs.unity3d.com/ScriptReference/Renderer-material.html;
			Destroy(mats[1]);
			mats[1] = brokenMaterial;
			ren.materials = mats;
		}
		//if unlocked
		else
		{
			// Destroy(mats[0]);
			Destroy(mats[1]);
			mats[1] = unlockedMaterial;
			ren.materials = mats;

			if (doorToOpen.name == "BlastDoor")
			{
				if (doorToOpen.GetComponent<BlastDoor>().locked == true)
				{
					// Destroy(mats[0]);
					Destroy(mats[1]);
					mats[1] = lockedMaterial;
					ren.materials = mats;
				}
			}
			else
			{
				if (doorToOpen.GetComponent<TriggerScript>().locked == true)
				{
					// Destroy(mats[0]);
					Destroy(mats[1]);
					mats[1] = lockedMaterial;
					ren.materials = mats;
				}
			}
		}

	}

	private bool PlayerHasKeyCards()
	{
		return (playerinv.inventory & neededKeyCard) == neededKeyCard;
	}

	private void SetColours(Color newColor)
	{
		// int id = Shader.PropertyToID("_EmissionColor");
		screenMaterial.SetColor("_EmissiveColor", newColor);
		screenMaterial.EnableKeyword("_EMISSION");
		// screenMaterial.color = newColor;
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
