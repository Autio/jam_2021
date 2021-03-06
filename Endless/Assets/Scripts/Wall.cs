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
        foreach(Transform wallblock in transform)
        {
            if(Random.Range(0,10) < 2)
            {
                wallblock.GetComponent<Renderer>().material = altWallMaterial;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
