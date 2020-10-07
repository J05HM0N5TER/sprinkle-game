using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastDoor : MonoBehaviour
{
    public bool locked = true;
    public AudioClip Openclip;
    private AudioSource audioS;
    // Start is called before the first frame update
    void Start()
    {
        audioS = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!locked)
        {
            gameObject.GetComponent<Animator>().SetTrigger("Open");
            audioS.PlayOneShot(Openclip);
            gameObject.GetComponent<BlastDoor>().enabled = false;
        }
    }
}
