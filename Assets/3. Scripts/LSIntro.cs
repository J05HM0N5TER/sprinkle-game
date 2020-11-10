using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class LSIntro : MonoBehaviour
{
    private NavMeshAgent agent;
    public GameObject walkPoint1;
    public GameObject lookAtPoint;
    public float timeOfLookAround = 5;
    public GameObject disappearPoint;
    public float suitWalkSpeed = 5;
    private Vector3 lookdirection;

    private Color chaseLight = new Color(84, 31, 81, 1);
	private Color investigateLight = new Color(161, 100, 16, 1);
	private Color searchLight = new Color(66, 94, 68, 1);

    public GameObject visorLight;
	public Material visorEmission;
	private Light lightvisor;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        //lights
        lightvisor = visorLight.GetComponent<Light>();
		lightvisor.color = searchLight;
        //emissions /  colour
        visorEmission.SetColor("_EmissiveColor", searchLight);
		visorEmission.EnableKeyword("_EMISSION");
		visorEmission.color = searchLight;
        //animations
        anim = gameObject.GetComponentInChildren<Animator>();
        anim.gameObject.GetComponent<Animator>().enabled = true;
        //lights a colour of visor
        lightvisor.color = searchLight;
        visorEmission.SetColor ("_EmissiveColor", searchLight);
        visorEmission.EnableKeyword ("_EMISSION");
        //animations starts
        anim.SetBool("walking", true);
    }

    // Update is called once per frame
    void Update()
    {
        // walk to first point
        agent.speed = suitWalkSpeed;
        agent.SetDestination(walkPoint1.transform.position);
        
        // do look around animations
        if(Vector3.Distance( agent.transform.position , walkPoint1.transform.position) < 1 )
        {
            timeOfLookAround -= Time.deltaTime;
            //make it look the lookatpoint
            lookdirection = ( lookAtPoint.transform.position - agent.transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(lookdirection);
            transform.rotation = lookRotation;
        }
        if(timeOfLookAround > 0 && Vector3.Distance( agent.transform.position , walkPoint1.transform.position) < 1 )
        {
            //make it look the lookatpoint
            
            //change visor colour
            lightvisor.color = investigateLight;
            visorEmission.SetColor ("_EmissiveColor", investigateLight);
            visorEmission.EnableKeyword ("_EMISSION");
            // do look around animations
            anim.SetBool("walking", false);
            anim.SetBool("searching", true);
            
        }
        else if(timeOfLookAround <= 0)
        {
            lightvisor.color = searchLight;
            visorEmission.SetColor ("_EmissiveColor", searchLight);
            visorEmission.EnableKeyword ("_EMISSION");
            agent.SetDestination(disappearPoint.transform.position);
            anim.SetBool("walking", true);
            anim.SetBool("searching", false);
            if(Vector3.Distance( agent.transform.position , disappearPoint.transform.position) < 1 )
            {
                Destroy(gameObject);
            }
        }
    }
}
