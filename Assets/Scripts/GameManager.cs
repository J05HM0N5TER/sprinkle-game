using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Scene names:")]
    [SerializeField] private string mainMenu;
    [SerializeField] private string mainGame;

    private void Awake()
    {
        if (FindObjectOfType<GameManager>() != null)
            Destroy(gameObject);
    }

    private void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void LaunchMainGame()
    {
        SceneChange(mainGame);
    }

    public void LaunchMainMenu()
    {
        SceneChange(mainGame);
    }
}
