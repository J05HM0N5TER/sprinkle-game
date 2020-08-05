using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Scene names:")]
    [SerializeField] private string mainMenu;
    [SerializeField] private string mainGame;

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
    public void LaunchMainGame() => SceneChange(mainGame);

    /// <summary>
    /// Navigates to the main menu scene
    /// </summary>
    public void LaunchMainMenu() => SceneChange(mainMenu);

    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
