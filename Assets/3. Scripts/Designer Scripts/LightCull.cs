using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightCull : MonoBehaviour
{
    [Tooltip("Time between culling (in seconds)")]
    public float cullInterval = 1;
    [Tooltip("How far away the light has to be to be disabled")]
    public float cullDistance = 10;
    // Used for more efficient math
    private float sqrCullDistance;
    // Time since it checked lights and disabled them
    float timeSinceCull = 0;
    // All of the lights in the scene
    List<Light> lights;
    GameObject player;
    private void Start()
    {
        lights = new List<Light>(FindObjectsOfType<Light>());
        player = FindObjectOfType<PlayerController>().gameObject;
        // Get the square fo the distance for better performance
        sqrCullDistance = cullDistance * cullDistance;
    }
    private void Update()
    {
        timeSinceCull += Time.deltaTime;

        if (timeSinceCull >= cullInterval)
        {
            // Update distance every frame only if it can be changed
            // (if it is in the editor)
#if UNITY_EDITOR
            sqrCullDistance = cullDistance * cullDistance;
#endif

            foreach (var light in lights)
            {
                // If light is further away then the cull distance disable it
                if (Vector3.SqrMagnitude(player.transform.position - light.transform.position) < sqrCullDistance)
                {
                    light.enabled = true;
                }
                else
                {
                    light.enabled = false;
                }
            }

            timeSinceCull = 0;
        }
    }
}
