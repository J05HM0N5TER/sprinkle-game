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
    private GameObject player;
    RaycastHit hit;
    [Tooltip("This is to make sure that the notes do not activate and deactivate at the same time. low number above 0.1 should be fine.")]
    public float bufferTimer = 0.2f; // as the notes will deactive and activate at the same time as a button press happens over multiple frames this should allow it to have time between.
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
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
                    player.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                    player.GetComponentInChildren<TextMeshProUGUI>().text = inscription;
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
            player.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            player.GetComponentInChildren<TextMeshProUGUI>().text = " ";
            FindObjectOfType<PauseMenu>().ResumeGame();

        }
    }
    
}


