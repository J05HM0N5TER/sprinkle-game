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
    // Start is called before the first frame update
    void Start()
    {
       audio =  animatedObject.GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!oneTimeUse)
        {
            animatedObject.GetComponent<Animator>().SetTrigger("Open");
            audio.PlayOneShot(Openclip);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!oneTimeUse)
        {
            animatedObject.GetComponent<Animator>().SetTrigger("Close");
            audio.PlayOneShot(Closeclip);
        }
    }
}
