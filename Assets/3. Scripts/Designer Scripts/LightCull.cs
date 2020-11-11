using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightCull : MonoBehaviour
{
    [Tooltip("Manually start process of culling lights")]
    public bool cullNow = false;
    [Tooltip("Time between culling (in seconds)")]
    public float cullInterval = 1;
    [Tooltip("How far away the light has to be to be disabled")]
    public float cullDistance = 10;
    // Time since it checked lights and disabled them
    float timeSinceCull = 0;
    // All of the lights in the scene
    List<Light> lights;
    GameObject player;
    private void Start()
    {
        lights = new List<Light>(FindObjectsOfType<Light>());
        player = FindObjectOfType<PlayerController>().gameObject;
    }
    private void Update()
    {
        if (cullNow)
        {
            timeSinceCull += cullInterval;
            cullNow = false;
        }

        timeSinceCull += Time.deltaTime;
        
        if (timeSinceCull >= cullInterval)
        {
            foreach (var light in lights)
            {
                if (Vector3.Distance(player.transform.position, light.transform.position) > cullDistance)
                {
                    light.enabled = false;
                }
                else
                {
                    light.enabled = true;
                }
            }

            timeSinceCull -= cullInterval;
        }
    }
}
