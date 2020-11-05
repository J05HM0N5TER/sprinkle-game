﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    [Tooltip("The object on the canvas that has the reticle sprite")]
    public GameObject lootIcon;
    public GameObject pressIcon;
    public GameObject grabIcon;
    private Camera playerCamera;
    [Tooltip("Should be the same as the grabbing distance, just the distance that the ray goes to check whats in front")]
    public float grabDistance = 30;
    // Start is called before the first frame update
    void Start()
    {
        playerCamera = gameObject.GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance))
        {
            if(hit.collider.gameObject.tag == "Keyitem")
            {
                lootIcon.SetActive(true);
            }
            else if(hit.collider.gameObject.tag == "Interact")
            {
                pressIcon.SetActive(true);
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Dynamic"))
            {
                grabIcon.SetActive(true);
            }
        }
        else
        {
            lootIcon.SetActive(false);
            pressIcon.SetActive(false);
            grabIcon.SetActive(false);
        }
        // if(RayOut.collider == null)
        // {
        //     lootIcon.SetActive(false);
        //     pressIcon.SetActive(false);
        //     grabIcon.SetActive(false);
        //     //Debug.Log("didnt hit shit");
        // }
        // if (Physics.Raycast(ray, out RaycastHit hit, grabDistance) && hit.collider.gameObject.tag == "keyitem")
        // {
        //     lootIcon.SetActive(true);
        // }
        // if (Physics.Raycast(ray, out RaycastHit hit, grabDistance) && hit.collider.gameObject.tag == "Interact")
        // {
        //     pressIcon.SetActive(true);
        // }
        // if (Physics.Raycast(ray, out RaycastHit hit, grabDistance) && hit.collider.gameObject.tag == "keyitem")
        // {

        // }
        // if(RayOut.collider. == "Keyitem")
        // {
        //     lootIcon.SetActive(true);
        // }
        // else if(RayOut.collider.gameObject.layer == LayerMask.NameToLayer("Interact"))
        // {
        //     pressIcon.SetActive(true);
        // }
        // else if (RayOut.collider.gameObject.layer == LayerMask.NameToLayer("Dynamic"))
        // {
        //     grabIcon.SetActive(true);
        // }
        // else 
        // {
        //     return;
        // }
    }
}
