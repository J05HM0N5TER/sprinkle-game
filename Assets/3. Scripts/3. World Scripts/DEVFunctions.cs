using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DEVFunctions : MonoBehaviour
{
    public string DevScene = ""; //The scene that will be loaded.
    public string GameScene = "";
    public string LevelDesignScene = "";
    public string ProgrammerScene = "";

    void Update()
    {

        //if (Input.GetKey(KeyCode.Tilde)) //Load the dev scene from the main game.
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            SceneManager.LoadScene(DevScene);
            Debug.Log("Developer Room Loaded");
        }

        if (Input.GetKey(KeyCode.KeypadMultiply)) //Loads the main game.
        {
            SceneManager.LoadScene(GameScene);
            Debug.Log("Welcome to the Dark");
        }

        if (Input.GetKey(KeyCode.Backspace)) //Quickly kill the game in its tracks.
        {
            Application.Quit();
            Debug.Log("Application Killed");
        }

        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            SceneManager.LoadScene(LevelDesignScene);
            Debug.Log("Plus Activated");
        }

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            //SceneManager.LoadScene(Sean^3);
            Debug.Log("Minus Activated");
        }
    }
}
