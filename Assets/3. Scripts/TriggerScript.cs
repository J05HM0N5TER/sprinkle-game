using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UIElements;

public class TriggerScript : MonoBehaviour
{
    [Tooltip("The object that will be animated")]
    public GameObject animatedObject;
    [Tooltip("The object that will make sound")]
    public GameObject soundSource;
    private AudioSource audio;
    [Tooltip("The open sound of the doors")]
    public AudioClip Openclip;
    [Tooltip("The close sound of the doors")]
    public AudioClip Closeclip;
    [Tooltip("Can only be used ones, false = no, true = yes")]
    public bool oneTimeUse = false;
    [Tooltip("is the door locked")]
    public bool locked = false;
    private bool hasplayedonce;
    private bool keepDoorsOpen;

    private List<GameObject> thingsInDoorway = new List<GameObject>();
    [Tooltip("time till door can close again")]
    public float timer = 5.0f;
    private float resettimer;
    private bool doorOpen;
    // Start is called before the first frame update
    void Start()
    {
       audio = soundSource.GetComponent<AudioSource>();
       resettimer = timer;
    }
    private void Update()
    {
        timer -= Time.deltaTime;
        if (thingsInDoorway.Count == 0 && timer <= 0 && doorOpen)
        {
            animatedObject.GetComponent<Animator>().SetTrigger("Close");
            audio.PlayOneShot(Closeclip);
            doorOpen = false;
            if (oneTimeUse)
            {
                hasplayedonce = true;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!locked)
        {
            if (!hasplayedonce)
            {
                if(other.tag == "Player" || other.tag == "Enemy")
                {
                    if(!doorOpen)
                    {
                        animatedObject.GetComponent<Animator>().SetTrigger("Open");
                        audio.PlayOneShot(Openclip);
                        timer = resettimer;
                    }
                    thingsInDoorway.Add(other.gameObject);
                    doorOpen = true;
                    if(oneTimeUse)
                    {
                        hasplayedonce = true;
                    }
                }
            }
        } 
    }
    private void OnTriggerExit(Collider other)
    {
        if(!locked)
        {
            if (!hasplayedonce)
            {
                if (other.tag == "Player" || other.tag == "Enemy")
                {
                    thingsInDoorway.Remove(other.gameObject);
                    
                }     
            }
        }  
    }
}
