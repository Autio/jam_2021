using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    // For each wall, draw the frontside a bit differently
    public Material altWallMaterial;  

    // Start is called before the first frame update
    void Start()
    {
        MakePatchy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakePatchy()
    {
        foreach(Transform wall in transform)
            {
                if(Random.Range(0,10) < 2)
                {
                    try {
                    wall.GetComponent<Renderer>().material = altWallMaterial;
                    }
                    catch{
                        // No material on child
                    }
                }
            }
        }
}
