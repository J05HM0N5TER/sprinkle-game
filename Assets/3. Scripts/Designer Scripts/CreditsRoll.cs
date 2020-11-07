using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsRoll : MonoBehaviour
{
    public GameObject creditsPanel;
    public int travelDistance;
    public float moveSpeed;

    void FixedUpdate()
    {
        Vector2 pos = creditsPanel.transform.position;
        for (int i = 0; i < travelDistance; i++)
        {
            pos.y += moveSpeed * Time.deltaTime;
            creditsPanel.transform.position = pos;
        }
    }
}
