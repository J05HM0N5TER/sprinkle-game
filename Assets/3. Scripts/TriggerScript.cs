using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public GameObject animatedObject;
    private Animation refToAnimation;
    public AnimationClip[] clip;
    int currentClip = 0;
    // Start is called before the first frame update
    void Start()
    {
        //animatedObject.GetComponent<Animation>();
        //refToAnimation = animatedObject.GetComponent<Animation>();
        //refToAnimation["Open"].layer = 0;
    }

    // Update is called once per frame
    void Update()
    {   
        
    }
    private void OnTriggerEnter(Collider other)
    {
        //refToAnimation.Play();
        animatedObject.GetComponent<Animator>().SetTrigger("Open");
    }
    private void OnTriggerExit(Collider other)
    {
        animatedObject.GetComponent<Animator>().SetTrigger("Close");
    }
}
