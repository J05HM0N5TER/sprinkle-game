using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayAnimation : MonoBehaviour
{
	// Start is called before the first frame update
	[Tooltip("How long the delay is between triggering it and when the animation plays (in seconds)")]
	public float delay;
	[Tooltip("The animator that the state is going to be called on after the delay")]
	public Animator animator;
	[Tooltip("The state that the animator is changed to (in the animator graph thing)")]
	public string animatorPlayName;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			// animator = GetComponent<Animator>();
			StartCoroutine("DelayAnimationStart");
		}
	}

	private IEnumerator DelayAnimationStart()
	{
		yield return new WaitForSeconds(delay);
		animator.Play(animatorPlayName);
        Destroy(this);
	}

}
