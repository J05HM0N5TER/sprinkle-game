using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
	[Tooltip("UI elements for the pause menu")]
	public GameObject pauseMenu;
	[Tooltip("Is the game paused")]
	public static bool isPaused = false;

	private GameManager manager = null;

	public GameManager Manager
	{
		get
		{
			if (!manager)
			{
				manager = FindObjectOfType<GameManager>();
				if (!manager)
                {
					Debug.LogError("No GameManager in scene!");
                }
			}
			return manager;
		}
	}

	private void Start()
	{
		GameObject.FindObjectOfType<GameManager>();
		pauseMenu.SetActive(false);
		ResumeGame();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetButtonDown("Pause"))
		{
			if (isPaused)
				ResumeGame();
			else
				PauseGame();
		}
	}

	/// <summary>
	/// Pauses the game and brings up the pause menu
	/// </summary>
	public void PauseGame()
	{
		UnityEngine.Cursor.lockState = CursorLockMode.None;
		isPaused = true;
		pauseMenu.SetActive(true);
		Time.timeScale = 0f;
		UnityEngine.Cursor.visible = true;
	}

	/// <summary>
	/// Resumes the game and hides the pause menu
	/// </summary>
	public void ResumeGame()
	{
		UnityEngine.Cursor.lockState = CursorLockMode.Locked;
		isPaused = false;
		pauseMenu.SetActive(false);
		Time.timeScale = 1f;
		UnityEngine.Cursor.visible = false;
	}
}
