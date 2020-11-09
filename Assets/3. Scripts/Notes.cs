using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Notes : MonoBehaviour
{
	[TextArea(5, 40)]
	public string inscription = "";
	[HideInInspector] public bool isActive = false;

	[Tooltip("The name of the text box that is being used as the display")]
	public string noteTextBoxName;
	// The text box that the text is put into
	private GameObject noteTextBox;

	[Tooltip("All of the other things that will be disabled other then when a note is being looked at")]
	public string[] otherNoteObjectsNames;
	// All of the other things that will be disabled other then when a note is being looked at
	private List<GameObject> otherNoteObjects;

	// Cache 
	TextMeshProUGUI textBoxText;
	PauseMenu pauseManager;

	bool isFirstFrame = true;
	[Tooltip("How close you have to be to the note to interact with it")]
	public float maxInteractDistance = 1;
	CameraControl playerCamera;

	// Start is called before the first frame update
	void Start()
	{
		playerCamera = FindObjectOfType<CameraControl>();
		otherNoteObjects = new List<GameObject>();
		noteTextBox = GameObject.Find(noteTextBoxName);
		if (noteTextBox == null)
			Debug.LogError($"Text box of name \"{noteTextBoxName}\" was not found", this);
		textBoxText = noteTextBox.GetComponent<TextMeshProUGUI>();
		pauseManager = FindObjectOfType<PauseMenu>();
		textBoxText.enabled = false;

		foreach (var name in otherNoteObjectsNames)
		{
			GameObject newObject = GameObject.Find(name);

			// Only in editor check and warn if not filled out properly in inspector
#if UNITY_EDITOR
			if (newObject == null)
				Debug.LogWarning($"\"{name}\" was not found", this);
#endif
			otherNoteObjects.Add(newObject);
		}

		// Only in editor check and warn if not filled out properly in inspector
#if UNITY_EDITOR
		if (noteTextBox == null)
			Debug.LogError($"Text box of name \"{noteTextBoxName}\" was not found", this);
#endif
	}

	// Update is called once per frame
	void Update()
	{
		// Disable the note on the first frame, this is because 
		// 		finding by name doesn't work for disabled objects
		if (isFirstFrame)
		{
			if (noteTextBox == null)
			{
				Debug.LogError("No text box", this);
			}
			Debug.Log("Disabling text", this);

			foreach (var obj in otherNoteObjects)
			{
				obj.SetActive(false);
			}
			isFirstFrame = false;
		}

		if (Input.GetButtonDown("Interact"))
		{
			// Only in editor update reticle position every click

			if (Physics.Raycast(playerCamera.CursorToRay(), out RaycastHit hit, maxInteractDistance))
			{
				if (hit.collider.gameObject == gameObject)
				{
					noteTextBox.SetActive(true);
					foreach (var item in otherNoteObjects)
					{
						item.SetActive(true);
					}
					textBoxText.enabled = true;
					textBoxText.text = inscription;
					pauseManager.PauseGame();
					pauseManager.pauseMenu.SetActive(false);
					UnityEngine.Cursor.lockState = CursorLockMode.None;
					isActive = true;
				}
			}
		}
		if (Input.GetButtonDown("Pause") && isActive)
		{
			isActive = false;
			noteTextBox.SetActive(false);
			foreach (var item in otherNoteObjects)
			{
				item.SetActive(false);
			}
			textBoxText.enabled = false;
			textBoxText.text = " ";
			pauseManager.ResumeGame();

		}
	}
}
