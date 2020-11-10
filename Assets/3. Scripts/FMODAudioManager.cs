using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODAudioManager : MonoBehaviour
{

	public enum AudioState
	{
		GameEnter,
		Credits,
		MainMenu
	}

	[Flags] public enum AudioExtras
	{
		Chase,
		Area60s,
		PauseFocus
	}
	float catchingUpAmount = 0;
	// Start is called before the first frame update
	public FMOD.Studio.EventInstance eventInstance;
	[HideInInspector] public DateTime timeAdded;
	public bool fadeTest = false;
	private void Awake()
	{
		DontDestroyOnLoad(this);
		// If there is more then one audio manager, delete all but the oldest of them
		timeAdded = DateTime.Now;
		List<FMODAudioManager> audioManagers = new List<FMODAudioManager>(FindObjectsOfType<FMODAudioManager>());
		FMODAudioManager oldestAudioManager = this;

		foreach (var item in audioManagers)
		{
			if (item.timeAdded < oldestAudioManager.timeAdded)
			{
				oldestAudioManager = item;
			}
		}
		audioManagers.Remove(oldestAudioManager);
		foreach (var item in audioManagers)
		{
			Destroy(item);
		}
		audioManagers.Clear();
	}
	void Start()
	{
		// EventIn = FMODUnity.RuntimeManager.CreateInstance("event:/MASTER TRACK");
		eventInstance = GetComponent<FMODUnity.StudioEventEmitter>().EventInstance;
		// EventIn.start();
	}

	public void ChangeState(AudioState newState)
	{

		switch (newState)
		{
			case AudioState.GameEnter:
				FindObjectOfType<FMODAudioManager>().eventInstance.setParameterByName("Game enter", 1);
				FindObjectOfType<FMODAudioManager>().eventInstance.setParameterByName("Credits", 0);

				break;
			case AudioState.Credits:
				FindObjectOfType<FMODAudioManager>().eventInstance.setParameterByName("Game enter", 0);
				FindObjectOfType<FMODAudioManager>().eventInstance.setParameterByName("Credits", 1);
				break;
			case AudioState.MainMenu:
				// FindObjectOfType<FMODAudioManager>().eventInstance.setParameterByName("Credits",
				// 1);
				// TODO: Find out how to change back to main menu
				break;
			default:
				Debug.LogError("Invalid audioState", this);
				break;
		}
	}

	// Update is called once per frame
	// void Update()
	// {
	//     if (fadeTest)
	//     {
	//         FadeIn();
	//     }
	//     else
	//     {
	//         FadeOut();
	//     }
	// }

	// public void FadeIn()
	// {
	//     eventInstance.setParameterByName("Chase", 1f);
	// }
	// public void FadeOut()
	// {
	//     eventInstance.setParameterByName("Chase", 0f);
	// }

}
