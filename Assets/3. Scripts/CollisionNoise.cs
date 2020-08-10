using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class CollisionNoise : MonoBehaviour
{
    //audio, 
    RequireComponent AudioSource; // component needed
    AudioSource audioSource;
    public AudioClip clip; // actual sound
    public float volume; // volume for later use
    public float volumeDropOff; // drop off for later use

    public float minSpeedToMakeSound = 1.0f; // speed that is needed to make sound
    public bool madeSound = false; // debug checking if sound was made
    public float timer = 1.0f; // time until the debug is changed and to get rid of the object from list so ai doesnt always get stuck on it forever
    

    private GameObject livingSuit; 
    private Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        livingSuit = GameObject.Find("LivingArmour");
        livingSuit.GetComponent<LivingArmourAI>();
        rb = GetComponent<Rigidbody>();
        
    }
    private IEnumerator OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude >= minSpeedToMakeSound)
        { 
            audioSource.PlayOneShot(clip);
            madeSound = true;
            livingSuit.GetComponent<LivingArmourAI>().soundSources.Add(gameObject);
            yield return StartCoroutine("removeFromlist");
        }
    }
    private IEnumerator removeFromlist()
    {
        yield return new WaitForSeconds(timer);
        madeSound = false;
        livingSuit.GetComponent<LivingArmourAI>().soundSources.Remove(gameObject);
    }
}
