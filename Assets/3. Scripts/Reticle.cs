using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    public GameObject reticleSprite;
    private Camera playerCamera;
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
            Debug.Log("didnt hit shit");
        }
        else//(RayOut.collider.gameObject.layer == LayerMask.NameToLayer("Dynamic"))
        {
            reticleSprite.SetActive(true);
            Debug.Log("dynamic hit "+ RayOut.transform.gameObject.name);
        }
        //else
        //{
        //    reticleSprite.SetActive(false);
        //    Debug.Log("hit " + RayOut.transform.gameObject.name);
        //}
    }
}
