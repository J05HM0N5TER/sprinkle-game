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

	[Tooltip("The position on the screen where it detects click at (decimal percentage)")]
	public string redicleName = "Reticle";
	// The info from the redicle used to calculate where to click
	private RectTransform redicle;
	// Same as previous
	private Vector2 cursorPosition = new Vector2(0.5f, 0.5f);

	// Cashe
	TextMeshProUGUI textBoxText;
	PauseMenu pauseManager;

	// Start is called before the first frame update
	void Start()
	{
		otherNoteObjects = new List<GameObject>();
		redicle = GameObject.Find(redicleName).GetComponent<RectTransform>();
		noteTextBox = GameObject.Find(noteTextBoxName);
		textBoxText = noteTextBox.GetComponent<TextMeshProUGUI>();
		pauseManager = FindObjectOfType<PauseMenu>();

		// TODO: Check for valid values
		noteTextBox.SetActive(false);

		foreach (var name in otherNoteObjectsNames)
		{
			GameObject newObect = GameObject.Find(name);

			// Only in editor check and warn if not filled out properly in inspector
#if UNITY_EDITOR
			if (newObect == null)
				Debug.LogWarning($"\"{name}\" was not found", this);
#endif
			newObect.SetActive(false);
			otherNoteObjects.Add(newObect);
		}



		// Only in editor check and warn if not filled out properly in inspector
#if UNITY_EDITOR
		if (redicle == null)
			Debug.LogWarning($"No Redicle of name \"{redicleName}\" was found", this);
		if (noteTextBox == null)
			Debug.LogWarning($"Text box of name \"{noteTextBoxName}\" was not found", this);
#endif
		cursorPosition = new Vector2(redicle.position.x / Screen.width, redicle.position.y / Screen.height);
	}

	// Update is called once per frame
	void Update()
	{

		if (Input.GetButtonDown("Interact"))
		{
			// Only in editor update redicle position every click
#if UNITY_EDITOR
			cursorPosition = new Vector2(redicle.position.x / Screen.width, redicle.position.y / Screen.height);
#endif
			// TODO: Make click position ajustable
			Ray ray = Camera.main.ScreenPointToRay(new Vector2(cursorPosition.x * Screen.width, cursorPosition.y * Screen.height));

			if (Physics.Raycast(ray, out RaycastHit hit))
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
					UnityEngine.Cursor.lockState = CursorLockMode.Locked;
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


