using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
#pragma warning disable 649
    [Header("Scene names:")]
    [SerializeField] private string mainMenu;
    [SerializeField] private string mainGame;
    [SerializeField] private string gameCredits;
#pragma warning restore 649

    /// <summary>
    /// Changes the scene
    /// </summary>
    /// <param name="sceneName">The name of the scene file to be opened</param>
    static private void SceneChange(string sceneName)
    {
        Debug.Log("Loading " + sceneName);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    /// <summary>
    /// Navigates to the main game scene
    /// </summary>
    public void LaunchMainGame()
    {
        FindObjectOfType<FMODAudioManager>().ChangeState(FMODAudioManager.AudioState.GameEnter);
        // FindObjectOfType<FMODAudioManager>().eventInstance.setParameterByName("Game enter", 1);
        SceneChange(mainGame);
    }

    /// <summary>
    /// Navigates to the main menu scene
    /// </summary>
    public void LaunchMainMenu()
    {
        SceneChange(mainMenu);
    }

    /// <summary>
    /// Navigates to the main menu scene
    /// </summary>
    public void LaunchGameCredits()
    {
        FindObjectOfType<FMODAudioManager>().ChangeState(FMODAudioManager.AudioState.Credits);
        SceneChange(gameCredits);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
