using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{
    public CharacterController Controller;

    public float speed = 12f;
    public float Gravity = -9.81f;

    public Transform Ground_check;
    public float ground_distance = 0.4f;
    public LayerMask ground_mask;
    public float Jump_height = 3;
    Vector3 Velocity;
    bool is_grounded;
    
    
    // animation stuff
    //public Animator anim;

    private void Start()
    {
        //anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        is_grounded = Physics.CheckSphere(Ground_check.position, ground_distance, ground_mask);

        if(is_grounded && Velocity.y < 0)
        {
            Velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;


        

       
        Controller.Move(move * speed * Time.deltaTime);
        
        if(Input.GetButtonDown("Jump")&& is_grounded)
        {
            Velocity.y = Mathf.Sqrt(Jump_height * -2f * Gravity);
        }

        Velocity.y += Gravity * Time.deltaTime;

        Controller.Move(Velocity * Time.deltaTime);

        //if (Input.GetKey(KeyCode.W))
        //{
        //    //anim.SetBool("Run", true);
        //}
        //else
        //{
        //    //anim.SetBool("Run", false);
        //}
         
    }
}
