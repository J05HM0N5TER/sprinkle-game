using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rigidbody_character_controller : MonoBehaviour
{
    public Rigidbody rb;
    public float speed  = 10.0f;
    public GameObject groundcheck;
    bool is_grounded;
    public float Jump_height = 15;
    public float ground_distance = 0.4f;
    public LayerMask ground_mask;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        groundcheck.GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //is_grounded = Physics.CheckSphere(groundcheck.transform.position, ground_distance, ground_mask);
        if(Physics.CheckSphere(groundcheck.transform.position, ground_distance, ground_mask))
        {
            is_grounded = true;

        }
        else
        {
            is_grounded = false;
        }
        
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(gameObject.transform.forward * speed);
            
           
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(gameObject.transform.right * -speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(gameObject.transform.forward * -speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(gameObject.transform.right * speed);
        }
        if (Input.GetKey(KeyCode.Space) && is_grounded)
        {
            rb.AddForce (gameObject.transform.up * Jump_height);
        }

    }
}
