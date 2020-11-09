using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    [Tooltip("The object on the canvas that has the reticle sprite")]
    public GameObject lootIcon;
    public GameObject pressIcon;
    public GameObject grabIcon;
    private CameraControl cameraControl;
    private Camera playerCamera;
    [Tooltip("Should be the same as the grabbing distance, just the distance that the ray goes to check whats in front")]
    public float grabDistance = 30;
    // Start is called before the first frame update
    void Start()
    {
        cameraControl = FindObjectOfType<CameraControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(cameraControl.CursorToRay(), out RaycastHit hit, grabDistance))
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
            else 
            {
                lootIcon.SetActive(false);
                pressIcon.SetActive(false);
                grabIcon.SetActive(false);
            }
        }
        else
        {
            lootIcon.SetActive(false);
            pressIcon.SetActive(false);
            grabIcon.SetActive(false);
        }
    }
}
