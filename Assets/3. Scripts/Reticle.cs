using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    public GameObject reticleSprite;
    private Camera playerCamera;
    public float grabDistance = 1;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Camera>();
        reticleSprite.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f)), out RaycastHit RayOut, grabDistance);
        if(RayOut.collider.GetComponent<LayerMask>() == LayerMask.NameToLayer("Dynamic"))
        {

        }
    }
}
