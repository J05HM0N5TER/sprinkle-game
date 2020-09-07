using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    [Tooltip("The object that will be animated")]
    public GameObject animatedObject;
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
    // Start is called before the first frame update
    void Start()
    {
       audio =  animatedObject.GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!locked)
        {
            if (!hasplayedonce)
            {
                if(other.tag == "Player" || other.tag == "Enemy")
                {
                    animatedObject.GetComponent<Animator>().SetTrigger("Open");
                    audio.PlayOneShot(Openclip);
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
                    animatedObject.GetComponent<Animator>().SetTrigger("Close");
                    audio.PlayOneShot(Closeclip);
                    if (oneTimeUse)
                    {
                        hasplayedonce = true;
                    }
                }     
            }
        }  
    }
}
