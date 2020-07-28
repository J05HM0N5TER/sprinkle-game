﻿using System;
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
	private float standHeight;
	private CapsuleCollider capsule;

	[Header("Controls:")]
	public KeyCode left = KeyCode.A;
	public KeyCode right = KeyCode.D;
	public KeyCode forwards = KeyCode.W;
	public KeyCode backwards = KeyCode.S;
	public KeyCode crouch = KeyCode.LeftShift;
	public KeyCode jump = KeyCode.Space;
	// Start is called before the first frame update
	void Start()
	{
		rb = gameObject.GetComponent<Rigidbody>();
		groundcheck.GetComponent<GameObject>();
		capsule = GetComponent<CapsuleCollider>();
		standHeight = capsule.height;
	}

	// Update is called once per frame
	void Update()
	{		
		if (Input.GetKey(forwards))
		{
			rb.AddForce(gameObject.transform.forward * speed);
		}
		if (Input.GetKey(left))
		{
			rb.AddForce(gameObject.transform.right * -speed);
		}
		if (Input.GetKey(backwards))
		{
			rb.AddForce(gameObject.transform.forward * -speed);
		}
		if (Input.GetKey(right))
		{
			rb.AddForce(gameObject.transform.right * speed);
		}
		if (Input.GetKey(jump) && Physics.CheckSphere(groundcheck.transform.position, ground_distance, ground_mask))
		{
			rb.AddForce (gameObject.transform.up * Jump_height);
		}
		if (Input.GetKeyDown(crouch))
		{

			// If the player is standing then make them crouch otherwise make them stand
			if (capsule.height == standHeight)
            {
                capsule.height = crouchHeight;
            }
            else if (!Physics.Raycast(transform.position, Vector3.up,
				// Distance is how much difference between current height and stand height
				standHeight - (capsule.radius < crouchHeight ? crouchHeight : capsule.radius))) 
			{
				capsule.height = standHeight;

			}
		}
	}
}
