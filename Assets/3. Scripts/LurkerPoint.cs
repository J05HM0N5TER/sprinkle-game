using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LurkerPoint : MonoBehaviour
{
    public bool isVisable = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.GetComponent<Renderer>().isVisible)
        {
            isVisable = true;
        }
        else
        {
            isVisable = false;
        }
    }
}
