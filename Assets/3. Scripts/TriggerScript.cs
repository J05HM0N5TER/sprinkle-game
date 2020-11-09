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
    private AudioSource audioS;
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
    [HideInInspector] public bool thingInbetweenDoors;

    public Material unlockedmat;
    public Material lockedmat;
    public GameObject keypadLeft;
    public GameObject ledLeft;
    public GameObject keypadRight;
    public GameObject ledRight;
    public GameObject nvLinkStart;
    public GameObject nvLinkEnd;
    // Start is called before the first frame update
    void Start()
    {
       audioS = soundSource.GetComponent<AudioSource>();
       resettimer = timer;
       keypadLeft.GetComponent<MeshRenderer>().material = unlockedmat;
       ledLeft.GetComponent<MeshRenderer>().material = unlockedmat;
       keypadRight.GetComponent<MeshRenderer>().material = unlockedmat;
       ledRight.GetComponent<MeshRenderer>().material = unlockedmat;
    }
    private void Update()
    {
        
        timer -= Time.deltaTime;
        if (thingsInDoorway.Count == 0 && timer <= 0 && doorOpen && !locked && !thingInbetweenDoors)
        {
            animatedObject.GetComponent<Animator>().SetTrigger("Close");
            audioS.PlayOneShot(Closeclip);
            doorOpen = false;
            if (oneTimeUse)
            {
                hasplayedonce = true;
            }
        }
        if(thingsInDoorway.Count > 0 && !locked && !doorOpen)
        {
            animatedObject.GetComponent<Animator>().SetTrigger("Open");
            audioS.PlayOneShot(Openclip);
            timer = resettimer;
            doorOpen = true;
            if(oneTimeUse)
            {
                hasplayedonce = true;
            }
        }
        if(locked)
        {
            keypadLeft.GetComponent<MeshRenderer>().material = lockedmat;
            ledLeft.GetComponent<MeshRenderer>().material = lockedmat;
            keypadRight.GetComponent<MeshRenderer>().material = lockedmat;
            ledRight.GetComponent<MeshRenderer>().material = lockedmat;

            nvLinkStart.SetActive(false);
            nvLinkEnd.SetActive(false);
        }
        else
        {
            keypadLeft.GetComponent<MeshRenderer>().material = unlockedmat;
            ledLeft.GetComponent<MeshRenderer>().material = unlockedmat;
            keypadRight.GetComponent<MeshRenderer>().material = unlockedmat;
            ledRight.GetComponent<MeshRenderer>().material = unlockedmat;

            nvLinkStart.SetActive(false);
            nvLinkEnd.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!hasplayedonce)
        {
            if (other.tag == "Player" || other.tag == "Suit")
            {
                thingsInDoorway.Add(other.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!hasplayedonce)
        {
            if (other.tag == "Player" || other.tag == "Suit")
            {
                thingsInDoorway.Remove(other.gameObject);
                
            }    
        }
    }
}
