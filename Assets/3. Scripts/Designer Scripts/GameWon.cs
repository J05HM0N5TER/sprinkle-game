using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWon : MonoBehaviour
{
    public string creditsScene;

    private void OnTriggerEnter(Collider other)
    {
        // If the Player enteres this trigger it sends them to the credits scene.
        if (other.tag == "Player")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(creditsScene);
        }
    }
}
