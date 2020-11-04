using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    [Tooltip("The object on the canvas that has the reticle sprite")]
    public GameObject reticleSprite;
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
        reticleSprite.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f)), out RaycastHit RayOut, grabDistance, 1 << 11);
        if(RayOut.collider == null)
        {
            reticleSprite.SetActive(false);
            lootIcon.SetActive(false);
            pressIcon.SetActive(false);
            grabIcon.SetActive(false);
            //Debug.Log("didnt hit shit");
        }
        else//(RayOut.collider.gameObject.layer == LayerMask.NameToLayer("Dynamic"))
        {
            reticleSprite.SetActive(true);
            //Debug.Log("dynamic hit "+ RayOut.transform.gameObject.name);
        }
        if(RayOut.collider.tag == "Keyitem")
        {
            lootIcon.SetActive(true);
        }
        if(RayOut.collider.tag == "interact")
        {
            pressIcon.SetActive(true);
        }
        if(RayOut.collider.tag == "Dynamic")
        {
            grabIcon.SetActive(true);
        }
    }
}
