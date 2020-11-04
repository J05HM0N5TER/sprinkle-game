using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class CreditsTimer : MonoBehaviour
{

    public int timeLeft = 60; //Seconds overall
    public Text countdown; //UI countdown timer
    public string nextScene;
    public GameObject Thankyou;

    void Start()
    {
        StartCoroutine("LoseTime");
        Time.timeScale = 1; //Setting the correct time scale
    }
    void Update()
    {
        countdown.text = ("" + timeLeft); //Showing the time on the Canvas

        if (timeLeft <= 0.0f)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }

        if (timeLeft <= 7.0f)
        {
            Thankyou.SetActive(true);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        }

    }
    //Coroutine
    IEnumerator LoseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            timeLeft--;
        }

    }

}
