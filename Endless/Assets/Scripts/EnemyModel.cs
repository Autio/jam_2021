using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {

        if(collision.gameObject.layer == 8)
        {
            transform.parent.GetComponent<Blob>().SetJumping(false);
            transform.parent.GetComponent<Blob>().CreateGroundParticles(this.transform.position);
        }
    }
}
