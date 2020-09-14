using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class tutorial : MonoBehaviour
{
    [Flags]public enum Tutorial : byte 
    { 
        none = 0,
        crouch = 1 << 0,
        interact = 1<< 1,
        leanRight = 1 << 2,
        leanLeft = 1 << 3,
        jump = 1 << 4,
        sprint = 1 << 5
    }
    [Tooltip("What button needs to pressed to 'complete' the tutorial")]
    public Tutorial tutorialButton;
    [TextAreaAttribute(5, 40)]
    public string tutorialText = "";
    [Tooltip("The box element that will display the text on the canvas")]
    public GameObject textbox;
    private string buttonToPress = "";
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        //checking which button has been selected for this trigger box
        switch(gameObject.GetComponent<tutorial>().tutorialButton.ToString())
        {
            case "crouch":
                buttonToPress = "Crouch";
                break;

            case "interact":
                buttonToPress = "Interact";
                break;

            case "leanRight":
                buttonToPress = "Lean Right";
                break;

            case "leanLeft":
                buttonToPress = "Lean Left";
                break;

            case "jump":
                buttonToPress = "Jump";
                break;

            case "sprint":
                buttonToPress = "Sprint";
                break;
        }

        textbox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //make sure it only pops up when player comes into the trigger box
        if(other == player)
        {
            textbox.SetActive(true);
            textbox.GetComponent<TextMeshProUGUI>().text = tutorialText;
            if(Input.GetButtonDown(buttonToPress))
            {
                textbox.SetActive(false);
                gameObject.SetActive(false);
            }
        }
    }



}
