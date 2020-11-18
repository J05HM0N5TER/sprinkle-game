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
    [HideInInspector] public AudioSource audioSource;
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
    private Rigidbody rb;
    // All of the suits in the scene
    private LivingArmourAI[] suits;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        suits = FindObjectsOfType<LivingArmourAI>();
        rb = GetComponent<Rigidbody>();

    }
    /// <summary>
    /// on collision, play a sound and add the object that made the sound to the list of places of interest for the ai to go and walk to.
    /// </summary>
    /// <param name="collision">Object that it collided with</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= minSpeedToMakeSound)
        {
            // For some reason this function can be called before start, this is to
            // combat that
            if (suits == null) { Start(); }
            try
            {
                audioSource.clip = clip;

            }
            catch (System.Exception)
            {

                throw;
            }
            // audioSource.PlayOneShot(clip);
            audioSource.Play();
            madeSound = true;
            foreach (var suit in suits)
            {
                if (suit.isActiveAndEnabled)
                {
                    suit.soundSources.Add(gameObject.transform.position);
                }
            }
        }
    }
}
