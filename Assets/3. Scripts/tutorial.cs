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
    public Tutorial tutorialButton;
    private GameObject player;
    public string tutorialText = "";
    public GameObject textbox;
    private string buttonToPress = "";
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        if(gameObject.GetComponent<tutorial>().tutorialButton.ToString() == "crouch")
        {
            buttonToPress = "Crouch";
        }
        else if(gameObject.GetComponent<tutorial>().tutorialButton.ToString() == "interact")
        {
            buttonToPress = "Interact";
        }
        else if(gameObject.GetComponent<tutorial>().tutorialButton.ToString() == "leanRight")
        {
            buttonToPress = "Lean Right";
        }
        else if(gameObject.GetComponent<tutorial>().tutorialButton.ToString() == "leanLeft")
        {
            buttonToPress = "Lean Left";
        }
        else if(gameObject.GetComponent<tutorial>().tutorialButton.ToString() == "jump")
        {
            buttonToPress = "Jump";
        }
        else if(gameObject.GetComponent<tutorial>().tutorialButton.ToString() == "sprint")
        {
            buttonToPress = "Sprint";
        }

        textbox.SetActive(false);
        //if(gameObject.GetComponent<tutorial>().tutorialButton.ToString() == "crouch")
        //{
        //    buttonToPress = "Crouch";
        //}
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
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
