using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour //This is a script written by Connor Force used for level design testing.
{

    public Transform player;

    private void LateUpdate()
    {
        Vector3 newPosition = player.position;//This makes the minimap follow the Player's position.
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f); //Turn this off to stop map rotation.
    }




}
