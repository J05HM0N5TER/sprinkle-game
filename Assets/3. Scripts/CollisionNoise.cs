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
    [Tooltip("The sound that will be played when hitting something")]
    public AudioClip clip; // actual sound
    [Tooltip("The volume of the sound")]
    public float volume; // volume for later use
    [Tooltip("Drop off rate of the sound (further that it is the quieter it becomes)")]
    public float volumeDropOff; // drop off for later use
    [Tooltip("Minimum speed something must go to be able to create noise")]
    public float minSpeedToMakeSound = 1.0f; // speed that is needed to make sound
    [Header("DEBUG")]
    [Tooltip("Debug, did this thing make noise")]
    public bool madeSound = false; // debug checking if sound was made
    [Tooltip("Debug,timer to remove object from list of places of interest, this is to give time for AI script to detect and use and calculate")]
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
    /// <summary>
    /// on collision, play a sound and add the object that made the sound to the list of places of interest for the ai to go and walk to.
    /// </summary>
    /// <param name="collision"></param>
    /// <returns></returns>
    private IEnumerator OnCollisionEnter(Collision collision)
    {
        if(collision.relativeVelocity.magnitude >= minSpeedToMakeSound)
        { 
            audioSource.PlayOneShot(clip);
            madeSound = true;
            livingSuit.GetComponent<LivingArmourAI>().soundSources.Add(gameObject.transform.position);
            yield return StartCoroutine("removeFromlist");
        }
    }
    /// <summary>
    /// remove the object that made sound for the list so that it is no longer a spot of interest.
    /// </summary>
    /// <returns></returns>
    private IEnumerator removeFromlist()
    {
        yield return new WaitForSeconds(timer);
        madeSound = false;
        livingSuit.GetComponent<LivingArmourAI>().soundSources.Remove(gameObject.transform.position);
    }
}
