using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SludgeScript : MonoBehaviour
{
    private GameObject player;
    private Camera playerCamera;
    [Tooltip("max distance away to interact with")]
    public float maxDistanceToInteract;
    [Tooltip("the amnount of time that the spray and animations/shaders will play")]
    public float timeOfplaying;
    private bool decreaseSize;
    private Vector3 sizeChange = new Vector3(0.1f, 0.1f, 0.0f);
    //sound
    private AudioSource audioS;
    [Tooltip("sound the sludge makes")]
    public AudioClip sludgeSound;
    [Tooltip("the object that will make the spray sound and have the particles attached to")]
    public GameObject sprayObject;
    [Tooltip("Sound of the Spray")]
    public AudioClip spraySound;
    //particles
    //public GameObject sprayParticles;
    private ParticleSystem ps;
    public float timeTillDelete = 2;
    public GameObject sludgePlane;
    public float fadespeed = 0.5f;
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioS = GetComponent<AudioSource>();
        ps = sprayObject.GetComponent<ParticleSystem>();
        playerCamera = player.GetComponentInChildren<Camera>();
        //rend = sludgePlane.GetComponent<Renderer>();
        color = sludgePlane.GetComponent<SkinnedMeshRenderer>().material.color;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && // Raycast check
                hit.collider.gameObject == gameObject && // Raycast hit this object
                Vector3.Distance(hit.point, playerCamera.transform.position) <= maxDistanceToInteract) // The player is in range 
            {
                if (player.GetComponent<PlayerController>().inventory.HasFlag(PlayerController.Inventory.ChemicalSpray))
                {
                    //PlaySpray();
                    audioS.PlayOneShot(sludgeSound);
                    audioS.PlayOneShot(spraySound);
                    
                    gameObject.GetComponent<Collider>().enabled = false;
                    decreaseSize = true;
                    //gameObject.SetActive(false);
                }
            }
        }
        if (decreaseSize == true)
        {
            
            
            color = sludgePlane.GetComponent<Renderer>().material.GetColor("_BaseColor");
            color.a -= Time.deltaTime * fadespeed;
            sludgePlane.GetComponent<Renderer>().material.SetColor("_BaseColor", color);
            if(color.a == 0)
            {
                color.a = 0;
            }
            
            timeTillDelete -= Time.deltaTime;
            if (timeTillDelete <= 0)
            {
                gameObject.SetActive(false);
            }
        }
        
    }
    private IEnumerator PlaySpray()
    {
        // var em = ps.emission;
        // em.enabled = true;
        // ps.Play();
        yield return StartCoroutine("TimeOfSpray");
    }
    private IEnumerator TimeOfSpray()
    {
        yield return new WaitForSeconds(timeOfplaying);
        // var em = ps.emission;
        // em.enabled = false;
        // ps.Stop();
        gameObject.SetActive(false);
    }

}
