using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class tutorial : MonoBehaviour
{
	public enum Tutorial : byte
	{
		none = 0,
		crouch,
		interact,
		lean,
		jump,
		sprint, 
		lantern,
        rotate,
        toss,
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
		switch (gameObject.GetComponent<tutorial>().tutorialButton)
		{
			case Tutorial.crouch:
				buttonToPress = "Crouch";
				break;

			case Tutorial.interact:
				buttonToPress = "Interact";
				break;

			case Tutorial.lean:
				buttonToPress = "Lean Right";
				altButtonToPress = "Lean Left";
				break;

			case Tutorial.jump:
				buttonToPress = "Jump";
				break;

			case Tutorial.sprint:
				buttonToPress = "Sprint";
				break;

			case Tutorial.lantern:
				buttonToPress = "Lantern";
				break;

            case Tutorial.rotate:
                buttonToPress = "Rotate Object";
                break;

            case Tutorial.toss:
                buttonToPress = "Throw object";
                break;

        }
		if(altButtonToPress == null)
		{
			altButtonToPress = "";
		}
	}
	private void Update()
	{
		if (hasEnteredTutorialBox && // Is the player in the correct position?
		(Input.GetButtonDown(buttonToPress) // Is the first button getting pressed?
		// If the second button is set is it being pressed down?
		|| !(altButtonToPress == "" || !Input.GetButtonDown(altButtonToPress))))
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
