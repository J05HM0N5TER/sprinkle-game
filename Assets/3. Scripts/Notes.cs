using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Notes : MonoBehaviour
{
    [TextAreaAttribute(5, 40)]
    public string inscription = "";
    [HideInInspector] public bool isActive = false;
    [Tooltip("the text mesh pro gameobject attached to the canvas that is to show the note pad text. this needs to be active for start up to show")]
    public string nameOfNoteTextBox;
    [Tooltip("the other thing that is to be deactivated, this also needs to be active on the start of the game")]
    public string nameotherThingToDisable;
    
    private GameObject noteTextBox;
    private GameObject otherThingToDisable;
    RaycastHit hit;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        noteTextBox = GameObject.Find(nameOfNoteTextBox);
        
        otherThingToDisable = GameObject.Find(nameotherThingToDisable);
        if(noteTextBox == null)
        {
            Debug.Log("couldnt find notetextbox");
        }
        noteTextBox.SetActive(false);
        otherThingToDisable.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.name);
                if(hit.collider.gameObject == gameObject)
                {
                    noteTextBox.SetActive(true);
                    otherThingToDisable.SetActive(true);
                    noteTextBox.GetComponent<TextMeshProUGUI>().enabled = true;
                    noteTextBox.GetComponent<TextMeshProUGUI>().text = inscription;
                    FindObjectOfType<PauseMenu>().PauseGame();
                    FindObjectOfType<PauseMenu>().pauseMenu.SetActive(false);
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                    isActive = true;
                }
            } 
        }
        if (Input.GetButtonDown("Pause") && isActive)
        {
            isActive = false;
            noteTextBox.SetActive(false);
            otherThingToDisable.SetActive(false);
            noteTextBox.GetComponent<TextMeshProUGUI>().enabled = false;
            noteTextBox.GetComponent<TextMeshProUGUI>().text = " ";
            FindObjectOfType<PauseMenu>().ResumeGame();

        }
    }
    
}


