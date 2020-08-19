using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notes : MonoBehaviour
{
    public string inscription = "";
    [HideInInspector] public bool isActive = false;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            player.GetComponentInChildren<TextMeshPro>().text = inscription;
        }
        if(!isActive)
        {
            player.GetComponentInChildren<TextMeshPro>().text = null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        isActive = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isActive = false;
    }
}
