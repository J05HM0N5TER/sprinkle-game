using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionNoiseManager : MonoBehaviour
{
	[Tooltip("How long the object has the ability to make noise after the player interacts with it")]
	public float scriptLifeTime = 5;
	[Tooltip("The volume of the sound")]
	public float volume;
	[Tooltip("The noise that object make when things collide with them")]
	public List<AudioClip> collisionClips = new List<AudioClip>();
	[Tooltip("Minimum speed something must go to be able to create noise")]
	public float minSpeedToMakeSound = 1.0f;
	// Start is called before the first frame update
	private class CollisionObject
	{
		public DateTime timeAdded;
		public CollisionNoise script;
		public AudioSource audio;
		public CollisionObject(CollisionNoise newScript, AudioSource newAudio)
		{
			timeAdded = DateTime.Now;
			script = newScript;
			audio = newAudio;
		}
	}

	List<CollisionObject> noisesScripts = new List<CollisionObject>(10);

	void Update()
	{
		RemoveOld();
	}

	public void RemoveOld()
	{
		List<CollisionObject> itemToDestoy = new List<CollisionObject>();
		foreach (var item in noisesScripts)
		{
			// If there is one that is too old
			if ((DateTime.Now - item.timeAdded).TotalSeconds > scriptLifeTime)
			{
				// Remove
				itemToDestoy.Add(item);
			}
		}
		foreach (var item in itemToDestoy)
		{
			noisesScripts.Remove(item);
			Destroy(item.script);
			Destroy(item.audio);
		}
	}

	public void Add(CollisionNoise newNoise, AudioSource newAudio)
	{
		CollisionObject temp = new CollisionObject(newNoise, newAudio);
		noisesScripts.Add(temp);
	}

	public void Add(GameObject objectInteractedWith)
	{
		CollisionNoise script = objectInteractedWith.GetComponent<CollisionNoise>();
		AudioSource audio = objectInteractedWith.GetComponent<AudioSource>();
		// If it already has a script
		if (script)
		{
			// Find it in the array
			CollisionObject oldObject = noisesScripts.Find(
				delegate(CollisionObject a)
				{
					return (a.script == script);
				}
			);
			// Change the added time to now
			oldObject.timeAdded = DateTime.Now;
		}
		else
		{
			script = objectInteractedWith.AddComponent<CollisionNoise>();
		}
		// If there is no audio add it
		if (!audio)
		{
			audio = objectInteractedWith.AddComponent<AudioSource>();
		}
		script.minSpeedToMakeSound = minSpeedToMakeSound;
		script.volume = volume;
		script.audioSource = audio;
		script.collisionClips = collisionClips;

		// Add the new stuff
		Add(script, audio);
	}
}
