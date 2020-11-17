using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeathScript : MonoBehaviour
{
    private GameObject player;
    public GameObject respawnPoint;
    private Animator anim;
    public GameObject blackScreen;

    public float timeTillScreenBlack = 3;
    public float respawnTime = 2;

    public float timeTillCanMoveAgain = 6;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = player.GetComponentInChildren<Animator>();
        anim.enabled = false;
        blackScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetComponent<PlayerController>().health <= 0)
        {
            anim.enabled = true;
            player.GetComponent<PlayerController>().enabled = false;
            anim.Play("P_Death");
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            screenblack();
            player.GetComponent<PlayerController>().health = 100;
        }
    }
     private void screenblack()
	{   
		StartCoroutine("makeblack");
	}
	private IEnumerator makeblack ()
	{
		yield return new WaitForSeconds(timeTillScreenBlack);
        blackScreen.SetActive(true);
        respawn();
        player.transform.position = respawnPoint.transform.position;
	}
    private void respawn()
	{   
		StartCoroutine("respawntime");
	}
	private IEnumerator respawntime ()
	{
		yield return new WaitForSeconds(respawnTime);
        blackScreen.SetActive(false);
        anim.Play("P_Revive");
        canmoveagain();
	}
    private void canmoveagain()
	{   
		StartCoroutine("moveagain");
	}
	private IEnumerator moveagain ()
	{
		yield return new WaitForSeconds(timeTillCanMoveAgain);
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        player.GetComponent<PlayerController>().enabled = true;
        anim.enabled = false;
        player.GetComponent<PlayerController>().health = 100;
	}
    
}
