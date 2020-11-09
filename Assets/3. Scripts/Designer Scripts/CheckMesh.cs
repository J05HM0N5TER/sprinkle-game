using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMesh : MonoBehaviour
{
    public Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        if (mesh != null)
        {
            Debug.Log("Vertices: " + mesh.vertexCount);
            Debug.Log("Triangles: " + mesh.triangles.Length);
        }
    }
}
