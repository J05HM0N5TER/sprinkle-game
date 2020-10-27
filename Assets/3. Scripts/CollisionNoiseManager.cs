using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionNoiseManager : MonoBehaviour
{
    public float scriptLifeTime = 5;
    // Start is called before the first frame update
    private class CollisionObject
    {
        public DateTime timeAdded;
        public CollisionNoise script;
        public CollisionObject(CollisionNoise noise)
        {
            timeAdded = DateTime.Now;
            script = noise;
        }
    }

    List<CollisionObject> noises = new List<CollisionObject>(10);

    void Update()
    {
        RemoveOld();
    }

    public void RemoveOld()
    {
        foreach (var item in noises)
        {
            // If there is one that is too old
            if ((DateTime.Now - item.timeAdded).TotalSeconds > scriptLifeTime)
            {
                // Remove
                noises.Remove(item);
                Destroy(item.script);
            }
        }
    }

    public void Add(CollisionNoise newNoise)
    {
        CollisionObject temp = new CollisionObject(newNoise);
        noises.Add(temp);
    }

    public void Add(GameObject objectInteractedWith)
    {
        CollisionNoise oldScript = objectInteractedWith.GetComponent<CollisionNoise>();
        // If it already has a script
        if (oldScript)
        {
            // Find it in the array
            CollisionObject oldObject = noises.Find(
                delegate(CollisionObject a)
                {
                    return (a.script == oldScript);
                }
            );
            // Change the added time to now
            oldObject.timeAdded = DateTime.Now;
        }
        else
        {
            CollisionNoise newScript = objectInteractedWith.AddComponent<CollisionNoise>();
            Add(newScript);
        }
    }
}
