using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LurkerPoint : MonoBehaviour
{
    private Renderer ren;
    // Start is called before the first frame update
    void Start()
    {
        ren = gameObject.GetComponent<Renderer>();
    }

    public bool IsVisable()
    {
        return ren.isVisible;
    }
}
