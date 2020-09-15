using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DumbPlayerTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject player;
    [TextAreaAttribute(5, 40)]
    public string tellPlayerToMoveText = "";
    public GameObject textbox;
    public bool testIfPlayerhasMoved = false;
    public float timeUntilPopUp = 10;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        textbox.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetComponent<Rigidbody>().velocity.magnitude <= 1 && !testIfPlayerhasMoved)
        {
            timeUntilPopUp -= Time.deltaTime;
            if(timeUntilPopUp <= 0)
            {
                textbox.SetActive(true);
                textbox.GetComponent<TextMeshProUGUI>().text = tellPlayerToMoveText;
            }
        }
        else if(player.GetComponent<Rigidbody>().velocity.magnitude > 1)
        {
            testIfPlayerhasMoved = true;
            textbox.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
