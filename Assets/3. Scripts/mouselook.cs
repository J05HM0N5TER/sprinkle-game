using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouselook : MonoBehaviour
{
    public float mouseSen = 100f;


    public Transform PlayerBody;
    private float Xrotation = 0f;
    // ray casting
    //int layermask = 1 << 10;
    private GameObject hitobject;
    RaycastHit hit;
    private bool object_being_look_at;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mousex = Input.GetAxis("Mouse X") * mouseSen * Time.deltaTime;
        float mousey = Input.GetAxis("Mouse Y") * mouseSen * Time.deltaTime;

        Xrotation -= mousey;
        Xrotation = Mathf.Clamp(Xrotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(Xrotation, 0, 0);
        PlayerBody.Rotate(Vector3.up * mousex);
        if (Input.GetKey(KeyCode.Q))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetKey(KeyCode.E))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        // ray casting
        //if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layermask))
        //{
        //    //Debug.Log("did hit");
        //    object_being_look_at = true;
            
        //    hitobject = (hit.collider.gameObject);
        //    hitobject.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.green);
          
        //}
        //else
        //{
        //    object_being_look_at = false;
        //}
        //if(object_being_look_at == false)
        //{
        //    hitobject.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
        //}
        

        




    }
}
