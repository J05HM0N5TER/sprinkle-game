using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class CreditsTimer : MonoBehaviour
{
    public float timeLeft = 60f;
    public Text countdown; //UI countdown timer
    public string nextScene;
    public GameObject Thankyou;

    //void Start()
    //{
    //    //StartCoroutine("LoseTime");
    //    //Time.timeScale = 1; //Setting the correct time scale
    //}
    void FixedUpdate()
    {
        timeLeft -= Time.deltaTime;
        countdown.text = ("" + timeLeft); //Showing the time on the Canvas
        if (timeLeft <= 0.0f)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }

        if (timeLeft <= 5.0f)
        {
            Thankyou.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }

    }
    ////Coroutine
    //IEnumerator LoseTime()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(1);
    //        timeLeft--;
    //    }

    //}

}
