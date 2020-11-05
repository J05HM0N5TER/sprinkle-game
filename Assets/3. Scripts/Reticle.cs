using System.Collections;
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
        Physics.Raycast(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f)), out RaycastHit RayOut, grabDistance, 1 << 11);
        if(RayOut.collider == null)
        {
            lootIcon.SetActive(false);
            pressIcon.SetActive(false);
            grabIcon.SetActive(false);
            //Debug.Log("didnt hit shit");
        }
    
        if(RayOut.collider.gameObject.tag == "Keyitem")
        {
            lootIcon.SetActive(true);
        }
        if(RayOut.collider.gameObject.tag == "Interact")
        {
            pressIcon.SetActive(true);
        }
        if (RayOut.collider.gameObject.layer == LayerMask.NameToLayer("Dynamic"))
        {
            grabIcon.SetActive(true);
        }
    }
}
