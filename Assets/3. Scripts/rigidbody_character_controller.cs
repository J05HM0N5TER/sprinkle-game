using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class rigidbody_character_controller : MonoBehaviour
{
	public Rigidbody rb;
	public GameObject groundcheck;
	public LayerMask ground_mask;
	public float speed  = 10.0f;
	public float ground_distance = 0.4f;
	public float Jump_height = 15;
	public float crouchHeight;
	private Vector3 defautScale;
	private float standHeight;
	private CapsuleCollider capsule;

	// Start is called before the first frame update
	void Start()
	{
		rb = gameObject.GetComponent<Rigidbody>();
		groundcheck.GetComponent<GameObject>();
		capsule = GetComponent<CapsuleCollider>();
		standHeight = capsule.height;
		defautScale = transform.localScale;
	}

	// Update is called once per frame
	void Update()
	{
		// Movement input
		Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		rb.AddForce(gameObject.transform.forward * input.y * speed);
		rb.AddForce(gameObject.transform.right * input.x * speed);


		if (Input.GetButton("Jump") && Physics.CheckSphere(groundcheck.transform.position, ground_distance, ground_mask))
		{
			rb.AddForce(gameObject.transform.up * Jump_height);
		}
		if (Input.GetButtonDown("Crouch"))
		{
			// If the player is standing then make them crouch otherwise make them stand
			if (transform.localScale == defautScale)
            {
				// Change the localScale of the gameObject so that the height is the crouch height
				gameObject.transform.localScale = new Vector3(transform.localScale.x, 
					defautScale.y / (standHeight / crouchHeight), transform.localScale.z);
				
			}
            else if (!Physics.Raycast(transform.position, Vector3.up,
				// Distance is how much difference between current height and stand height
				standHeight - (capsule.radius < crouchHeight ? crouchHeight : capsule.radius))) 
			{
				// When the player stands up reset the scale back to what it was at the start
				gameObject.transform.localScale = defautScale;

			}
		}
	}
}
