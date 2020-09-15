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
    [TextAreaAttribute(5, 40)]
    public string inscription = "";
    [HideInInspector] public bool isActive = false;
    [Tooltip("the text mesh pro gameobject attached to the canvas that is to show the note pad text. this needs to be active for start up to show")]
    public GameObject noteTextBox;
    [Tooltip("the other thing that is to be deactivated, this also needs to be active on the start of the game")]
    public GameObject otherThingToDisable;
    [Tooltip("The position on the screen where it detects click at (decimal percentage)")]
	private Vector2 cursorPosition = new Vector2(0.5f, 0.5f);
	public string redicleName = "Reticle";
    private RectTransform redicle;


    // Start is called before the first frame update
    void Start()
    {
        redicle = GameObject.Find(redicleName).GetComponent<RectTransform>();
        //noteTextBox = GameObject.Find(nameOfNoteTextBox);
        
        //otherThingToDisable = GameObject.Find(nameotherThingToDisable);
        if(noteTextBox == null)
        {
            Debug.LogWarning("couldnt find notetextbox");
        }
        // TODO: Check for valid values
        noteTextBox.SetActive(false);
        otherThingToDisable.SetActive(false);


#if UNITY_EDITOR
        if (redicle == null)
            Debug.LogWarning("No Redicle found", this);
#endif
        cursorPosition = new Vector2(redicle.position.x / Screen.width, redicle.position.y / Screen.height);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Interact"))
        {
#if UNITY_EDITOR
            cursorPosition = new Vector2(redicle.position.x / Screen.width, redicle.position.y / Screen.height);
#endif
            // TODO: Make click position ajustable
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(cursorPosition.x * Screen.width, cursorPosition.y * Screen.height));

			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				Debug.Log(hit.collider.gameObject.name);
				if (hit.collider.gameObject == gameObject)
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


