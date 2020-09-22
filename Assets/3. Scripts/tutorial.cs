using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class tutorial : MonoBehaviour
{
	[Flags]
	public enum Tutorial : byte
	{
		none = 0,
		crouch = 1 << 0,
		interact = 1 << 1,
		lean = 1 << 2,
		jump = 1 << 3,
		sprint = 1 << 4
	}
	[Tooltip("What button needs to pressed to 'complete' the tutorial")]
	public Tutorial tutorialButton;
	[TextAreaAttribute(5, 40)]
	public string tutorialText = "";
	[Tooltip("The box element that will display the text on the canvas")]
	public GameObject textbox;
	private string buttonToPress = "";
	private string altButtonToPress = "";
	private bool hasEnteredTutorialBox;

	// Start is called before the first frame update
	void Start()
	{
		//checking which button has been selected for this trigger box
		switch (gameObject.GetComponent<tutorial>().tutorialButton.ToString())
		{
			case "crouch":
				buttonToPress = "Crouch";
				break;

			case "interact":
				buttonToPress = "Interact";
				break;

			case "lean":
				buttonToPress = "Lean Right";
				altButtonToPress = "Lean Left";
				break;

			case "jump":
				buttonToPress = "Jump";
				break;

			case "sprint":
				buttonToPress = "Sprint";
				break;
		}
	}
	private void Update()
	{
		if ((Input.GetButtonDown(buttonToPress) || Input.GetButtonDown(altButtonToPress)) && hasEnteredTutorialBox)
		{
			textbox.SetActive(false);
			gameObject.SetActive(false);
			hasEnteredTutorialBox = false;
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		//make sure it only pops up when player comes into the trigger box
		if (other.CompareTag("Player"))
		{
			textbox.SetActive(true);
			textbox.GetComponent<TextMeshProUGUI>().text = tutorialText;
			hasEnteredTutorialBox = true;
		}
	}

}
