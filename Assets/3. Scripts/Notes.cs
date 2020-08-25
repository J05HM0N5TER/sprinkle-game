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
    public string inscription = "";
    [HideInInspector] public bool isActive = false;
    [Tooltip("the text mesh pro gameobject attached to the canvas that is to show the note pad text")]
    public GameObject noteTextBox;
    RaycastHit hit;
    
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.gameObject.name);
                if(hit.collider.gameObject == gameObject)
                {
                    noteTextBox.SetActive(true);
                    noteTextBox.GetComponent<TextMeshProUGUI>().enabled = true;
                    noteTextBox.GetComponent<TextMeshProUGUI>().text = inscription;
                    FindObjectOfType<PauseMenu>().PauseGame();
                    FindObjectOfType<PauseMenu>().pauseMenu.SetActive(false);
                    UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                    isActive = true;
                }
            } 
        }
        if (Input.GetKeyDown(KeyCode.E) && isActive)
        {
            isActive = false;
            noteTextBox.SetActive(false);
            noteTextBox.GetComponent<TextMeshProUGUI>().enabled = false;
            noteTextBox.GetComponent<TextMeshProUGUI>().text = " ";
            FindObjectOfType<PauseMenu>().ResumeGame();

        }
    }
    
}


